from src.utils.db import SNPMDB, CryptoDB
from src.models import db



class User(db.Model, SNPMDB):

    __tablename__ = 'users'

    id = db.Column('user_id', db.Integer, primary_key=True)
    email = db.Column('email', db.String, nullable=False, unique=True)
    challenge_string = db.Column(db.String, nullable=False)
    created_at = db.Column(db.DateTime, nullable=False)
    deleted_at = db.Column(db.DateTime, nullable=True)
    email_verified = db.Column(db.Boolean, nullable=False)
    secret_2fa = db.Column(db.String, nullable=False)
    encryption_type_id = db.Column(db.Integer, db.ForeignKey('encryption.encryption_type_id'), nullable=True)
    email_verify_token = db.Column(db.String, nullable=True)
    email_verify_token_exp = db.Column(db.DateTime, nullable=True)
    user_del_token = db.Column(db.String, nullable=True)
    user_del_token_exp = db.Column(db.DateTime, nullable=True)

    def __init__(self, email :str, crypto :CryptoDB) -> None:
        pass

    def __repr__(self):
        return f'<User {self.id}>'
