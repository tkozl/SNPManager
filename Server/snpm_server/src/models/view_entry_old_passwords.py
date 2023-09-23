from src.utils.db import SNPMDB, CryptoDB
from src.models import db



class EntryOldPasswordView(db.Model, SNPMDB):

    __tablename__ = 'entry_old_passwords'

    id = db.Column('entry_id', db.Integer, primary_key=True)
    password_id = db.Column('pass_id', db.Integer, primary_key=True)
    __value = db.Column('password', db.LargeBinary, primary_key=True)
    created_at = db.Column(db.DateTime, primary_key=True)

    @property
    def value(self) -> str:
        return self.crypto.decrypt(self.__value)
    
    @value.setter
    def value(self, value :str) -> None:
        self.__value = self.crypto.encrypt(value)
