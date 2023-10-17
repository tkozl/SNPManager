from Crypto.Hash import SHA512
from datetime import datetime
from time import mktime
from sqlalchemy.sql import null

from config import Config
from src.utils.db import SNPMDB, CryptoDB
from src.utils.mail import Mail
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
from src.schemas.api_special_dir import ApiSpecialDir



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

        email_hash = self.hash_email(email)
        conflict = User.query.filter_by(email_hash=email_hash).first()
        if conflict != None:
            raise e.EmailCurrentlyInUseError(f'Email address {email} is currently in use')

        self.crypto = CryptoDB(algorithm_type_id)
        self.crypto.create_key(password, email)

        self.encrypted_email = self.crypto.encrypt(email)
        self.email_hash = email_hash

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

    def hash_email(self, plain_email :str) -> bytes:
        """
        Args:
            plain_email (str): user email address
        Return:
            Email hash (bytes)
        """
        email_hash = SHA512.new(data=bytes(plain_email, 'utf-8'))
        return email_hash.digest()

    def delete(self) -> None:
        """Deletes item"""
        self.email_hash = b''
        self.encrypted_email = b''
        self.secret_2fa = None
        self.email_verify_token = None
        self.email_verify_token_exp = None
        self.deleted_at = datetime.now()

    def change_crypto(self, new_crypto :CryptoDB) -> None:
        """Encrypts table with new crypto"""
        user_email = self.crypto.decrypt(self.encrypted_email)
        self.crypto = new_crypto
        self.encrypted_email = self.crypto.encrypt(user_email)

    def get_directories(self, parent_id :int=None, recursive :bool=True, trash :bool=False) -> list[dict]:
        """
        Gets list of directory in specified parent directory
        Args:
            parent_id (int): id of parent directory, defaults: None
            recursive (bool): if True, then returns all subdirectories (defaults: True)
            trash (bool): if True, then searchs only in trash (defaults: False)
        Return:
            list of directories dictionaries:
            {
                'directory_id': int,
                'special_directory_id': int,
                'parent_id': int,
                'directory_name': str
            }
        """

        if trash:
            special_dir = self.trash_id
        else:
            special_dir = self.root_dir_id

        directories = UserDirectoryView.query.filter_by(user_id=self.id, parent_id=parent_id, special_directory_id=special_dir).all()
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
                res += self.get_directories(directory.directory_id, trash=trash)
        return res

    def get_entries(self, parent_dir_id :int=None, recursive :bool=True, append_parameters :bool=False, append_related_windows :bool=False, trash :bool=False) -> list[EntryJSON]:
        """
        Gets list of user entries
        Args:
            parent_dir_id (int): id of the parent directory (defaults: None)
            recursive (bool): if True, then returns entries recurrently
            append_parameters (bool): if True, then appends enties parameters to the result (defaults: False)
            append_related_windows (bool): if True, then appends enties related windows to the result (defaults: False)
            trash (bool): if True, then searchs only in trash (defaults: False)
        Return:
            list of EntryJSON objects
        """

        if trash:
            special_dir = self.trash_id
        else:
            special_dir = self.root_dir_id

        entries = EntryUserPasswordView.query.filter_by(user_id=self.id, directory_id=parent_dir_id, special_directory_id=special_dir).all()
        res = []
        for entry in entries:
            entry.crypto = self.crypto
            if entry.deleted_at != None:
                continue
            entry_json = EntryJSON()
            entry_json.entry_id = entry.entry_id
            entry_json.directory_id = entry.directory_id
            if entry_json.directory_id == None:
                if trash:
                    entry_json.directory_id = ApiSpecialDir.TRASH
                else:
                    entry_json.directory_id = ApiSpecialDir.ROOT
            entry_json.entry_name = entry.name
            entry_json.lifetime = entry.pass_lifetime
            entry_json.note = entry.note
            entry_json.username = entry.username
            entry_json.password = entry.password
            entry_json.password_update_time = int(mktime(entry.password_created_at.timetuple()))

            if append_parameters:
                parameters = EntryParameter.query.filter_by(entry_id=entry.entry_id).all()
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
                related_windows = RelatedWindow.query.filter_by(entry_id=entry.entry_id).all()
                related_windows_list = []
                for related_window in related_windows:
                    related_window.crypto = self.crypto
                    if related_window.deleted_at != None:
                        continue
                    related_windows_list.append(related_window.name)
                entry_json.related_windows = related_windows_list

            res.append(entry_json)

        if recursive:
            directories = UserDirectoryView.query.filter_by(user_id=self.id, parent_id=parent_dir_id, special_directory_id=special_dir).all()
            for directory in directories:
                directory.crypto = self.crypto
                res += self.get_entries(directory.directory_id, recursive, append_parameters, append_related_windows, trash)

        return res

    def send_mail(self, subject :str, message_html :str, mail_address :str=None) -> None:
        """
        Sends mail to email address related with user.
        Args:
            subject (str): email subject
            message_html (str): email html content
            mail_address (str): email address, if None then use address related with user accoung (defaults: None)
        """

        mail = Mail(server=Config.MAIL_SMTP_SERVER, port=Config.MAIL_PORT, username=Config.MAIL_USERNAME, password=Config.MAIL_PASSWORD)
        if mail_address == None:
            mail_address = self.crypto.decrypt(self.encrypted_email)
        try:
            mail.send(mail_address, 'SNPManager', subject, message_html, is_html=True)
        except:
            pass
