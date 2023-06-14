from Crypto.Hash import SHA512
from datetime import datetime
from sqlalchemy.sql import null

from src.utils.db import SNPMDB, CryptoDB
from src.schemas.entry import EntryJSON
from src.models import db
from src.models.view_locked_users import LockedUserView
from src.models.special_directories import SpecialDir
from src.models.view_users_directories import UserDirectoryView
from src.models.view_entry_user_password import EntryUserPasswordView
from src.models.parameters import EntryParameter
from src.models.related_windows import RelatedWindow
from src.models.directories import Directory
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
        locked = LockedUserView.query.filter_by(user_id=self.id).first()
        if locked == None:
            return False
        else:
            return True

    @property
    def root_dir_id(self) -> int:
        """Id of root directory"""
        root_directory = SpecialDir.query.filter_by(user_id=self.id, special_directory_type_id=SpecialDir.ROOT_ID).first()
        if root_directory == None:
            raise e.ModelError(f'Not found root directory for user {self.id}')
        return root_directory.id
    
    @property
    def trash_id(self) -> int:
        """Id of trash directory"""
        trash_directory = SpecialDir.query.filter_by(user_id=self.id, special_directory_type_id=SpecialDir.TRASH_ID).first()
        if trash_directory == None:
            raise e.ModelError(f'Not found trash for user {self.id}')
        return trash_directory.id

    def get_directories(self, parent_id :int=None, recursive :bool=True) -> list[dict]:
        """
        Gets list of directory in specified parent directory
        Args:
            parent_id (int): id of parent directory, defaults: None
            recursive (bool): is True, then returns all subdirectories (defaults: True)
        Return:
            list of directories dictionaries:
            {
                'directory_id': int,
                'special_directory_id': int,
                'parent_id': int,
                'directory_name': str
            }
        """

        db.session.expire_all()
        directories = UserDirectoryView.query.filter_by(user_id=self.id, parent_id=parent_id).all()
        res = []
        for directory in directories:
            directory.crypto = self.crypto
            if directory.deleted_at != None:
                continue
            res.append({
                'directory_id': directory.directory_id,
                'special_directory_id': directory.special_directory_id,
                'parent_id': directory.parent_id,
                'directory_name': directory.name
            })
            if recursive:
                res += self.get_directories(directory.directory_id)
        return res

    def get_entries(self, parent_dir_id :int=None, recursive :bool=True, append_parameters :bool=False, append_related_windows :bool=False) -> list[EntryJSON]:
        """
        Gets list of user entries
        Args:
            parent_dir_id (int): id of the parent directory (defaults: None)
            recursive (bool): if True, then returns entries recurrently
            append_parameters (bool): if True, then appends enties parameters to the result (defaults: False)
            append_related_windows (bool): if True, then appends enties related windows to the result (defaults: False)
        Return:
            list of EntryJSON objects
        """

        db.session.expire_all()
        entries = EntryUserPasswordView.query.filter_by(user_id=self.id, directory_id=parent_dir_id).all()
        res = []
        for entry in entries:
            entry.crypto = self.crypto
            if entry.deleted_at != None:
                continue
            entry_json = EntryJSON()
            entry_json.entry_id = entry.entry_id
            entry_json.directory_id = entry.directory_id
            entry_json.entry_name = entry.name
            entry_json.lifetime = entry.pass_lifetime
            entry_json.note = entry.note
            entry_json.username = entry.username
            entry_json.password = entry.password

            if append_parameters:
                parameters = EntryParameter.query.filter_by(entry_id=self.id).all()
                parameters_list = []
                for parameter in parameters:
                    parameter.crypto = self.crypto
                    if parameter.deleted_at != None:
                        continue
                    parameters_list.append({
                        'name': parameter.name,
                        'value': parameter.value
                    })
                entry_json.parameters = parameters_list
            
            if append_related_windows:
                related_windows = RelatedWindow.query.filter_by(entry_id=self.id).all()
                related_windows_list = []
                for related_window in related_windows:
                    related_window.crypto = self.crypto
                    if related_window.deleted_at != None:
                        continue
                    related_windows_list.append(related_window.name)
                entry_json.related_windows = related_windows_list
            
            res.append(entry_json)
        
        if recursive:
            directories = UserDirectoryView.query.filter_by(user_id=self.id, parent_id=parent_dir_id).all()
            for directory in directories:
                directory.crypto = self.crypto
                res += self.get_entries(directory.directory_id, recursive, append_parameters, append_related_windows)
        
        return res
