from jwcrypto import jwk, jwe
from jwcrypto.common import json_encode, json_decode
from time import time
from base64 import b64encode, b64decode

from config import Config



class TokenExpiredError(Exception):
    pass


class IncorrectTokenError(Exception):
    pass


class AccessToken:
    """Managing API access token"""

    def __init__(self) -> None:
        self.__key = Config.SECRET_KEY
        self.__token = None
        self.__data = {
            'user_id': None,
            'user_ip': None,
            'expiration': 0,
            'renewal_expiration': 0,
            'user_mail': None,
            'user_password': None,
            'algorithm_id': None
        }
    
    @property
    def user_id(self) -> int:
        return int(self.__data['user_id'])

    @property
    def user_ip(self) -> str:
        return self.__data['user_ip']
    
    @property
    def user_email(self) -> str:
        return self.__data['user_mail']
    
    @property
    def algorithm_id(self) -> int:
        return int(self.__data['algorithm_id'])
    
    @property
    def expiration(self) -> int:
        return int(self.__data['expiration'])
    
    @property
    def renewal_expiration(self) -> int:
        return int(self.__data['renewal_expiration'])

    @property
    def user_password(self) -> str:
        return self.__data['user_password']
    
    @property
    def is_valid(self) -> bool:
        """Is token valid"""
        if time() < self.expiration:
            return True
        else:
            return False
    
    def export_token(self) -> str:
        """
        Exports token from object
        Return:
            access token in base64 (str)
        """
        return self.__token
    
    def import_token(self, token :str) -> None:
        """
        Imports token to object.
        Args:
            token (str): access token in base64
        """

        try:
            tokenb64 = token
            token = b64decode(tokenb64).decode('utf-8')
            jwetoken = jwe.JWE()
            jwetoken.deserialize(token)
            key = jwk.JWK.from_password(self.__key)
            jwetoken.decrypt(key)
            payload = jwetoken.payload
            data = json_decode(payload)
            keys = data.keys()
        except Exception:
            raise IncorrectTokenError
        if 'user_id' in keys and 'user_ip' in keys and 'user_mail' in keys and 'user_password' in keys and 'algorithm_id' in keys and 'expiration' in keys and 'renewal_expiration' in keys:
            self.__data = data
            self.__token = tokenb64
        else:
            raise IncorrectTokenError

    def generate_token(self, user_id :int, user_ip :str, user_email :str, user_password :str, algorithm_id :int, lifetime :int=300, total_lifetime :int=43200) -> int:
        """
        Generates access token
        Args:
            user_id (int): current user id
            user_ip (str): current user ip address
            user_mail (str): current user mail address
            user_password (str): current user password
            algorithm_id (int): id of user cryptographic algorithm (from CryptoDB class)
            lifetime (int): token lifetime in seconds (defaul: 300)
            total_lifetime (int): token max allowed lifetime in seconds considering token renewal (default: 43200)
        Return:
            token expiration unix time in seconds (int)
        """

        if total_lifetime < lifetime:
            raise ValueError('Total lifetime must be greater then or equal to lifetime')

        # Preparing payload
        cur_time = time()
        self.__data = {
            'user_id': user_id,
            'user_ip': user_ip,
            'expiration': int(cur_time + lifetime),
            'renewal_expiration': int(cur_time + total_lifetime),
            'user_mail': user_email,
            'user_password': user_password,
            'algorithm_id': algorithm_id
        }
        if self.__data['expiration'] > self.__data['renewal_expiration']:
            self.__data['expiration'] = self.__data['renewal_expiration']
        payload = json_encode(self.__data)

        # Preparing JWE token, encrypting it with AES256 CBC and signing with HS512
        jwetoken = jwe.JWE(payload.encode('utf-8'), json_encode({'alg': 'A256KW', 'enc': 'A256CBC-HS512'}))
        jwetoken.add_recipient(jwk.JWK.from_password(self.__key))
        enc = jwetoken.serialize()

        # Encoding token with base64
        encb64 = b64encode(enc.encode('utf-8')).decode('utf-8')
        self.__token = encb64

        return self.__data['expiration']
    
    def renew_token(self, lifetime :int=300) -> int:
        """
        Renews the token without exceeding renewal_expiration time
        Args:
            lifetime (int): token lifetime in seconds (defaul: 300)
        Return:
            token expiration unix time in seconds (int)
        """

        if not self.is_valid:
            raise TokenExpiredError(f'Token expired on {self.expiration}')
        total_lifetime = int(self.renewal_expiration - time())
        if lifetime > total_lifetime:
            lifetime = total_lifetime
        if total_lifetime <= 0:
            raise TokenExpiredError(f'Token expired on {self.renewal_expiration}')
        expiration = self.generate_token(self.user_id, self.user_ip, self.user_email, self.__data['user_password'], self.algorithm_id, lifetime, total_lifetime)
        if expiration <= 0:
            raise TokenExpiredError(f'Token expired on {self.renewal_expiration}')
        return expiration
