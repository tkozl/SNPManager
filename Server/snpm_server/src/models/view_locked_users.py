from src.utils.db import SNPMDB, CryptoDB
from src.models import db



class LockedUserView(db.Model, SNPMDB):

    __tablename__ = 'locked_users'

    user_id = db.Column(db.Integer, primary_key=True)
