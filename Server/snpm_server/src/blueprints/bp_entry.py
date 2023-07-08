from flask import Blueprint, request, abort

import src.models as models
import src.models.errors as e
from src.utils.auth import token_required
from src.utils.token import AccessToken
from src.schemas.error_rsp import ErrorRsp
from src.schemas.entry import EntryJSON
from src.schemas.api_special_dir import ApiSpecialDir


bp_entry = Blueprint(name="blueprint_entry", import_name=__name__)


@bp_entry.route('', methods=['POST'])
@token_required
def create_entry(user :models.User, token :AccessToken):
    """Creates new entry"""

    entry_data = EntryJSON()
    try:
        entry_data.import_json(request.json)
    except ValueError:
        abort(400)
    
    if entry_data.is_complete() == False:
        abort(400)

    errors = entry_data.get_errors()
    if errors.quantity > 0:
        return errors.json, 400
    
    # Validate if directory id is correct
    entry_directory_id = entry_data.directory_id
    if entry_data.directory_id == ApiSpecialDir.TRASH:
        abort(403)
    if entry_data.directory_id != ApiSpecialDir.ROOT:
        directory = models.UserDirectoryView.query.filter_by(user_id=user.id, directory_id=entry_data.directory_id).first()
        if directory == None:
            abort(404)
        if directory.special_directory_id == user.trash_id:
            abort(403)
    else:
        entry_directory_id = None
    
    # Create new entry
    try:
        entry = models.Entry(user.crypto, user.id, entry_directory_id, user.root_dir_id, entry_data.entry_name, entry_data.username, entry_data.note, entry_data.lifetime)
    except e.EntryAlreadyExists:
        errors.add(ErrorRsp.ALREADY_EXISTS, 'Entry with the same name already exists in specified directory')
        return errors.json, 400
    
    models.db.session.add(entry)
    models.db.session.commit()

    entry_password = models.Password(user.crypto, entry.id, entry_data.password)
    models.db.session.add(entry_password)

    for entry_related_window in entry_data.related_windows:
        try:
            related_window = models.RelatedWindow(user.crypto, entry.id, entry_related_window)
        except e.EntryRelatedWindowAlreadyExists:
            continue
        models.db.session.add(related_window)

    for parameter in entry_data.parameters:
        try:
            entry_parameter = models.EntryParameter(user.crypto, entry.id, parameter['name'], parameter['value'])
        except e.EntryParameterAlreadyExists:
            continue
        models.db.session.add(entry_parameter)
    
    models.db.session.commit()

    return {
        'id': entry.id
    }, 201


@bp_entry.route('', methods=['GET'])
@token_required
def get_entries(user :models.User, token :AccessToken):
    """Gets list of entries"""
    
    # Loading request data
    include = request.args.get('include', '')
    directory_id = request.args.get('directoryID')
    recursive = request.args.get('recursive', 'true')

    if directory_id == None:
        abort(400)

    try:
        directory_id = int(directory_id)
    except ValueError:
        abort(404)
    requested_parts = include.replace(' ', '+').split('+')
    trash = False
    if directory_id == ApiSpecialDir.TRASH:
        trash = True
    if directory_id in (ApiSpecialDir.ROOT, ApiSpecialDir.TRASH):
        directory_id = None
    if recursive == 'true':
        recursive = True
    else:
        recursive = False
    
    # Checking if directory id is correct
    if directory_id != None:
        try:
            directory_id = int(directory_id)
        except ValueError:
            abort(404)
        directory = models.UserDirectoryView.query.filter_by(user_id=user.id, directory_id=directory_id).first()
        if directory == None:
            abort(404)
        if directory.special_directory_id == user.trash_id:
            trash = True
    
    # Interpreting request data
    if 'parameters' in requested_parts:
        append_parameters = True
    else:
        append_parameters = False
    if 'relatedWindows' in requested_parts:
        append_related_windows = True
    else:
        append_related_windows = False
    
    # Loading entries data from sql db
    entries = user.get_entries(directory_id, recursive, append_parameters, append_related_windows, trash)
    rsp = []
    for entry in entries:
        rsp.append(entry.export_json(include))
    return rsp, 200


@bp_entry.route('/<entry_id>', methods=['GET'])
@token_required
def get_entry(user :models.User, token :AccessToken, entry_id :str):
    """Gets entry data"""

    try:
        entry_id = int(entry_id)
    except ValueError:
        abort(404)
    
    # Loading entry
    entry = models.EntryUserPasswordView.query.filter_by(entry_id=entry_id, user_id=user.id).first()
    if entry == None:
        abort(404)

    entry.crypto = user.crypto

    entry_data = EntryJSON()
    if entry.directory_id == None:
        if entry.special_directory_id == user.trash_id:
            entry_data.directory_id = ApiSpecialDir.TRASH
        else:
            entry_data.directory_id = ApiSpecialDir.ROOT
    else:
        entry_data.directory_id = entry.directory_id
    entry_data.entry_name = entry.name
    entry_data.username = entry.username
    entry_data.password = entry.password
    entry_data.note = entry.note
    entry_data.lifetime = entry.pass_lifetime
    
    # Loading entry related windows
    related_windows = models.RelatedWindow.query.filter_by(entry_id=entry_id)
    related_windows_list = []
    for related_window in related_windows:
        related_window.crypto = user.crypto
        related_windows_list.append(related_window.name)
    entry_data.related_windows = related_windows_list

    # Loading entry parameters
    parameters = models.EntryParameter.query.filter_by(entry_id=entry_id)
    parameters_list = []
    for parameter in parameters:
        parameter.crypto = user.crypto
        parameters_list.append({
            'name': parameter.name,
            'value': parameter.value
        })
    entry_data.parameters = parameters_list

    return entry_data.export_json(), 200
