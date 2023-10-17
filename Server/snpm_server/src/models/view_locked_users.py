from src.utils.db import SNPMDBView, CryptoDB
from src.models import db



class LockedUserView(db.Model, SNPMDBView):

    __tablename__ = 'locked_users'

    user_id = db.Column(db.Integer, primary_key=True)
