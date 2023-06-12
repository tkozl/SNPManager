from flask import request, abort
from functools import wraps
from jwcrypto import jwk, jwe

from src.schemas import ErrorRsp
from src.models.users import User
from src.models.view_locked_users import LockedUserView
from src.utils.db import CryptoDB
from src.utils.token import AccessToken, IncorrectTokenError
from config import Config



def token_required(f):
    """
    Decorator indicating that a given endpoint requires tokenization.
    Sends a 401 message with an error description to the remote host if something is wrong with the attached token or if token is missing.
    Return:
        user (src.models.User), token (AccessToken)
    """

    @wraps(f)
    def decorated(*args, **kwargs):
        token = None
        errors = ErrorRsp()
        # Getting token from request header
        if "Authorization" in request.headers:
            try:
                tokenb64 = request.headers["Authorization"].split(" ")[1]
            except IndexError:
                tokenb64 = ''
            token = AccessToken()
            try:
                token.import_token(tokenb64)
            except IncorrectTokenError:
                errors.add(ErrorRsp.INVALID_TOKEN)
                return errors.json, 401
        else:
            # Token is missing
            errors.add(ErrorRsp.TOKEN_REQUIRED)
            return errors.json, 401
        try:
            # Validating token lifetime and user ip address
            if token.user_id is None or not token.is_valid or request.remote_addr != token.user_ip:
                errors.add(ErrorRsp.INVALID_TOKEN)
                return errors.json, 401

            # Getting user model from database
            user = User.query.filter_by(id=token.user_id).first()
            user.crypto = CryptoDB(token.algorithm_id)
            user.crypto.create_key(token.user_password, token.user_email)

            # Checking if account isn't locked
            if user.is_locked:
                errors.add(ErrorRsp.INVALID_TOKEN)
                return errors.json, 401

        except Exception as e:
            # Unknown error
            abort(500)

        # Token has been verified
        return f(user, token, *args, **kwargs)
    return decorated
