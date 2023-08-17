from os import getenv
from os.path import abspath, join, dirname
from dotenv import load_dotenv



load_dotenv(join(abspath(dirname(__file__)), 'access.env'))


class Config:

    SECRET_KEY = getenv('SERVER_SECRET_KEY')
    SERVER_PORT = getenv('SERVER_PORT')

    SQLALCHEMY_DATABASE_URI = getenv('DATABASE_URI')
    SQLALCHEMY_TRACK_MODIFICATIONS = False

    MAIL_SMTP_SERVER = getenv('MAIL_SMTP_SERVER')
    MAIL_PORT = getenv('MAIL_PORT')
    MAIL_USERNAME = getenv('MAIL_USERNAME')
    MAIL_PASSWORD = getenv('MAIL_PASSWORD')

    ACCESS_TOKEN_LIFETIME = 300
    ACCESS_TOKEN_TOTAL_LIFETIME = 43200

    MAX_PASSWORD_LEN = 128
    MAX_EMAIL_LEN = 128
    MAX_DIR_LEN = 128
    MAX_ENTRY_NAME_LEN = 128
    MAX_ENTRY_USERNAME_LEN = 512
    MAX_ENTRY_PASSWORD_LEN = 512
    MAX_ENTRY_NOTE_LEN = 2048
    MAX_ENTRY_RELATED_WINDOW_LEN = 512
    MAX_ENTRY_PARAMETER_NAME_LEN = 128
    MAX_ENTRY_PARAMETER_VALUE_LEN = 512
