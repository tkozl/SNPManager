from src.utils.db import SNPMDBView, CryptoDB
from src.models import db



class UserIncorrectLogins24hView(db.Model, SNPMDBView):

    __tablename__ = 'incorrect_logins_24h'

    user_id = db.Column(db.Integer, primary_key=True)
    incorrect_logins_quantity = db.Column(db.Integer, primary_key=True)
