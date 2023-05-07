from flask import Blueprint, jsonify, request

import src.models as models
from src.utils.auth import token_required



bp_user = Blueprint(name="blueprint_user", import_name=__name__)


@bp_user.route('/possible-activities', methods=['GET'])
def possible_activities():
    """Test api method"""

    activities = models.ActivityType.query.all()
    output = {'activities': []}
    for activity in activities:
        output['activities'].append(activity.activity_name)

    return jsonify(output)


@bp_user.route('/secret', methods=['GET'])
@token_required
def secret():
    """Test api method with auth required"""

    return {'secret': 123}
