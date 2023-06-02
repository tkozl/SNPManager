class ErrorRsp:

    INVALID_EMAIL = 'invalid_email'
    WRONG_PASSWORD = 'wrong_password'
    TOO_LOW_PASSWORD_COMPLEXITY = 'too_low_password_complexity'
    INVALID_NAME = 'invalid_name'
    TOO_LONG_STRING = 'too_long_string'
    ALREADY_EXISTS = 'already_exists'
    UNKNOWN_ENCRYPTION_TYPE = 'unknown_encryption_type'
    TOKEN_REQUIRED = 'token_required'
    INVALID_TOKEN = 'invalid_token'

    def __init__(self):
        self.__errors = {
            'errors': []
        }

    def add(self, error_id :str, error_msg :str=None) -> None:
        self.__errors['errors'].append({
            'errorID': error_id,
            'description': error_msg
        })
    
    @property
    def json(self) -> dict:
        return self.__errors

    @property
    def quantity(self) -> int:
        return len(self.__errors['errors'])
