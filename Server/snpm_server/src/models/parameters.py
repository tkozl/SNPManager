from src.utils.db import SNPMDB, CryptoDB
from src.models import db



class EntryParameter(db.Model, SNPMDB):

    __tablename__ = 'parameters'

    id = db.Column('parameter_id', db.Integer, primary_key=True)
    entry_id = db.Column(db.Integer, db.ForeignKey('entries.entry_id'), nullable=False)
    __name = db.Column('parameter_name', db.LargeBinary, nullable=False)
    __value = db.Column('parameter_value', db.LargeBinary, nullable=False)
    deleted_at = db.Column(db.DateTime, nullable=True)
    __deleted_by = db.Column('deleted_by', db.LargeBinary, nullable=True)

    @property
    def name(self) -> str:
        return self.crypto.decrypt(self.__name)
    
    @name.setter
    def name(self, name :str) -> None:
        self.__name = self.crypto.encrypt(name)

    @property
    def value(self) -> str:
        return self.crypto.decrypt(self.__value)
    
    @value.setter
    def value(self, value :str) -> None:
        self.__value = self.crypto.encrypt(value)

    @property
    def deleted_by(self) -> str:
        return self.crypto.decrypt(self.__deleted_by)
    
    @deleted_by.setter
    def deleted_by(self, deleted_by :str) -> None:
        self.__deleted_by = self.crypto.encrypt(deleted_by)
