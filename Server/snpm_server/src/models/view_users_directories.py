from src.utils.db import SNPMDB, CryptoDB
from src.models import db



class UserDirectoryView(db.Model, SNPMDB):

    __tablename__ = 'users_directories'

    user_id = db.Column(db.Integer, primary_key=True)
    directory_id = db.Column(db.Integer, nullable=False)
    parent_id = db.Column(db.Integer, db.ForeignKey('directories.directory_id'), nullable=False)
    special_directory_id = db.Column(db.Integer, db.ForeignKey('special_directories.special_directory_id'), nullable=False)
    __name = db.Column('directory_name', db.LargeBinary, nullable=False)
    deleted_at = db.Column(db.DateTime, nullable=True)
    deleted_by = db.Column(db.String, nullable=True)

    def __repr__(self):
        return f'<User {self.user_id}, directory {self.directory_id}>'

    @property
    def name(self) -> str:
        return self.crypto.decrypt(self.__name)
