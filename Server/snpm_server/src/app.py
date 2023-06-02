# Load libaries
from flask import Flask, jsonify
from flask_cors import CORS
from flask_sqlalchemy import SQLAlchemy
import sys

# Load config
from config import Config

# Init Flask app
app = Flask(__name__)
CORS(app)
app.config.from_object(Config)

# Load modules
from src.blueprints.bp_account import bp_account
from src.blueprints.bp_login import bp_login

# Register blueprints
app.register_blueprint(bp_account, url_prefix="/api/v1/account")
app.register_blueprint(bp_login, url_prefix="/api/v1/login")
