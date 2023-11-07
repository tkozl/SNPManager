from datetime import datetime
from sqlalchemy.sql import null

from src.utils.db import SNPMDB, CryptoDB
from src.models import db
from src.models.view_users_entries import UserEntryView
from src.models.errors import EntryAlreadyExists



class Entry(db.Model, SNPMDB):

    __tablename__ = 'entries'

    id = db.Column('entry_id', db.Integer, primary_key=True)
    directory_id = db.Column(db.Integer, db.ForeignKey('directories.directory_id'), nullable=True)
    special_directory_id = db.Column(db.Integer, db.ForeignKey('special_directories.special_directory_id'), nullable=False)
    __name = db.Column('entry_name', db.LargeBinary, nullable=False)
    __username = db.Column('username', db.LargeBinary, nullable=False)
    __note = db.Column('note', db.LargeBinary, nullable=False)
    __created_at = db.Column('created_at', db.LargeBinary, nullable=False)
    __pass_lifetime = db.Column('pass_lifetime', db.LargeBinary, nullable=False)
    deleted_at = db.Column(db.DateTime, nullable=True)
    __deleted_by = db.Column('deleted_by', db.LargeBinary, nullable=True)
    __moved_at = db.Column('moved_at', db.LargeBinary, nullable=True)

    def __init__(self, crypto :CryptoDB, user_id :int, directory_id :int, special_directory_id :int, name :str, username :str, note :str, pass_lifetime :str) -> None:
        self.crypto = crypto
        directory_entries = UserEntryView.query.filter_by(directory_id=directory_id, user_id=user_id, deleted_at=None).all()
        for directory_entry in directory_entries:
            directory_entry.crypto = crypto
            if directory_entry.name == name:
                raise EntryAlreadyExists(f'Entry with name "{name}" already exists in directory {directory_id}')

        if directory_id == None:
            self.directory_id = null()
        else:
            self.directory_id = directory_id
        self.special_directory_id = special_directory_id
        self.name = name
        self.username = username
        self.note = note
        self.created_at = datetime.now()
        self.pass_lifetime = pass_lifetime
        self.__deleted_at = null()
        self.__deleted_by = null()
        self.moved_at = self.created_at

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
    def note(self, note :str) -> None:
        self.__note = self.crypto.encrypt(note)

    @property
    def created_at(self) -> datetime:
        return datetime.strptime(self.crypto.decrypt(self.__created_at), '%Y-%m-%d %H:%M:%S')

    @created_at.setter
    def created_at(self, created_at :datetime) -> None:
        self.__created_at = self.crypto.encrypt(created_at.strftime('%Y-%m-%d %H:%M:%S'))

    @property
    def pass_lifetime(self) -> int:
        return int(self.crypto.decrypt(self.__pass_lifetime))

    @pass_lifetime.setter
    def pass_lifetime(self, pass_lifetime :int) -> None:
        self.__pass_lifetime = self.crypto.encrypt(str(int(pass_lifetime)))

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
            return self.created_at
        else:
            return datetime.strptime(self.crypto.decrypt(self.__moved_at), '%Y-%m-%d %H:%M:%S')

    @moved_at.setter
    def moved_at(self, moved_at :datetime) -> None:
        if moved_at == None:
            moved_at = self.created_at
        self.__moved_at = self.crypto.encrypt(moved_at.strftime('%Y-%m-%d %H:%M:%S'))

    def delete(self) -> None:
        """Deletes item"""
        self.__name = b''
        self.__username = b''
        self.__note = b''
        self.__pass_lifetime = b''
        self.deleted_at = datetime.now()

    def change_crypto(self, new_crypto :CryptoDB) -> None:
        """Encrypts table with new crypto"""
        entry_name = self.name
        username = self.username
        note = self.note
        created_at = self.created_at
        pass_lifetime = self.pass_lifetime
        deleted_by = self.deleted_by
        deleted_by = self.deleted_by
        moved_at = self.moved_at

        self.crypto = new_crypto
        self.name = entry_name
        self.username = username
        self.note = note
        self.created_at = created_at
        self.pass_lifetime = pass_lifetime
        self.deleted_by = deleted_by
        self.moved_at = moved_at

    def move(self, directory_id :int, special_directory_id :int, entry_name :str) -> None:
        """
        Moves entry
        Args:
            directory_id (int): new directory id
            special_directory_id (int): new special directory id
            entry_name (str): new entry name
        """

        self.special_directory_id = special_directory_id

        if directory_id != self.directory_id:
            directory_entries = Entry.query.filter_by(directory_id=directory_id, special_directory_id=special_directory_id).all()
            for directory_entry in directory_entries:
                directory_entry.crypto = self.crypto
                if directory_entry.name == entry_name:
                    raise EntryAlreadyExists(f'Entry with name "{entry_name}" already exists in directory {directory_id}')

            if directory_id == None:
                self.directory_id = null()
            else:
                self.directory_id = directory_id

        self.name = entry_name

        self.moved_at = datetime.now()
