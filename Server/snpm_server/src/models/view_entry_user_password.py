from src.utils.db import SNPMDB, CryptoDB
from src.models import db



class EntryUserPasswordView(db.Model, SNPMDB):

    __tablename__ = 'entry_user_password'

    __password = db.Column('password', db.LargeBinary, nullable=False)
    user_id = db.Column(db.Integer, primary_key=True)
    entry_id = db.Column(db.Integer, nullable=True)
    directory_id = db.Column(db.Integer, db.ForeignKey('directories.directory_id'), nullable=False)
    special_directory_id = db.Column(db.Integer, db.ForeignKey('special_directories.special_directory_id'), nullable=False)
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
    
    @property
    def username(self) -> str:
        return self.crypto.decrypt(self.__username)
    
    @property
    def note(self) -> str:
        return self.crypto.decrypt(self.__note)

    @property
    def created_at(self) -> str:
        return self.crypto.decrypt(self.__created_at)

    @property
    def pass_lifetime(self) -> int:
        return int(self.crypto.decrypt(self.__pass_lifetime))
    
    @property
    def deleted_by(self) -> str:
        return self.crypto.decrypt(self.__deleted_by)
    
    @property
    def password(self) -> str:
        return self.crypto.decrypt(self.__password)
