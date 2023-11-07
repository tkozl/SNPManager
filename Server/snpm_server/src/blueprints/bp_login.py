from flask import Blueprint, jsonify, request, abort
from sqlalchemy import desc
from Crypto.Hash import SHA512
from random import uniform
from time import sleep
from datetime import datetime
import pyotp

import src.models as models
import src.models.errors as e
from src.utils.auth import token_without_2fa_required
from src.utils.db import CryptoDB
from src.utils.data_validation import is_mail_correct
from src.utils.token import AccessToken
from src.schemas.error_rsp import ErrorRsp
import src.templates as templates
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
    user = models.User.query.filter_by(email_hash=email_hash.digest(), deleted_at=None).first()
    if user == None:
        return '', 401

    # Checking if user is not blocked
    last_block = models.ActivityLog.query.filter_by(user_id=user.id, activity_type_id=3).order_by(desc(models.ActivityLog.ocurred_at)).first()
    if last_block != None and (datetime.now() - last_block.ocurred_at).total_seconds() < 60 * 60:
        # User is currently blocked
        return '', 401

    # Veryfing user password
    user.crypto = CryptoDB(user.encryption_type_id)
    incorrect_login = False
    try:
        user.crypto.create_key(password, mail)
    except ValueError:
        incorrect_login = True
    if incorrect_login == True or user.encrypted_email != user.crypto.encrypt(mail):
        # Incorrect login - saving stats
        incorrect_login_log = models.ActivityLog(user, 2, request.remote_addr, public_ip=True)
        models.db.session.add(incorrect_login_log)

        # Checking if user should be blocked
        last_incorrect_logins = models.UserIncorrectLogins24hView.query.filter_by(user_id=user.id).first()
        if last_incorrect_logins != None and last_incorrect_logins.incorrect_logins_quantity >= 10:
            # Blocking user account
            user_block_log = models.ActivityLog(user, 3, request.remote_addr, public_ip=True)
            models.db.session.add(user_block_log)
            user.send_mail(
                subject='Security alert: temporary account lock',
                message_html=templates.user_block_message(),
                mail_address=mail
            )
        else:
            # Sending email to user about incorrect login attempt
            user.send_mail(
                subject='Security alert: incorrect login attempt',
                message_html=templates.incorrect_login_message(request.remote_addr),
                mail_address=mail
            )

        models.db.session.commit()

        return '', 401

    # Checking if 2fa is required
    required_2fa = False
    if user.secret_2fa != None:
        required_2fa = True

    # Removing old data from trash
    entries_to_del = user.get_old_trashed_entries()
    dirs_to_del = user.get_empty_trashed_dirs()

    for del_entry_id in entries_to_del:
        del_entry = models.Entry.query.filter_by(id=del_entry_id).first()
        del_entry.crypto = user.crypto
        del_entry.delete()
    for del_dir_id in dirs_to_del:
        del_dir = models.Directory.query.filter_by(id=del_dir_id).first()
        del_dir.crypto = user.crypto
        del_dir.delete()

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
    if not correct:
        return '', 401

    # Creating new token
    token.pass_2fa()
    token.renew_token(Config.ACCESS_TOKEN_LIFETIME)
    return {
        'token': token.export_token(),
        'expiration': token.expiration
    }
