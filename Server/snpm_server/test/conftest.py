import pytest
from config import Config


def pytest_addoption(parser):
    # ability to test API on different hosts
    parser.addoption("--host", action="store", default='http://localhost:5000')

@pytest.fixture(scope="session")
def host(request):
    return request.config.getoption("--host")

@pytest.fixture(scope="session")
def api_v1_host(host):
    return f'{host}/api/v1'

@pytest.fixture(scope="session")
def api_test_token():
    return 'eyJjaXBoZXJ0ZXh0IjoiZEZXZW1tcjRUVFZNUXJKbDdodjJKOGU2TlNnMmdlV0lCSlYyZzVkc1FQbkwtMHFKR0wyemd5Mzlza2lldHVyQnNBQjVleDIzMkVnV0xnWHJhRURnNmxTVHFWdjFWZVYyZDJaeDB2dXd2R0VHV2stUmp1ODVKbUpBZ1dlYXNvVWk2U3BLTWNWMFZqMEl3c0xtcFpVa01IdG9fNmx2cFBFZU96cVBtVFJSMFpUQWVRUEhyQlZlS2UwN3JlVk9iQUtjUGJ4OTlYZnRzSjE4N2JvbDU3X0tmeDcxaTkwQWVGRllqaTlyblJJR093cm9pcHdxc3VVM1p3QzRVNmtKdUlUVyIsImVuY3J5cHRlZF9rZXkiOiIwM0FscFNsaTNJd3VMbGs3ZmExYmpiRVpFQmtaeExoaW1sRXh6YWxmR3U0ZU5VYjBFMEF4UnVRT1BDZEZLako4Nm50djNkRl9ub200b1R3Vlhia1Q1N2l3RmV0VzJOaG0iLCJpdiI6IlB5UllIMXp3YnY2SlM2OWVTTTdkMWciLCJwcm90ZWN0ZWQiOiJleUpoYkdjaU9pSkJNalUyUzFjaUxDSmxibU1pT2lKQk1qVTJRMEpETFVoVE5URXlJbjAiLCJ0YWciOiJzNXl2Q29WdmpuWW5IdEtraG5CR2I5R3E0MjJWWWgxU2JKM21yTGZkaFZnIn0='

@pytest.fixture(scope="session")
def api_test_headers(api_test_token):
    return {
        'Authorization': f'Bearer {api_test_token}',
        'accept': 'application/json'
    }
