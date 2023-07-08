from src.utils.db import SNPMDB, CryptoDB
from src.models import db



class UserLastIncorrectLoginView(db.Model, SNPMDB):

    __tablename__ = 'last_incorrect_logins'

    user_id = db.Column(db.Integer, primary_key=True)
    activity_id = db.Column(db.Integer, primary_key=True)
    occured_at = db.Column(db.DateTime, primary_key=True)
