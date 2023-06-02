import re


def is_mail_correct(mail :str) -> bool:
    """
    Checks if provided email address is correct
    Return:
        bool
    """

    res = re.match(r'.+@.+\..+', mail)
    if not res:
        return False
    else:
        return True


def is_password_strong_enough(password :str) -> bool:
    """
    Checks if provided password is strong enought
    Return:
        bool
    """

    if len(password) >= 12:
        return True
    else:
        return False


def user_encryption_type_to_id(encryption_type :str) -> int:
    """
    Converts provided by user encryption type string to id.
    Args:
        encryption_type (str): Encryption type string. If incorrect, raises ValueError
    Return:
        encryption type id (int)
    """

    encryption_type_id = {
        'aes-256': 1,
        'aes-192': 2,
        'aes-128': 3,
        'twofish-256': 4,
        'twofish-192': 5,
        'twofish-128': 6,
        'serpent-256': 7,
        'serpent-192': 8,
        'serpent-128': 9
    }

    if encryption_type not in encryption_type_id.keys():
        raise ValueError(f'Incorrect encryption_type "{encryption_type}')
    return encryption_type_id.get(encryption_type)
