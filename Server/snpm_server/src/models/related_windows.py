from sqlalchemy.sql import null
from datetime import datetime

from src.utils.db import SNPMDB, CryptoDB
from src.models import db
from src.models.errors import EntryRelatedWindowAlreadyExists



class RelatedWindow(db.Model, SNPMDB):

    __tablename__ = 'related_windows'

    id = db.Column('window_id', db.Integer, primary_key=True)
    entry_id = db.Column(db.Integer, db.ForeignKey('entries.entry_id'), nullable=False)
    __name = db.Column('window_name', db.LargeBinary, nullable=False)
    deleted_at = db.Column(db.DateTime, nullable=True)
    __deleted_by = db.Column('deleted_by', db.LargeBinary, nullable=True)

    def __init__(self, crypto :CryptoDB, entry_id :int, related_window_name :str) -> None:
        self.crypto = crypto
        exists_windows = RelatedWindow.query.filter_by(entry_id=entry_id, deleted_at=None)
        for exists_window in exists_windows:
            exists_window.crypto = crypto
            if exists_window.name == related_window_name:
                raise EntryRelatedWindowAlreadyExists(f'Related window with name "{related_window_name}" already exists in entry {entry_id}')
        self.entry_id = entry_id
        self.name = related_window_name
        self.deleted_at = null()
        self.__deleted_by = null()

    def change_crypto(self, new_crypto :CryptoDB) -> None:
        """Encrypts table with new crypto"""
        name = self.name
        deleted_by = self.deleted_by

        self.crypto = new_crypto
        self.name = name
        self.deleted_by = deleted_by

    @property
    def name(self) -> str:
        return self.crypto.decrypt(self.__name)

    @name.setter
    def name(self, name :str) -> None:
        self.__name = self.crypto.encrypt(name)

    @property
    def deleted_by(self) -> str:
        return self.crypto.decrypt(self.__deleted_by)

    @deleted_by.setter
    def deleted_by(self, deleted_by :str) -> None:
        self.__deleted_by = self.crypto.encrypt(deleted_by)

    def delete(self, ip :str) -> None:
        """Deletes entry related window"""
        self.deleted_at = datetime.now()
        self.deleted_by = ip
