from Crypto.Protocol.KDF import PBKDF2
from Crypto.Hash import SHA512
from Crypto.Cipher import AES
from Crypto.Util.Padding import pad, unpad
from cryptography.hazmat.primitives.ciphers import Cipher, algorithms, modes
from abc import abstractmethod



class NoKeyError(Exception):
    """Cryptographic key required for function not provided"""
    pass


class CryptoDB:
    """Cryptographic aspects of SQL Database in SNPM Server"""

    # Supported cryptographic algorithms
    AES256 = 1
    AES192 = 2
    AES128 = 3
    CAMELLIA256 = 4
    CAMELLIA192 = 5
    CAMELLIA128 = 6
    SERPENT256 = 7
    SERPENT192 = 8
    SERPENT128 = 9

    def __init__(self, algorithm_id :int, key :bytes=None, iv :bytes=None) -> None:

        # Expected lengths of key and iv for each supported algorithms
        self.__expected_len = {
            str(self.AES256): (256, 128),
            str(self.AES192): (192, 128),
            str(self.AES128): (128, 128),
            str(self.CAMELLIA256): (256, 128),
            str(self.CAMELLIA192): (192, 128),
            str(self.CAMELLIA128): (128, 128),
            str(self.SERPENT256): (256, 128),
            str(self.SERPENT192): (192, 128),
            str(self.SERPENT128): (128, 128),
        }

        # Validating arguments
        if key != None and iv == None or iv != None and key == None:
            raise ValueError(f'If you want to provide a cryptographic key, you must also provide iv')
        if str(algorithm_id) not in self.__expected_len.keys():
            raise ValueError(f'Unknown algorithm id "{algorithm_id}"')
        if key != None:
            if len(key) != self.__expected_len.get(str(algorithm_id))[0] or len(iv) != self.__expected_len.get(str(algorithm_id))[1]:
                raise ValueError(f'{self.__expected_len.get(str(algorithm_id))[0]}-bit key and {self.__expected_len.get(str(algorithm_id))[1]}-bit iv are expected for the chosen algorithm')

        # Assigning values
        self.__algorithm_id = algorithm_id
        self.__key = key
        self.__iv = iv

    def encrypt(self, data :str) -> bytes:
        """
        Encrypt data using the loaded cryptographic parameters.
        Args:
            data (str): data to encrypt
        Return:
            encrypted data (bytes)
        """

        if self.__key == None:
            raise NoKeyError(f'First generate the cryptographic key')

        if self.__algorithm_id in (CryptoDB.AES256, CryptoDB.AES192, CryptoDB.AES128):
            cipher = AES.new(self.__key, AES.MODE_CBC, iv=self.__iv)
            ciphertext = cipher.encrypt(pad(bytes(data, 'utf-8'), AES.block_size))

        elif self.__algorithm_id in (CryptoDB.CAMELLIA256, CryptoDB.CAMELLIA192, CryptoDB.CAMELLIA128):
            cipher = Cipher(algorithms.Camellia(self.__key), modes.CBC(self.__iv))
            encryptor = cipher.encryptor()
            ciphertext = encryptor.update(pad(bytes(data, 'utf-8'), algorithms.Camellia.block_size)) + encryptor.finalize()

        return ciphertext

    def decrypt(self, ciphertext :bytes) -> str:
        """
        Decrypt data using the loaded cryptographic parameters.
        Args:
            data (str): data to encrypt
        Return:
            decrypted data (bytes)
        """

        if self.__key == None:
            raise NoKeyError(f'First generate the cryptographic key')

        if self.__algorithm_id in (CryptoDB.AES256, CryptoDB.AES192, CryptoDB.AES128):
            cipher = AES.new(self.__key, AES.MODE_CBC, iv=self.__iv)
            plaintext = unpad(cipher.decrypt(ciphertext), AES.block_size)

        elif self.__algorithm_id in (CryptoDB.CAMELLIA256, CryptoDB.CAMELLIA192, CryptoDB.CAMELLIA128):
            cipher = Cipher(algorithms.Camellia(self.__key), modes.CBC(self.__iv))
            decryptor = cipher.decryptor()
            plaintext = unpad(decryptor.update(ciphertext), algorithms.Camellia.block_size) + decryptor.finalize()

        return plaintext.decode('utf-8')

    def create_key(self, password :str, email :str) -> tuple[bytes, bytes]:
        """
        Generates a key using the kdf algorithm based on sha512 and plugs it into the object.
        Args:
            password (str): password which will be converted to a cryptographic key
            email (str): email address that will be used to increase key security
        Return:
            crypto key (bytes), iv (bytes)
        """

        key_len = int(self.__expected_len.get(str(self.__algorithm_id))[0] / 8) # key length in bytes
        iv_len = int(self.__expected_len.get(str(self.__algorithm_id))[1] / 8) # iv length in bytes

        username = email.split('@')[0]
        if len(username) <= 2:
            username = email
        if len(username) <= 2:
            raise ValueError('Email address is too short')
        if len(password) <= 4:
            raise ValueError('Password is too short')

        salt = bytes(username, 'utf-8') # users with identical passwords will have different keys
        kdf_res = PBKDF2(password, salt, key_len+iv_len, count=1000, hmac_hash_module=SHA512)
        self.__iv = kdf_res[:iv_len]
        self.__key = kdf_res[iv_len:]

        return (self.__key, self.__iv)


class SNPMDBView:
    """Plugs cryptography into db views subclasses"""

    @property
    def crypto(self) -> CryptoDB:
        try:
            return self.__crypto
        except AttributeError:
            raise AttributeError('Cannot do it without crypto')

    @crypto.setter
    def crypto(self, crypto :CryptoDB) -> None:
        self.__crypto = crypto


class SNPMDB(SNPMDBView):
    """Plugs cryptography into db views subclasses"""

    @abstractmethod
    def change_crypto(self, new_crypto :CryptoDB) -> None:
        """Encrypts table with new crypto"""
        pass
