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
        'camellia-256': 4,
        'camellia-192': 5,
        'camellia-128': 6,
        'serpent-256': 7,
        'serpent-192': 8,
        'serpent-128': 9
    }

    if encryption_type not in encryption_type_id.keys():
        raise ValueError(f'Incorrect encryption_type "{encryption_type}')
    return encryption_type_id.get(encryption_type)


def user_encryption_id_to_type(encryption_id :int) -> str:
    """
    Converts encryption id to string encryption name
    Args:
        encryption_id (int): id of encryption type
    Return:
        encryption type name (str)
    """

    encryption_type_name = [
        'aes-256',
        'aes-192',
        'aes-128',
        'camellia-256',
        'camellia-192',
        'camellia-128',
        'serpent-256',
        'serpent-192',
        'serpent-128'
    ]
    encryption_id = int(encryption_id)
    if encryption_id < 1 or encryption_id > len(encryption_type_name):
        raise ValueError(f'Incorrect encryption_id {encryption_id}')
    return encryption_type_name[encryption_id-1]


def is_dir_name_correct(directory_name :str) -> bool:
    """
    Checks if directory name is allowed
    Args:
        directory_name (str): checking directory name
    Return:
        bool
    """

    blacklist = ['/', '\\', ':', '*', '?', '"', '<', '>', '|']
    for char in directory_name:
        if char in blacklist:
            return False
    return True


def is_entry_name_correct(entry_name :str) -> bool:
    """
    Checks if entry name is allowed
    Args:
        entry_name (str): checking entry name
    Return:
        bool
    """

    return is_dir_name_correct(entry_name)
