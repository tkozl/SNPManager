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

    MAX_PASSWORD_LEN = 128
    MAX_EMAIL_LEN = 128
