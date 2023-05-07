from src.utils.db import SNPMDB, CryptoDB
from src.models import db



class UserEntryView(db.Model, SNPMDB):

    __tablename__ = 'users_entries'

    user_id = db.Column(db.Integer, primary_key=True)
    entry_id = db.Column(db.Integer, nullable=False)
