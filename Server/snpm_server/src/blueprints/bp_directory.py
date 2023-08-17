from flask import Blueprint, jsonify, request, abort

import src.models as models
import src.models.errors as e
from src.utils.auth import token_required
from src.utils.data_validation import is_dir_name_correct
from src.utils.token import AccessToken
from src.schemas.error_rsp import ErrorRsp
from src.schemas.api_special_dir import ApiSpecialDir
from config import Config



bp_directory = Blueprint(name="blueprint_directory", import_name=__name__)


@bp_directory.route('', methods=['POST'])
@token_required
def create_directory(user :models.User, token :AccessToken):
    """Creates new directory"""

    directory_name = request.json.get('name')
    parent_id = request.json.get('parentID')
    if directory_name == None:
        abort(400)
    
    # Validating user data
    try:
        parent_id = int(parent_id)
    except ValueError:
        abort(404)
    errors = ErrorRsp()
    if len(directory_name) > Config.MAX_DIR_LEN:
        errors.add(ErrorRsp.TOO_LONG_STRING, f'Max allowed directory name is {Config.MAX_DIR_LEN}')
    if not is_dir_name_correct(directory_name):
        errors.add(ErrorRsp.INVALID_NAME, 'Directory name contains forbidden chars')
    if errors.quantity > 0:
        return errors.json, 400
    
    # Validating parent directory id
    if parent_id == ApiSpecialDir.TRASH:
        return '', 403
    if parent_id != ApiSpecialDir.ROOT:
        parent_dir = models.UserDirectoryView.query.filter_by(directory_id=parent_id, user_id=user.id).first()
        if parent_dir == None:
            abort(404)
        if parent_dir.special_directory_id == models.SpecialDir.TRASH_ID:
            return '', 403
    else:
        parent_id = None
    
    # Creating directory
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
    
    if parent_id == None:
        abort(400)
    try:
        parent_id = int(parent_id)
    except ValueError:
        abort(404)
    if recursive not in ('true', 'false'):
        abort(400)
    if recursive == 'true':
        recursive = True
    else:
        recursive = False
    trash = False
    if parent_id not in (ApiSpecialDir.ROOT, ApiSpecialDir.TRASH):
        # Checks if parent directory exists
        parent_dir = models.UserDirectoryView.query.filter_by(directory_id=parent_id, user_id=user.id).first()
        if parent_dir == None:
            abort(404)
        if parent_dir.special_directory_id == user.trash_id:
            trash = True
    else:
        if parent_id == ApiSpecialDir.TRASH:
            trash = True
        else:
            trash = False
        parent_id = None

    rsp = []
    directories = user.get_directories(parent_id, recursive, trash)
    for directory in directories:
        directory_json = {
            'id': directory['directory_id'],
            'name': directory['directory_name'],
            'parentID': directory['parent_id']
        }
        if directory_json['parentID'] == None:
            if trash:
                directory_json.update({'parentID': ApiSpecialDir.TRASH})
            else:
                directory_json.update({'parentID': ApiSpecialDir.ROOT})
        
        rsp.append(directory_json)
    
    return rsp


@bp_directory.route('/<directory_id>', methods=['GET'])
@token_required
def get_directory_info(user :models.User, token :AccessToken, directory_id :str):
    """Gets directory info"""

    try:
        directory_id = int(directory_id)
    except ValueError:
        abort(404)
    
    # Loading directory
    directory = models.UserDirectoryView.query.filter_by(user_id=user.id, directory_id=directory_id, deleted_at=None).first()
    if directory == None:
        if directory_id in (ApiSpecialDir.ROOT, ApiSpecialDir.TRASH):
            abort(403)
        else:
            abort(404)
    
    directory.crypto = user.crypto
    
    parent_dir_id = directory.parent_id
    if parent_dir_id == None:
        if directory.special_directory_id == user.root_dir_id:
            parent_dir_id = ApiSpecialDir.ROOT
        else:
            parent_dir_id = ApiSpecialDir.TRASH
    
    return {
        'name': directory.name,
        'parentID': parent_dir_id
    }, 200


