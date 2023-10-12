from src.utils.db import SNPMDB, CryptoDB
from src.models import db



class SpecialDir(db.Model, SNPMDB):

    __tablename__ = 'special_directories'

    ROOT_ID = 1
    TRASH_ID = 2

    id = db.Column('special_directory_id', db.Integer, primary_key=True)
    user_id = db.Column(db.Integer, db.ForeignKey('users.user_id'), nullable=False)
    special_directory_type_id = db.Column(db.Integer, nullable=False)

    def __init__(self, user_id :int, special_directory_type_id :int) -> None:
        self.special_directory_type_id = special_directory_type_id
        self.user_id = user_id

    def change_crypto(self, new_crypto :CryptoDB) -> None:
        """Encrypts table with new crypto"""
        self.crypto = new_crypto
