from flask import Blueprint, jsonify, request, abort
import re

import src.models as models
import src.models.errors as e
from src.utils.auth import token_required
from src.utils.token import AccessToken, TokenExpiredError
from src.utils.db import CryptoDB
from src.utils.data_validation import is_mail_correct, is_password_strong_enough, user_encryption_type_to_id
from src.schemas.error_rsp import ErrorRsp
from config import Config



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
@token_required
def renew_token(user :models.User, token :AccessToken):
    """Renews user access token"""

    try:
        token.renew_token(Config.ACCESS_TOKEN_LIFETIME)
    except TokenExpiredError:
        abort(403)
    
    return {
        'token': token.export_token(),
        'expiration': token.expiration
    }, 200