@bp_directory.route('/<directory_id>', methods=['PUT'])
@token_required
def edit_directory(user :models.User, token :AccessToken, directory_id :str):
    """Gets directory info"""

    # Loading request data
    new_directory_name = request.json.get('name')
    new_parent_id = request.json.get('parentID')
    if new_directory_name == None or new_parent_id == None:
        abort(400)
    
    errors = ErrorRsp()

    # Validating data
    try:
        directory_id = int(directory_id)
        new_parent_id = int(new_parent_id)
    except ValueError:
        abort(404)

    if directory_id == new_parent_id:
        abort(403)

    if not is_dir_name_correct(new_directory_name):
        errors.add(ErrorRsp.INVALID_NAME, error_msg='Invalid destination directory name')
        return errors.json, 400
    
    if len(new_directory_name) > Config.MAX_DIR_LEN:
        errors.add(ErrorRsp.TOO_LONG_STRING, error_msg=f'Max allowed directory name length is {Config.MAX_DIR_LEN}')
        return errors.json, 400
    
    # Loading directory
    directory = models.UserDirectoryView.query.filter_by(user_id=user.id, directory_id=directory_id, deleted_at=None).first()
    if directory == None:
        if directory_id in (ApiSpecialDir.ROOT, ApiSpecialDir.TRASH):
            abort(403)
        else:
            abort(404)
    
    directory.crypto = user.crypto

    new_special_dir_id = directory.special_directory_id
    if new_parent_id == ApiSpecialDir.ROOT:
        new_parent_id = None
        new_special_dir_id = user.root_dir_id
    elif new_parent_id == ApiSpecialDir.TRASH:
        new_parent_id = None
        new_special_dir_id = user.trash_id

    # Checking if requested destination id and name are available
    if new_parent_id != None:
        dst_directory = models.UserDirectoryView.query.filter_by(user_id=user.id, directory_id=new_parent_id, deleted_at=None).first()
        if dst_directory == None:
            abort(404)
    dst_dir_content = models.UserDirectoryView.query.filter_by(user_id=user.id, parent_id=new_parent_id, deleted_at=None).all()
    for dst_dir_element in dst_dir_content:
        dst_dir_element.crypto = user.crypto
        if dst_dir_element.name == new_directory_name and dst_dir_element.directory_id != directory.directory_id:
            errors.add(ErrorRsp.ALREADY_EXISTS, 'Directory with specified name already exists in selected directory')
            return errors.json, 400
    
    # Changing directory data
    directory = models.Directory.query.filter_by(id=directory_id).first()
    directory.crypto = user.crypto
    directory.name = new_directory_name
    directory.parent_id = new_parent_id

    # Changing special dir id for each child entry and directory if special dir id has been changed
    if new_special_dir_id != directory.special_directory_id:
        directory.special_directory_id = new_special_dir_id
        trash = False
        if new_special_dir_id == user.trash_id:
            trash = True
        child_directories = user.get_directories(parent_id=directory_id, recursive=True, trash=not trash)
        child_entries = user.get_entries(parent_dir_id=directory_id, recursive=True, trash=not trash)
        for child_dir in child_directories:
            child = models.Directory.query.filter_by(id=child_dir['directory_id']).first()
            child.crypto = user.crypto
            child.special_directory_id = new_special_dir_id
        for child_entry in child_entries:
            child = models.Entry.query.filter_by(id=child_entry.entry_id).first()
            child.special_directory_id = new_special_dir_id

    # Saving changes
    models.db.session.commit()
    return '', 204


@bp_directory.route('/special', methods=['GET'])
@token_required
def get_special_dir_id(user :models.User, token :AccessToken):
    """Returns ids of the special directories"""

    return {
        'root': ApiSpecialDir.ROOT,
        'trash': ApiSpecialDir.TRASH
    }, 200
