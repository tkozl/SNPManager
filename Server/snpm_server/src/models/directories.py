from sqlalchemy.sql import null
from datetime import datetime

from src.utils.db import SNPMDB, CryptoDB
from src.models import db
from src.models.errors import DirectoryAlreadyExists



class Directory(db.Model, SNPMDB):

    __tablename__ = 'directories'

    id = db.Column('directory_id', db.Integer, primary_key=True)
    parent_id = db.Column(db.Integer, db.ForeignKey('directories.directory_id'), nullable=False)
    special_directory_id = db.Column(db.Integer, db.ForeignKey('special_directories.special_directory_id'), nullable=False)
    __name = db.Column('directory_name', db.LargeBinary, nullable=False)
    deleted_at = db.Column(db.DateTime, nullable=True)
    __deleted_by = db.Column('deleted_by', db.LargeBinary, nullable=True)
    __moved_at = db.Column('moved_at', db.LargeBinary, nullable=True)

    def __repr__(self):
        return f'<Directory {self.id}>'

    def __init__(self, crypto :CryptoDB, directory_name :str, parent_id :int, special_directory_id :int) -> None:
        self.crypto = crypto

        # Checking if directory doesn't exist
        conflict = Directory.query.filter_by(name=directory_name, parent_id=parent_id, special_directory_id=special_directory_id, deleted_at=None).first()
        if conflict != None:
            raise DirectoryAlreadyExists(f'Directory with name "{directory_name}" already exists in directory with id {parent_id}')

        self.name = directory_name
        self.parent_id = parent_id
        self.special_directory_id = special_directory_id
        self.__deleted_at = null()
        self.__deleted_by = null()
        self.moved_at = datetime.now()

    def change_crypto(self, new_crypto :CryptoDB) -> None:
        """Encrypts table with new crypto"""
        directory_name = self.name
        deleted_by = self.deleted_by
        moved_at = self.moved_at
        self.crypto = new_crypto
        self.name = directory_name
        self.deleted_by = deleted_by
        self.moved_at = moved_at

    def delete(self, user_ip :str=None) -> None:
        """Deletes directory"""
        self.__name = b''
        self.deleted_at = datetime.now()
        if user_ip != None:
            self.deleted_by = user_ip

    @property
    def name(self) -> str:
        return self.crypto.decrypt(self.__name)

    @name.setter
    def name(self, name :str) -> None:
        self.__name = self.crypto.encrypt(name)

    @property
    def deleted_by(self) -> str:
        if self.__deleted_by == None:
            return None
        else:
            return self.crypto.decrypt(self.__deleted_by)

    @deleted_by.setter
    def deleted_by(self, deleted_by :str) -> None:
        if deleted_by == None:
            self.__deleted_by = null()
        else:
            self.__deleted_by = self.crypto.encrypt(deleted_by)

    @property
    def moved_at(self) -> str:
        if self.__moved_at == None:
            return datetime.now()
        else:
            return datetime.strptime(self.crypto.decrypt(self.__moved_at), '%Y-%m-%d %H:%M:%S')

    @moved_at.setter
    def moved_at(self, moved_at :datetime) -> None:
        if moved_at == None:
            moved_at = datetime.now()
        self.__moved_at = self.crypto.encrypt(moved_at.strftime('%Y-%m-%d %H:%M:%S'))
