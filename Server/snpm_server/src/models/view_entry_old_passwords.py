from src.utils.db import SNPMDB, CryptoDB
from src.models import db



class EntryOldPasswordView(db.Model, SNPMDB):

    __tablename__ = 'entry_old_password'

    id = db.Column('entry_id', db.Integer, primary_key=True)
    password_id = db.Column('pass_id', db.Integer, primary_key=True)
