from src.utils.db import SNPMDBView, CryptoDB
from src.models import db



class UserDirectoryView(db.Model, SNPMDBView):

    __tablename__ = 'users_directories'

    user_id = db.Column(db.Integer, primary_key=True)
    directory_id = db.Column(db.Integer, primary_key=True)
    parent_id = db.Column(db.Integer, db.ForeignKey('directories.directory_id'), primary_key=True)
    special_directory_id = db.Column(db.Integer, db.ForeignKey('special_directories.special_directory_id'), primary_key=True)
    __name = db.Column('directory_name', db.LargeBinary, primary_key=True)
    deleted_at = db.Column(db.DateTime, primary_key=True)
    deleted_by = db.Column(db.String, primary_key=True)

    def __repr__(self):
        return f'<User {self.user_id}, directory {self.directory_id}>'

    @property
    def name(self) -> str:
        return self.crypto.decrypt(self.__name)
