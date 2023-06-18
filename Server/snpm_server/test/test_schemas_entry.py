from src.schemas.entry import EntryJSON
from pytest import raises



def test_entry_json():
    entry_json = {
        "directoryID": '1',
        "entryName": "name",
        "lifetime": 0,
        "note": "",
        "password": "securepassword",
        "username": "username",
        "relatedWindows": [
            "window"
        ],
        "parameters": [
            {
                "name": "param",
                "value": "value"
            }
        ]
    }

    # Testing import
    entry = EntryJSON()
    entry.import_json(entry_json)
    assert entry.directory_id == 1
    assert entry.entry_name == 'name'
    assert entry.lifetime == 0
    assert entry.note == ''
    assert entry.username == 'username'
    assert entry.password == 'securepassword'
    assert entry.parameters == [{"name": "param", "value": "value"}]
    assert entry.related_windows == ['window']

    assert entry.is_complete() == True
    entry2 = entry
    entry2.entry_name = None
    assert entry2.is_complete() == False

    # Testing export
    assert len(entry.export_json().keys()) == 8
    assert list(entry.export_json('entryName+note').keys()) == ['entryName', 'note']

    entry.entry_id = '2'
    assert len(entry.export_json().keys()) == 9
    assert list(entry.export_json('entryName+note').keys()) == ['entryName', 'note', 'entryID']
