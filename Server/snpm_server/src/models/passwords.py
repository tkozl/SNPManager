from src.utils.db import SNPMDB, CryptoDB
from src.models import db



class Password(db.Model, SNPMDB):

    __tablename__ = 'passwords'

    id = db.Column('pass_id', db.Integer, primary_key=True)
    entry_id = db.Column(db.Integer, db.ForeignKey('entries.entry_id'), nullable=False)
    __value = db.Column('password', db.LargeBinary, nullable=False)
    created_at = db.Column(db.DateTime, nullable=False)

    @property
    def value(self) -> str:
        return self.crypto.decrypt(self.__value)
    
    @value.setter
    def value(self, value :str) -> None:
        self.__value = self.crypto.encrypt(value)
