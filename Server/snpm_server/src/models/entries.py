from src.utils.db import SNPMDB, CryptoDB
from src.models import db



class Entry(db.Model, SNPMDB):

    __tablename__ = 'entries'

    id = db.Column('entry_id', db.Integer, primary_key=True)
    directory_id = db.Column(db.Integer, db.ForeignKey('directories.directory_id'), nullable=False)
    __name = db.Column('entry_name', db.LargeBinary, nullable=False)
    __username = db.Column('username', db.LargeBinary, nullable=False)
    __note = db.Column('note', db.LargeBinary, nullable=False)
    __created_at = db.Column('created_at', db.LargeBinary, nullable=False)
    __pass_lifetime = db.Column('pass_lifetime', db.LargeBinary, nullable=False)
    deleted_at = db.Column(db.DateTime, nullable=True)
    __deleted_by = db.Column('deleted_by', db.LargeBinary, nullable=True)

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

    @property
    def created_at(self) -> str:
        return self.crypto.decrypt(self.__created_at)
    
    @created_at.setter
    def created_at(self, created_at :str) -> None:
        self.__created_at = self.crypto.encrypt(created_at)

    @property
    def pass_lifetime(self) -> int:
        return int(self.crypto.decrypt(self.__pass_lifetime))
    
    @pass_lifetime.setter
    def created_at(self, pass_lifetime :int) -> None:
        self.__pass_lifetime = self.crypto.encrypt(str(int(pass_lifetime)))
    
    @property
    def deleted_by(self) -> str:
        return self.crypto.decrypt(self.__deleted_by)
    
    @deleted_by.setter
    def deleted_by(self, deleted_by :str) -> None:
        self.__deleted_by = self.crypto.encrypt(deleted_by)
