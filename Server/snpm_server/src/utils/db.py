class CryptoDB:

    def __init__(self, key :bytes) -> None:
        self.__key = key

    def encrypt(self, data :str) -> bytes:
        pass

    def decrypt(self, data :bytes) -> str:
        pass


class SNPMDB:

    @property
    def crypto(self) -> CryptoDB:
        return self.__crypto
    
    @crypto.setter
    def crypto(self, crypto :CryptoDB) -> None:
        self.__crypto = crypto
