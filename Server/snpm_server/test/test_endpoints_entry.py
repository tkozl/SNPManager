import requests


def test_blueprint_entry_get_entries(api_v1_host, api_test_headers):
    endpoint = f'{api_v1_host}/entry?include=directoryID+entryName+username+password+note+lifetime+relatedWindows+parameters'
    rsp = requests.get(endpoint)
    assert rsp.status_code == 401
    rsp = requests.get(endpoint, headers=api_test_headers)
    assert rsp.status_code == 200
    entry = rsp.json()[0]
    assert len(entry.keys()) == 9

    endpoint = f'{api_v1_host}/entry'
    rsp = requests.get(endpoint, headers=api_test_headers)
    assert rsp.status_code == 200
    entry = rsp.json()[0]
    assert len(entry.keys()) == 1
