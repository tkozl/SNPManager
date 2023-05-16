from pytest import raises
from time import sleep

from src.utils.token import AccessToken, TokenExpiredError, IncorrectTokenError



def test_access_token():

    token = AccessToken()
    token.generate_token(
        user_id=1,
        user_ip='82.4.2.66',
        user_email='test@example.com',
        user_password='password',
        algorithm_id=1,
        lifetime=300,
        total_lifetime=700
    )
    assert token.is_valid == True

    token_str = token.export_token()

    # Testing import
    token2 = AccessToken()
    with raises(IncorrectTokenError):
        token2.import_token('')
    with raises(IncorrectTokenError):
        token2.import_token('1' + token_str[1:])
    token2.import_token(token_str)
    token2_str = token2.export_token()

    assert token_str == token2_str
    assert token2.user_id == 1
    assert token2.user_ip == '82.4.2.66'
    assert token2.user_email == 'test@example.com'
    assert token2.user_password == 'password'
    assert token2.algorithm_id == 1
    
    # Testing renew
    token3 = AccessToken()
    token3.import_token(token2_str)
    token3.renew_token(lifetime=1)
    sleep(1) # now token3 should expire

    token2.renew_token(lifetime=300)
    assert token2.is_valid == True
    assert token2.export_token() != token_str
    assert token2.expiration > token.expiration

    token2.renew_token(lifetime=700) # lifetime greater than max allowed lifetime
    assert token2.expiration == token2.renewal_expiration
    
    # Testing token expiration
    with raises(TokenExpiredError):
        token3.renew_token(lifetime=2)
    assert token3.is_valid == False
