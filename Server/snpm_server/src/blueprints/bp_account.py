from flask import Blueprint, jsonify, request, abort
from sqlalchemy import desc
import re
import pyotp
import random
import string
from datetime import datetime, timedelta
from time import mktime
from Crypto.Hash import SHA512

import src.models as models
import src.models.errors as e
from src.utils.auth import token_without_email_validation_required
from src.utils.token import AccessToken, TokenExpiredError
from src.utils.db import CryptoDB
from src.utils.data_validation import is_mail_correct, is_password_strong_enough, user_encryption_type_to_id, user_encryption_id_to_type
from src.schemas.error_rsp import ErrorRsp
from config import Config
import src.templates as templates



bp_account = Blueprint(name="blueprint_account", import_name=__name__)


@bp_account.route('', methods=['POST'])
def create_account():
    """Creates user account"""

    mail = request.json.get('mail')
    password = request.json.get('password')
    encryption_type = request.json.get('encryptionType')
    if mail == None or password == None or encryption_type == None:
        abort(400)
    
    # Validating user data
    errors = ErrorRsp()
    if not is_mail_correct(mail):
        errors.add(ErrorRsp.INVALID_EMAIL, 'Invalid email address')
    if not is_password_strong_enough(password):
        errors.add(ErrorRsp.TOO_LOW_PASSWORD_COMPLEXITY, 'Password does not meet required complexity rules')
    try:
        encryption_type_id = user_encryption_type_to_id(encryption_type)
    except ValueError:
        errors.add(ErrorRsp.UNKNOWN_ENCRYPTION_TYPE, 'Unknown encryption type')
    if len(password) > Config.MAX_PASSWORD_LEN:
        errors.add(ErrorRsp.TOO_LONG_STRING, f'Max password length is currently set to {Config.MAX_PASSWORD_LEN}')
    if len(mail) > Config.MAX_EMAIL_LEN:
        errors.add(ErrorRsp.TOO_LONG_STRING, f'Max email length is currently set to {Config.MAX_EMAIL_LEN}')
    if errors.quantity > 0:
        return errors.json, 400
    
    # Creating new account
    try:
        user = models.User(mail, password, encryption_type_id)
    except e.EmailCurrentlyInUseError:
        errors.add(ErrorRsp.INVALID_EMAIL, 'Invalid email address')
        return errors.json, 400
    models.db.session.add(user)
    models.db.session.commit()
    
    # Creating special directories
    root_dir = models.SpecialDir(user.id, models.SpecialDir.ROOT_ID)
    trash_dir = models.SpecialDir(user.id, models.SpecialDir.TRASH_ID)
    models.db.session.add(root_dir)
    models.db.session.add(trash_dir)
    models.db.session.commit()

    return '', 201


@bp_account.route('/token', methods=['POST'])
@token_without_email_validation_required
def renew_token(user :models.User, token :AccessToken):
    """Renews user access token"""

    try:
        token.renew_token(Config.ACCESS_TOKEN_LIFETIME)
    except TokenExpiredError:
        return '', 403
    
    return {
        'token': token.export_token(),
        'expiration': token.expiration
    }, 200


@bp_account.route('/2fa', methods=['POST'])
@token_without_email_validation_required
def create_2fa(user :models.User, token :AccessToken):
    """Creates 2fa mechanism"""

    if user.secret_2fa != None:
        return '', 403
    
    secret_2fa = pyotp.random_base32()
    user.secret_2fa = secret_2fa
    models.db.session.commit()

    return {
        'secretCode': secret_2fa
    }, 201


@bp_account.route('/2fa', methods=['DELETE'])
@token_without_email_validation_required
def disable_2fa(user :models.User, token :AccessToken):
    """Deletes 2fa mechanism"""

    user.secret_2fa = None
    models.db.session.commit()

    return '', 204


@bp_account.route('/verify-email', methods=['POST'])
@token_without_email_validation_required
def create_email_verification(user :models.User, token :AccessToken):
    """Creates and sends verification url to user"""

    if user.email_verified:
        abort(403)
    
    while True:
        token = ''.join(random.choice(string.ascii_letters+string.digits) for _ in range(128))
        token_hash = SHA512.new(data=bytes(token, 'utf-8')).hexdigest()
        users = models.User.query.filter_by(email_verify_token=token_hash, email_verified=False).all()
        if len(users) == 0:
            break
    user.email_verify_token = token_hash
    user.email_verify_token_exp = datetime.now() + timedelta(hours=1)
    url = f'{Config.SERVER_ADDRESS}/token/email-verify/{token}'
    user.send_mail(subject='Account verification', message_html=templates.email_verification_mail(url))
    models.db.session.commit()
    return '', 204


@bp_account.route('', methods=['GET'])
@token_without_email_validation_required
def get_account_info(user :models.User, token :AccessToken):
    """Gets account basic info"""

    # Loading args
    try:
        max_correct_logins = int(request.args.get('max_correct_logins', '10'))
        max_incorrect_logins = int(request.args.get('max_incorrect_logins', '10'))
    except ValueError:
        abort(400)
    
    # Loading last access data
    last_access_list = []
    access_history = models.ActivityLog.query.filter_by(user_id=user.id, activity_type_id=1).order_by(desc(models.ActivityLog.ocurred_at)).limit(max_correct_logins).all()
    for event in access_history:
        event.crypto = user.crypto
        ip = event.ip
        if ip == None:
            ip = event.obfuscated_ip
            if '.' in ip:
                ip += '.*'
            else:
                ip += ':*'
        last_access_list.append({
            'time': int(mktime(event.ocurred_at.timetuple())),
            'ip': ip
        })
    
    # Loading last incorrect logins data
    last_il_list = []
    il_history = models.ActivityLog.query.filter_by(user_id=user.id, activity_type_id=2).order_by(desc(models.ActivityLog.ocurred_at)).limit(max_incorrect_logins).all()
    for event in il_history:
        event.crypto = user.crypto
        ip = event.ip
        if ip == None:
            ip = event.obfuscated_ip
            if '.' in ip:
                ip += '.*'
            else:
                ip += ':*'
        last_il_list.append({
            'time': int(mktime(event.ocurred_at.timetuple())),
            'ip': ip
        })
    
    # Loading last password change date
    password_changed = models.ActivityLog.query.filter_by(user_id=user.id, activity_type_id=5).order_by(desc(models.ActivityLog.ocurred_at)).first()
    if password_changed == None:
        password_changed = user.created_at
    password_changed = int(mktime(password_changed.timetuple()))
    
    # Loading and and sending to client rest of data
    if user.secret_2fa == None:
        is_2fa_active = False
    else:
        is_2fa_active = True

    return {
        'mail': token.user_email,
        'encryptionType': user_encryption_id_to_type(user.encryption_type_id),
        'is2faActive': is_2fa_active,
        'isMailActive': user.email_verified,
        'creationDate': int(mktime(user.created_at.timetuple())),
        'numberOfEntries': int(models.UserEntryView.query.filter_by(user_id=user.id, deleted_at=None).count()),
        'lastPasswordChange': password_changed,
        'lastAccess': last_access_list,
        'lastLoginErrors': last_il_list
    }, 200
