from flask import Blueprint, jsonify, request, abort
from Crypto.Hash import SHA1
import requests



bp_check_password_strength = Blueprint(name="blueprint_check_password_strength", import_name=__name__)


@bp_check_password_strength.route('', methods=['POST'])
def check_password_strength():
    """Checks if password was ever leaked"""

    password = str(request.json.get('password'))
    if password == None:
        abort(400)
    
    h = SHA1.new()
    h.update(password.encode())
    hash = h.hexdigest()
    hash_short = hash[:5]
    rsp = requests.get(f'https://api.pwnedpasswords.com/range/{hash_short}')
    if rsp.status_code != 200:
        abort(500)
    items = rsp.text.split('\n')
    for item in items:
        if item.split(':')[0].lower() == hash[5:].lower():
            return {
                'strong': False
            }, 200
    return {
        'strong': True
    }, 200
