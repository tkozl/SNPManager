from datetime import datetime

from src.utils.db import SNPMDBView, CryptoDB
from src.models import db



class UserEntryView(db.Model, SNPMDBView):

    __tablename__ = 'users_entries'

    user_id = db.Column(db.Integer, primary_key=True)
    entry_id = db.Column(db.Integer, primary_key=True)
    directory_id = db.Column(db.Integer, db.ForeignKey('directories.directory_id'), primary_key=True)
    special_directory_id = db.Column(db.Integer, db.ForeignKey('special_directories.special_directory_id'), primary_key=True)
    __name = db.Column('entry_name', db.LargeBinary, primary_key=True)
    __username = db.Column('username', db.LargeBinary, primary_key=True)
    __note = db.Column('note', db.LargeBinary, primary_key=True)
    __created_at = db.Column('created_at', db.LargeBinary, primary_key=True)
    __pass_lifetime = db.Column('pass_lifetime', db.LargeBinary, primary_key=True)
    deleted_at = db.Column(db.DateTime, primary_key=True)
    __deleted_by = db.Column('deleted_by', db.LargeBinary, primary_key=True)
    __moved_at = db.Column('moved_at', db.LargeBinary, primary_key=True)

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
    def created_at(self) -> datetime:
        return datetime.strptime(self.crypto.decrypt(self.__created_at), '%Y-%m-%d %H:%M:%S')

    @property
    def pass_lifetime(self) -> int:
        return int(self.crypto.decrypt(self.__pass_lifetime))
    
    @property
    def deleted_by(self) -> str:
        return self.crypto.decrypt(self.__deleted_by)

    @property
    def moved_at(self) -> str:
        if self.__moved_at == None:
            return self.created_at
        else:
            return datetime.strptime(self.crypto.decrypt(self.__moved_at), '%Y-%m-%d %H:%M:%S')
