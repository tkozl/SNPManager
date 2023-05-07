from src.utils.db import SNPMDB, CryptoDB
from src.models import db



class ActivityLog(db.Model, SNPMDB):

    __tablename__ = 'activity_log'

    id = db.Column('activity_id', db.Integer, primary_key=True)
    user_id = db.Column(db.Integer, db.ForeignKey('user.user_id'), nullable=False)
    activity_type_id = db.Column(db.Integer, nullable=False)
    ocurred_at = db.Column(db.DateTime, nullable=False)
    ip = db.Column(db.String, nullable=False)