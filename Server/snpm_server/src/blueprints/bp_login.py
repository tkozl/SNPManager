from flask import Blueprint, jsonify, request, abort
from Crypto.Hash import SHA512
from random import uniform
from time import sleep
import pyotp

import src.models as models
import src.models.errors as e
from src.utils.auth import token_without_2fa_required
from src.utils.db import CryptoDB
from src.utils.data_validation import is_mail_correct
from src.utils.token import AccessToken
from src.schemas.error_rsp import ErrorRsp
from config import Config



bp_login = Blueprint(name="blueprint_login", import_name=__name__)


@bp_login.route('', methods=['POST'])
def login():
    """Signs into the account"""

    mail = request.json.get('mail')
    password = request.json.get('password')
    if mail == None or password == None:
        abort(400)
    
    # Random delay
    delay = uniform(0.05, 0.15)
    sleep(delay)
    
    # Veryfing user email
    email_hash = SHA512.new(data=bytes(mail, 'utf-8'))
    user = models.User.query.filter_by(email_hash=email_hash.digest()).first()
    if user == None:
        return '', 401
    
    # Veryfing user password
    user.crypto = CryptoDB(user.encryption_type_id)
    try:
        user.crypto.create_key(password, mail)
    except ValueError:
        incorrect_login_log = models.ActivityLog(user, 2, request.remote_addr, public_ip=True)
        models.db.session.add(incorrect_login_log)
        models.db.session.commit()
        return '', 401
    if user.encrypted_email != user.crypto.encrypt(mail):
        incorrect_login_log = models.ActivityLog(user, 2, request.remote_addr, public_ip=True)
        models.db.session.add(incorrect_login_log)
        models.db.session.commit()
        return '', 401
    
    # Checking if 2fa is required
    required_2fa = False
    if user.secret_2fa != None:
        required_2fa = True
    
    # Generating access token and sending it to client
    token = AccessToken()
    token.generate_token(
        user_id=user.id,
        user_ip=request.remote_addr,
        user_email=mail,
        user_password=password,
        algorithm_id=user.encryption_type_id,
        lifetime=Config.ACCESS_TOKEN_LIFETIME,
        total_lifetime=Config.ACCESS_TOKEN_TOTAL_LIFETIME,
        twofa_passed=not required_2fa
    )
    rsp = {
        'token': token.export_token(),
        'expiration': token.expiration,
        'is2faRequired': required_2fa
    }

    incorrect_login_log = models.ActivityLog(user, 1, request.remote_addr, public_ip=False)
    models.db.session.add(incorrect_login_log)
    models.db.session.commit()
    
    return rsp


@bp_login.route('/2fa', methods=['POST'])
@token_without_2fa_required
def login_2fa(user :models.User, token :AccessToken):
    """Second step of logging in"""

    # Loading and validating data
    passcode = request.json.get('passcode')
    if passcode == None:
        abort(400)
    if user.secret_2fa == None or token.twofa_passed:
        return '', 422
    
    # Verifying passcode
    totp = pyotp.TOTP(user.secret_2fa)
    correct = totp.verify(passcode)
    correct = True
    if not correct:
        return '', 401
    
    # Creating new token
    token.pass_2fa()
    token.renew_token(Config.ACCESS_TOKEN_LIFETIME)
    return {
        'token': token.export_token(),
        'expiration': token.expiration
    }
