from src.utils.db import SNPMDB, CryptoDB
from src.models import db



class Entry(db.Model, SNPMDB):

    __tablename__ = 'entries'

    id = db.Column('entry_id', db.Integer, primary_key=True)
    directory_id = db.Column(db.Integer, db.ForeignKey('directories.directory_id'), nullable=False)
    __name = db.Column('entry_name', db.LargeBinary, nullable=False)
    __username = db.Column('username', db.LargeBinary, nullable=False)
    __note = db.Column('note', db.LargeBinary, nullable=False)
    created_at = db.Column(db.DateTime, nullable=False)
    pass_lifetime = db.Column(db.Integer, nullable=False)
    deleted_at = db.Column(db.DateTime, nullable=True)
    deleted_by = db.Column(db.String, nullable=True)

    @property
    def name(self) -> str:
        return self.crypto.decrypt(self.__name)
    
    @name.setter
    def name(self, name :str) -> None:
        self.__name = self.crypto.encrypt(name)
    
    @property
    def username(self) -> str:
        return self.crypto.decrypt(self.__username)
    
    @username.setter
    def username(self, username :str) -> None:
        self.__username = self.crypto.encrypt(username)
    
    @property
    def note(self) -> str:
        return self.crypto.decrypt(self.__note)
    
    @note.setter
    def name(self, note :str) -> None:
        self.__note = self.crypto.encrypt(note)
