import os
import requests


def test_blueprint_user_possible_activities(api_v1_host):
    endpoint = f'{api_v1_host}/user/possible-activities'
    print(endpoint)
    response = requests.get(endpoint)
    assert response.status_code == 200
    json = response.json()
    assert 'activities' in json


def test_blueprint_user_secret(api_v1_host):
    endpoint = f'{api_v1_host}/user/secret'
    response = requests.get(endpoint)
    assert response.status_code == 401
