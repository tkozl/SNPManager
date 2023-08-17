from datetime import datetime

from src.utils.db import SNPMDB, CryptoDB
from src.models import db
from src.models.errors import EntryParameterAlreadyExists



class EntryParameter(db.Model, SNPMDB):

    __tablename__ = 'parameters'

    id = db.Column('parameter_id', db.Integer, primary_key=True)
    entry_id = db.Column(db.Integer, db.ForeignKey('entries.entry_id'), nullable=False)
    __name = db.Column('parameter_name', db.LargeBinary, nullable=False)
    __value = db.Column('parameter_value', db.LargeBinary, nullable=False)
    deleted_at = db.Column(db.DateTime, nullable=True)
    __deleted_by = db.Column('deleted_by', db.LargeBinary, nullable=True)

    def __init__(self, crypto :CryptoDB, entry_id :int, parameter_name :str, parameter_value :str) -> None:
        self.crypto = crypto

        entry_parameters = EntryParameter.query.filter_by(entry_id=entry_id, deleted_at=None).all()
        for entry_parameter in entry_parameters:
            entry_parameter.crypto = crypto
            if entry_parameter.name == parameter_name:
                raise EntryParameterAlreadyExists(f'Parameter with name "{parameter_name}" already exists in entry {entry_id}')

        self.entry_id = entry_id
        self.name = parameter_name
        self.value = parameter_value

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

    def delete(self, ip :str) -> None:
        """Deletes entry parameter"""
        self.deleted_at = datetime.now()
        self.deleted_by = ip
