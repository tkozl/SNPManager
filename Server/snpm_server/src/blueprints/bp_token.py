from flask import Blueprint
from Crypto.Hash import SHA512
from datetime import datetime

import src.models as models
import src.templates as templates



bp_token = Blueprint(name="blueprint_token", import_name=__name__)

@bp_token.route('/email-verify/<token>', methods=['GET'])
def verify_email_token(token :str):
    """Verifies user email"""

    # Validating token
    if len(token) != 128:
        return templates.url_error_page('It seems that url is broken. Please request new url in your app.'), 400
    token_hash = SHA512.new(data=bytes(token, 'utf-8')).hexdigest()
    user = models.User.query.filter_by(email_verify_token=token_hash, email_verified=False).first()
    if user == None:
        return templates.url_error_page('It seems that url is broken. Please request new url in your app.'), 400
    if user.email_verify_token_exp <= datetime.now():
        return templates.url_error_page('It seems that url has expired. Please request new url in your app.'), 400
    
    # Token is correct - changing sql data
    user.email_verified = True
    user.email_verify_token_exp = datetime.now()
    models.db.session.commit()

    return templates.url_success_page('Email verified. Now you can use your account.'), 201
