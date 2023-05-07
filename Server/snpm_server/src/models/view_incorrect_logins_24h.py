from src.utils.db import SNPMDB, CryptoDB
from src.models import db



class UserIncorrectLogins24hView(db.Model, SNPMDB):

    __tablename__ = 'incorrect_logins_24h'

    user_id = db.Column(db.Integer, primary_key=True)
    incorrect_logins_quantity = db.Column(db.Integer, nullable=False)
