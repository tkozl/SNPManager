import os
import requests


def test_blueprint_directory_get_directories(api_v1_host, api_test_headers):
    endpoint = f'{api_v1_host}/directory'
    rsp = requests.get(endpoint)
    assert rsp.status_code == 401
    rsp = requests.get(endpoint, headers=api_test_headers)
    assert rsp.status_code == 200
    directory = rsp.json()[0]
    assert 'id' in directory.keys() and 'name' in directory.keys() and 'parentID' in directory.keys()
    rsp2 = requests.get(endpoint + '?parentID=' + str(directory.get('id')), headers=api_test_headers)
    assert rsp2.status_code == 200
    assert rsp2.content != rsp.content
    rsp3 = requests.get(endpoint + '?parentID=0', headers=api_test_headers)
    assert rsp3.status_code == 404
    all_ids = []
    for directory in rsp.json():
        all_ids.append(directory.get('id'))
    rsp4 = requests.get(endpoint + '?parentID=' + str(max(all_ids) + 1), headers=api_test_headers)
    assert rsp4.status_code == 404
