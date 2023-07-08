from src.utils.db import SNPMDB, CryptoDB
from src.models import db



class CurrentPasswordView(db.Model, SNPMDB):

    __tablename__ = 'entry_current_password'

    id = db.Column('entry_id', db.Integer, primary_key=True)
    password_id = db.Column('pass_id', db.Integer, primary_key=True)
