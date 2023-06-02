from Crypto.Hash import SHA512
from datetime import datetime
from sqlalchemy.sql import null

from src.utils.db import SNPMDB, CryptoDB
from src.models import db
from src.models.view_locked_users import LockedUserView
import src.models.errors as e



class User(db.Model, SNPMDB):

    __tablename__ = 'users'

    id = db.Column('user_id', db.Integer, primary_key=True)
    email_hash = db.Column(db.LargeBinary, nullable=False, unique=True)
    encrypted_email = db.Column(db.LargeBinary, nullable=False)
    created_at = db.Column(db.DateTime, nullable=False)
    deleted_at = db.Column(db.DateTime, nullable=True)
    email_verified = db.Column(db.Boolean, nullable=False)
    secret_2fa = db.Column(db.String, nullable=True)
    encryption_type_id = db.Column(db.Integer, db.ForeignKey('encryption.encryption_type_id'), nullable=False)
    email_verify_token = db.Column(db.String, nullable=True)
    email_verify_token_exp = db.Column(db.DateTime, nullable=True)
    user_del_token = db.Column(db.String, nullable=True)
    user_del_token_exp = db.Column(db.DateTime, nullable=True)

    def __init__(self, email :str, password :str, algorithm_type_id :int) -> None:
        """
        Creates new user account
        Args:
            email (str): user email address
            password (str): user password
            algorithm_type_id (int): id of crypto algorithm
        Possible errors:
            EmailCurrentlyInUse: raises when another account has the same email address as provided
        """

        email_hash = SHA512.new(data=bytes(email, 'utf-8'))
        conflict = User.query.filter_by(email_hash=email_hash.digest()).first()
        if conflict != None:
            raise e.EmailCurrentlyInUseError(f'Email address {email} is currently in use')
        
        self.crypto = CryptoDB(algorithm_type_id)
        self.crypto.create_key(password, email)

        self.encrypted_email = self.crypto.encrypt(email)
        self.email_hash = email_hash.digest()
        
        self.encryption_type_id = algorithm_type_id
        self.email_verified = False
        self.created_at = datetime.now()
        self.secret_2fa = null()
        self.email_verify_token = null()
        self.user_del_token = null()

    def __repr__(self):
        return f'<User {self.id}>'

    @property
    def is_locked(self) -> bool:
        """Is user locked (bool)"""
        locked = LockedUserView.query.filter_by(id=self.user_id).first()
        if locked == None:
            return False
        else:
            return True
