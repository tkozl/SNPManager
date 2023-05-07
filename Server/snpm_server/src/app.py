# Load libaries
from flask import Flask, jsonify
from flask_sqlalchemy import SQLAlchemy
import sys

# Load config
from config import Config

# Init Flask app
app = Flask(__name__)
app.config.from_object(Config)

# Load modules
from src.blueprints.bp_user import bp_user

# Register blueprints
app.register_blueprint(bp_user, url_prefix="/api/v1/user")
