from flask import request, abort
from functools import wraps
from jwcrypto import jwk, jwe
from jwcrypto.common import json_encode, json_decode
from time import time
from base64 import b64encode, b64decode

from src.schemas import ErrorRsp
from src.models.users import User
from src.models.view_locked_users import LockedUserView
from src.utils.db import CryptoDB
from config import Config



def generate_token(user_id :int, user_ip :str, user_mail :str, user_password :str, algorithm_id :int, lifetime :int=300) -> str:
    """
    Generates access token
    Args:
        user_id (int): current user id
        user_ip (str): current user ip address
        user_mail (str): current user mail address
        user_password (str): current user password
        algorithm_id (int): id of user cryptographic algorithm (from CryptoDB class)
        lifetime (int): token lifetime in seconds (defaul: 300)
    Return:
        Access JWE token encoded with base64 (str)
    """

    # Loading key from server secret key
    key = jwk.JWK.from_password(Config.SECRET_KEY)

    # Preparing payload
    data = {
        'user_id': user_id,
        'user_ip': user_ip,
        'expiration': int(time() + lifetime),
        'user_mail': user_mail,
        'user_password': user_password,
        'algorithm_id': algorithm_id
    }
    payload = json_encode(data)

    # Preparing JWE token, encrypting it with AES256 CBC and signing with HS512
    jwetoken = jwe.JWE(payload.encode('utf-8'), json_encode({'alg': 'A256KW', 'enc': 'A256CBC-HS512'}))
    jwetoken.add_recipient(key)
    enc = jwetoken.serialize()

    # Encoding token with base64
    encb64 = b64encode(enc.encode('utf-8')).decode('utf-8')
    return encb64


def token_required(f):
    """
    Decorator indicating that a given endpoint requires tokenization.
    Sends a 401 message with an error description to the remote host if something is wrong with the attached token or if token is missing.
    Return:
        user (src.models.User) 
    """

    @wraps(f)
    def decorated(*args, **kwargs):
        token = None
        # Getting token from request header
        if "Authorization" in request.headers:
            tokenb64 = request.headers["Authorization"].split(" ")[1]
            token = b64decode(tokenb64).decode('utf-8')

        if not token:
            # Token is missing
            errors = ErrorRsp()
            errors.add(ErrorRsp.TOKEN_REQUIRED)
            return errors.json, 401
        try:
            # Decrypting and encoding payload
            jwetoken = jwe.JWE()
            jwetoken.deserialize(token)
            key = jwk.JWK.from_password(Config.SECRET_KEY)
            jwetoken.decrypt(key)
            payload = jwetoken.payload
            data = json_decode(payload)

            # Loading data from payload
            user_id = data.get('user_id', None)
            token_expiration = int(data.get('expiration', 0))
            user_email = data.get('user_mail', '')
            user_ip = data.get('user_ip', '')
            user_password = data.get('user_password', '')
            algorithm_id = int(data.get('algorithm_id', 0))

            # Validating token lifetime and user ip address
            if user_id is None or token_expiration <= time() or request.remote_addr != user_ip:
                errors = ErrorRsp()
                errors.add(ErrorRsp.INVALID_TOKEN)
                return errors.json, 401

            # Getting user model from database
            user = User.query.filter_by(id=user_id).first()
            user.crypto = CryptoDB(algorithm_id)
            user.crypto.generate_key(user_password, user_email)

            # Checking if account isn't locked
            if user.is_locked:
                errors = ErrorRsp()
                errors.add(ErrorRsp.INVALID_TOKEN)
                return errors.json, 401

        except Exception as e:
            # Unknown error
            abort(500)

        # Token has been verified
        return f(user, *args, **kwargs)
    return decorated
