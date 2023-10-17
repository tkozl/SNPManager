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


@bp_token.route('/delete-account/<token>', methods=['GET'])
def delete_account(token :str):
    """Deletes account"""

    # Validating token
    if len(token) != 128:
        return templates.url_error_page('It seems that url is broken. Please request new url in your app.'), 400
    token_hash = SHA512.new(data=bytes(token, 'utf-8')).hexdigest()
    user = models.User.query.filter_by(user_del_token=token_hash, deleted_at=None).first()
    if user == None:
        return templates.url_error_page('It seems that url is broken. Please request new url in your app.'), 400
    if user.user_del_token_exp <= datetime.now():
        return templates.url_error_page('It seems that url has expired. Please request new url in your app.'), 400

    # Token is correct - removing all user data
    # Table "directories"
    directories = models.UserDirectoryView.query.filter_by(user_id=user.id).all()
    for directory in directories:
        directory = models.Directory.query.filter_by(id=directory.directory_id).first()
        directory.delete()

    # Table "entries"
    entries = models.UserEntryView.query.filter_by(user_id=user.id).all()
    for entry in entries:
        entry = models.Entry.query.filter_by(id=entry.entry_id).first()
        entry.delete()

        # Table "related_windows"
        related_windows = models.RelatedWindow.query.filter_by(entry_id=entry.id).all()
        for related_window in related_windows:
            related_window.delete()

        # Table "passwords"
        passwords = models.Password.query.filter_by(entry_id=entry.id).all()
        for password in passwords:
            password.delete()

        # Table "parameters"
        parameters = models.EntryParameter.query.filter_by(entry_id=entry.id).all()
        for parameter in parameters:
            parameter.delete()

    # Table "users"
    user.delete()

    # Saving changes
    models.db.session.commit()

    return templates.url_success_page('Account has been successfully deleted.'), 201
