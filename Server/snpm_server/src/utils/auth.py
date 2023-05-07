from flask import request, abort
from functools import wraps
import jwt
from time import time

from src.schemas import ErrorRsp
from config import Config



def generate_token(user_id :int, password :str, lifetime :int=300) -> str:
    token = jwt.encode({
            'user_id': user_id,
            'expiration': int(time() + lifetime),
            'user_secret': password
        },
        Config.SECRET_KEY,
        algorithm="HS512"
    )
    return token


def token_required(f):
    @wraps(f)
    def decorated(*args, **kwargs):
        token = None
        if "Authorization" in request.headers:
            token = request.headers["Authorization"].split(" ")[1]

        if not token:
            errors = ErrorRsp()
            errors.add(ErrorRsp.TOKEN_REQUIRED)
            return errors.json, 401
        try:
            data=jwt.decode(token, Config.SECRET_KEY, algorithms=["HS512"])
            current_user_id, current_user_secret, token_expiration = data.get('user_id', None), data.get('user_secret', ''), data.get('expiration', 0)
            if current_user_id is None or int(token_expiration) <= time():
                errors = ErrorRsp()
                errors.add(ErrorRsp.INVALID_TOKEN)
                return errors.json, 401
        except Exception as e:
            abort(500)

        return f(current_user_id, current_user_secret, *args, **kwargs)
    return decorated
