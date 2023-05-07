from src.utils.db import SNPMDB, CryptoDB
from src.models import db



class SpecialDir(db.Model, SNPMDB):

    __tablename__ = 'special_directories'

    id = db.Column('special_directory_id', db.Integer, primary_key=True)
    user_id = db.Column(db.Integer, db.ForeignKey('users.user_id'), nullable=False)
    special_directory_type_id = db.Column(db.Integer, nullable=False)
