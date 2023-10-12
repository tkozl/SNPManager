from src.utils.db import SNPMDB, CryptoDB
from src.models import db



class Encryption(db.Model, SNPMDB):

    __tablename__ = 'encryption'

    id = db.Column('encryption_type_id', db.Integer, primary_key=True)
    encryption_name = db.Column(db.String, nullable=False)

    def change_crypto(self, new_crypto :CryptoDB) -> None:
        """Encrypts table with new crypto"""
        self.crypto = new_crypto
