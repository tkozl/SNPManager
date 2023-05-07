from flask_sqlalchemy import SQLAlchemy

from src.app import app



# Database object
db = SQLAlchemy(app)

# Load database tables
from src.models.users import User
from src.models.activity_log import ActivityLog
from src.models.activity_type import ActivityType
from src.models.directories import Directory
from src.models.encryption import Encryption
from src.models.entries import Entry
from src.models.parameters import EntryParameter
from src.models.passwords import Password
from src.models.related_windows import RelatedWindow
from src.models.special_directories import SpecialDir

# Load database views
from src.models.entry_current_password_view import CurrentPasswordView
