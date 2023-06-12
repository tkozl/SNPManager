from flask import Blueprint, jsonify, request, abort

import src.models as models
import src.models.errors as e
from src.utils.auth import token_required
from src.utils.data_validation import is_dir_name_correct
from src.utils.token import AccessToken
from src.schemas.error_rsp import ErrorRsp
from config import Config



bp_directory = Blueprint(name="blueprint_directory", import_name=__name__)


@bp_directory.route('', methods=['POST'])
@token_required
def create_directory(user :models.User, token :AccessToken):
    """Creates new directory"""

    directory_name = request.json.get('name')
    parent_id = request.json.get('parentID', '')
    if directory_name == None:
        abort(400)
    
    # Validating user data
    errors = ErrorRsp()
    if len(directory_name) > Config.MAX_DIR_LEN:
        errors.add(ErrorRsp.TOO_LONG_STRING, f'Max allowed directory name is {Config.MAX_DIR_LEN}')
    if not is_dir_name_correct(directory_name):
        errors.add(ErrorRsp.INVALID_NAME, 'Directory name contains forbidden chars')
    if errors.quantity > 0:
        return errors.json, 400
    
    # Validating parent directory id
    if len(str(parent_id)) > 0:
        parent_dir = models.UserDirectoryView.query.filter_by(directory_id=parent_id, user_id=user.id).first()
        if parent_dir == None:
            abort(404)
        if parent_dir.special_directory_id == models.SpecialDir.TRASH_ID:
            return '', 403
    
    # Creating directory
    if len(str(parent_id)) == 0:
        parent_id = None
    try:
        directory = models.Directory(user.crypto, directory_name, parent_id, user.root_dir_id)
    except e.DirectoryAlreadyExists:
        errors.add(ErrorRsp.ALREADY_EXISTS, 'Directory already exists in selected directory')
        return errors.json, 400
    models.db.session.add(directory)
    models.db.session.commit()

    return {
        'id': directory.id
    }, 201


@bp_directory.route('', methods=['GET'])
@token_required
def get_directories(user :models.User, token :AccessToken):
    """Gets directories tree"""

    parent_id = request.args.get('parentID')
    recursive = request.args.get('recursive', 'true')
    
    if recursive not in ('true', 'false'):
        abort(400)
    if recursive == 'true':
        recursive = True
    else:
        recursive = False
    if parent_id != None:
        # Checks if parent directory exists
        parent_dir = models.UserDirectoryView.query.filter_by(directory_id=parent_id, user_id=user.id).first()
        if parent_dir == None:
            abort(404)

    rsp = []

    directories = user.get_directories(parent_id, recursive)
    for directory in directories:
        directory_json = {
            'id': directory['directory_id'],
            'name': directory['directory_name'],
            'parentID': directory['parent_id']
        }
        
        rsp.append(directory_json)
    
    return rsp
