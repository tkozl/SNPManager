from src.utils.db import SNPMDBView, CryptoDB
from src.models import db



class UnlockedUserView(db.Model, SNPMDBView):

    __tablename__ = 'unlocked_users'

    user_id = db.Column(db.Integer, primary_key=True)
