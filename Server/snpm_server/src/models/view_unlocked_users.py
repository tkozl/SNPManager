from src.utils.db import SNPMDB, CryptoDB
from src.models import db



class UnlockedUserView(db.Model, SNPMDB):

    __tablename__ = 'unlocked_users'

    user_id = db.Column(db.Integer, primary_key=True)
