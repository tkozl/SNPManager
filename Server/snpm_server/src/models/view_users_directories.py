from src.utils.db import SNPMDB, CryptoDB
from src.models import db



class UserDirectoryView(db.Model, SNPMDB):

    __tablename__ = 'users_directories'

    user_id = db.Column(db.Integer, primary_key=True)
    directory_id = db.Column(db.Integer, nullable=False)
