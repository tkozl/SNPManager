from src.utils.db import CryptoDB
from pytest import raises



def test_cryptodb_keys():
    crypto = CryptoDB(CryptoDB.AES256)
    key, iv = crypto.create_key('sample password', 'mail@example.org')
    key2, iv2 = crypto.create_key('sample password', 'mail@example.org')
    key3, iv3 = crypto.create_key('sample password 2', 'mail@example.org')
    key4, iv4 = crypto.create_key('sample password', 'mail2@example.org')

    assert key == key2
    assert iv == iv2
    assert key != key3
    assert key != key4
    assert key3 != key4
    assert iv != iv4
    assert iv3 != iv4


def test_cryptodb_encrypt_decrypt():
    for algorithm in (CryptoDB.AES256, CryptoDB.AES192, CryptoDB.AES128):
        crypto = CryptoDB(algorithm)
        crypto.create_key('sample password', 'mail@example.org')

        sample_text = 'sample text'
        encrypted1 = crypto.encrypt(sample_text)
        encrypted2 = crypto.encrypt(sample_text)
        assert encrypted1 == encrypted2
        assert encrypted1 != sample_text

        crypto.create_key('sample password2', 'mail@example.org')
        encrypted3 = crypto.encrypt(sample_text)
        assert encrypted1 != encrypted3

        decrypted = crypto.decrypt(encrypted3)
        assert decrypted == sample_text
        with raises(ValueError):
            crypto.decrypt(encrypted1)
