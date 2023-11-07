from flask import Blueprint, request, abort
from sqlalchemy import desc
from time import mktime

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
        directory = models.UserDirectoryView.query.filter_by(user_id=user.id, directory_id=directory_id, deleted_at=None).first()
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
    entry = models.EntryUserPasswordView.query.filter_by(entry_id=entry_id, user_id=user.id, deleted_at=None).first()
    if entry == None:
        abort(404)
    
    passwords = models.Password.query.filter_by(entry_id=entry_id)
    for password in passwords:
        password.crypto = user.crypto

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
    related_windows = models.RelatedWindow.query.filter_by(entry_id=entry_id, deleted_at=None)
    related_windows_list = []
    for related_window in related_windows:
        related_window.crypto = user.crypto
        related_windows_list.append(related_window.name)
    entry_data.related_windows = related_windows_list

    # Loading entry parameters
    parameters = models.EntryParameter.query.filter_by(entry_id=entry_id, deleted_at=None)
    parameters_list = []
    for parameter in parameters:
        parameter.crypto = user.crypto
        parameters_list.append({
            'name': parameter.name,
            'value': parameter.value
        })
    entry_data.parameters = parameters_list

    return entry_data.export_json(), 200


@bp_entry.route('/<entry_id>', methods=['PUT'])
@token_required
def edit_entry(user :models.User, token :AccessToken, entry_id :str):
    """Edits entry"""

    try:
        entry_id = int(entry_id)
    except ValueError:
        abort(404)
    
    # Loading entry
    entry_view = models.EntryUserPasswordView.query.filter_by(entry_id=entry_id, user_id=user.id, deleted_at=None).first()
    if entry_view == None:
        abort(404)

    entry_view.crypto = user.crypto
    entry = models.Entry.query.filter_by(id=entry_id).first()
    entry.crypto = user.crypto

    entry_data = EntryJSON()
    try:
        entry_data.import_json(request.json)
    except ValueError:
        abort(400)

    errors = entry_data.get_errors()
    if errors.quantity > 0:
        return errors.json, 400
    
    # Moving entry to new location if required
    if entry_data.directory_id != None or entry_data.entry_name != None:
        if entry_data.directory_id == None:
            entry_data.directory_id = entry.directory_id
        if entry_data.entry_name == None:
            entry_data.entry_name = entry.name
        if entry_data.directory_id in (ApiSpecialDir.ROOT, ApiSpecialDir.TRASH):
            new_directory_id = None
            if entry_data.directory_id == ApiSpecialDir.ROOT:
                new_special_dir_id = user.root_dir_id
            else:
                new_special_dir_id = user.trash_id
        else:
            new_directory_id = entry_data.directory_id
            new_special_dir_id = entry.special_directory_id
        if new_directory_id != None:
            # Validate if directory id is correct
            directory = models.UserDirectoryView.query.filter_by(user_id=user.id, directory_id=entry_data.directory_id).first()
            if directory == None:
                abort(404)
            if directory.special_directory_id == user.trash_id:
                abort(403)
        
        if new_directory_id != entry.directory_id or new_special_dir_id != entry.special_directory_id or entry_data.entry_name != entry.name:
            try:
                entry.move(new_directory_id, new_special_dir_id, entry_data.entry_name)
            except e.EntryAlreadyExists:
                errors = ErrorRsp()
                errors.add(errors.ALREADY_EXISTS, 'Entry with specified name already exist in destination directory')
                return errors.json
    
    # Changing entry data
    if entry_data.lifetime != None:
        entry.pass_lifetime = entry_data.lifetime
    if entry_data.note != None:
        entry.note = entry_data.note
    if entry_data.password != None and entry_data.password != entry_view.password:
        password = models.Password(user.crypto, entry_id, entry_data.password)
        models.db.session.add(password)
    if entry_data.username != None:
        entry.username = entry_data.username

    # Changing entry parameters list
    if entry_data.parameters != None:
        parameters = models.EntryParameter.query.filter_by(entry_id=entry_id, deleted_at=None).all()
        for parameter in parameters:
            updated = False

            parameter.crypto = user.crypto
            for new_parameter in entry_data.parameters:
                if new_parameter['name'] == parameter.name:
                    parameter.value = new_parameter['value']
                    updated = True
                    entry_data.parameters.remove(new_parameter)
                    break
            if not updated:
                parameter.delete(request.remote_addr)
        for new_parameter in entry_data.parameters:
            parameter = models.EntryParameter(user.crypto, entry_id, new_parameter['name'], new_parameter['value'])
            models.db.session.add(parameter)
    
    # Changing entry related windows list
    if entry_data.related_windows != None:
        related_windows = models.RelatedWindow.query.filter_by(entry_id=entry_id, deleted_at=None).all()
        for rw in related_windows:
            exists = False

            rw.crypto = user.crypto
            for new_rw in entry_data.related_windows:
                if new_rw == rw.name:
                    exists = True
                    entry_data.related_windows.remove(new_rw)
                    break
            if not exists:
                rw.delete(request.remote_addr)
        for new_rw in entry_data.related_windows:
            rw = models.RelatedWindow(user.crypto, entry_id, new_rw)
            models.db.session.add(rw)

    # Saving changes
    models.db.session.commit()

    return '', 204


@bp_entry.route('/<entry_id>/stats', methods=['GET'])
@token_required
def get_entry_stats(user :models.User, token :AccessToken, entry_id :str):
    """Gets entry statistics"""

    # Validating data
    try:
        old_passwords_limit = int(request.args.get('oldPasswordsLimit', '5'))
    except ValueError:
        abort(400)
    
    try:
        entry_id = int(entry_id)
    except ValueError:
        return '', 404
    
    # Loading data from db models
    entry = models.EntryUserPasswordView.query.filter_by(user_id=user.id, entry_id=entry_id, deleted_at=None).first()
    if entry == None:
        return '', 404
    entry.crypto = user.crypto

    # Loading old passwords
    old_passwords_list = []   
    old_passwords = models.EntryOldPasswordView.query.filter_by(id=entry_id).order_by(desc(models.EntryOldPasswordView.created_at)).limit(old_passwords_limit).all()
    for old_password in old_passwords:
        old_password.crypto = user.crypto
        old_passwords_list.append({
            'chDate': int(mktime(old_password.created_at.timetuple())),
            'password': old_password.value
        })
    
    # Preparing json rsp
    return {
        'creationDate': int(mktime(entry.created_at.timetuple())),
        'lastPassChange': int(mktime(entry.password_created_at.timetuple())),
        'oldPasswords': old_passwords_list
    }, 200
