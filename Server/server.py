from flask import Flask, request, abort, current_app
from flask_restx import Resource, Api
from os import getenv
from functools import wraps
import jwt
from time import time

from sql import SNPMSqlServer
from mail import Mail



class ClientError:

    INVALID_EMAIL = 'invalid_email'
    WRONG_PASSWORD = 'wrong_password'
    TOO_LOW_PASSWORD_COMPLEXITY = 'too_low_password_complexity'
    INVALID_NAME = 'invalid_name'
    TOO_LONG_STRING = 'too_long_string'
    ALREADY_EXISTS = 'already_exists'
    UNKNOWN_ENCRYPTION_TYPE = 'unknown_encryption_type'
    TOKEN_REQUIRED = 'token_required'
    INVALID_TOKEN = 'invalid_token'

    def __init__(self, error_id :str, error_msg :str=None):
        self.__error = error_id
        self.__msg = error_msg
    
    @property
    def error(self) -> int:
        return self.__error
    
    @property
    def msg(self) -> str:
        return self.__msg


class SNPMServer(Flask):

    def __init__(self, *args, **kwargs):
        super(SNPMServer, self).__init__(*args, **kwargs)
        sql_access = {
            'server': getenv('SQL_SERVER'),
            'database': getenv('SQL_SERVER_DATABASE'),
            'username': getenv('SQL_SERVER_USERNAME'),
            'password': getenv('SQL_SERVER_PASS'),
            'sql_driver': getenv('SQL_DRIVER')
        }
        mail_access = {
            'server': getenv('MAIL_SMTP_SERVER'),
            'port': getenv('MAIL_PORT'),
            'username': getenv('MAIL_USERNAME'),
            'password': getenv('MAIL_PASSWORD')
        }
        self.__secret_key = getenv('SERVER_SECRET_KEY')
        self.__sql = SNPMSqlServer(**sql_access)
        self.__mail = Mail(**mail_access)
        self.__api = Api(self)
    
    @property
    def sql(self) -> SNPMSqlServer:
        return self.__sql
    
    @property
    def mail(self) -> Mail:
        return self.__mail
    
    @property
    def api(self) -> Api:
        return self.__api
    
    def errors_response(self, errors :list[ClientError]) -> dict:
        res = {'errors': []}
        for error in errors:
            res['errors'].append({
                'errorID': error.error,
                'description': error.msg
            })
        return res

    def generate_token(self, user_id :int, password :str, lifetime :int=300) -> str:
        token = jwt.encode({
                'user_id': user_id,
                'expiration': int(time() + lifetime),
                'user_secret': password
            },
            self.__secret_key,
            algorithm="HS512"
        )
        return token
    
    def token_required(self, f):
        @wraps(f)
        def decorated(*args, **kwargs):
            token = None
            if "Authorization" in request.headers:
                token = request.headers["Authorization"].split(" ")[1]

            if not token:
                return self.errors_response([ClientError(ClientError.TOKEN_REQUIRED)]), 401
            try:
                data=jwt.decode(token, self.__secret_key, algorithms=["HS256"])
                current_user_id, current_user_secret, token_expiration = data.get('user_id', None), data.get('user_secret', ''), data.get('expiration', 0)
                if current_user_id is None or int(token_expiration) <= time():
                    return self.errors_response([ClientError(ClientError.INVALID_TOKEN)]), 401
            except Exception as e:
                abort(500)

            return f(current_user_id, current_user_secret, *args, **kwargs)

        return decorated


server = SNPMServer('SNPMServer')


@server.api.route('/hello')
class Account(Resource):

    @server.token_required
    def get(user_id, user_secret, self):
        print(user_id, user_secret)
        return {'msg': 'Hello World!'}
