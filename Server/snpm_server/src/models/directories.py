from src.utils.db import SNPMDB, CryptoDB
from src.models import db



class Directory(db.Model, SNPMDB):

    __tablename__ = 'directories'

    id = db.Column('directory_id', db.Integer, primary_key=True)
    parent_id = db.Column(db.Integer, db.ForeignKey('directories.directory_id'), nullable=False)
    special_directory_id = db.Column(db.Integer, db.ForeignKey('special_directories.special_directory_id'), nullable=False)
    __name = db.Column('directory_name', db.LargeBinary, nullable=False)
    deleted_at = db.Column(db.DateTime, nullable=True)
    deleted_by = db.Column(db.String, nullable=True)

    def __init__(self, direcotry_name :str, parent_id :int) -> None:
        pass

    @property
    def name(self) -> str:
        return self.crypto.decrypt(self.__name)
    
    @name.setter
    def name(self, name :str) -> None:
        self.__name = self.crypto.encrypt(name)
