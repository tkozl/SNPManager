from src.schemas import ErrorRsp
from src.utils.data_validation import is_entry_name_correct
from config import Config



class EntryJSON:

    def __init__(self) -> None:
        self.entry_id = None

    def import_json(self, entry_json :dict) -> None:
        """
        Imports entry json schema
        Args:
            entry_json (dict): entry data
        """

        self.entry_id = entry_json.get('entryID')
        self.directory_id = entry_json.get('directoryID')
        self.entry_name = entry_json.get('entryName')
        self.username = entry_json.get('username')
        self.password = entry_json.get('password')
        self.note = entry_json.get('note')
        self.lifetime = entry_json.get('lifetime')
        self.related_windows = entry_json.get('relatedWindows')
        self.parameters = entry_json.get('parameters')
    
    def export_json(self, include_str :str=None) -> dict:
        """
        Exports entry json
        Args:
            include_str (str): string received from user defines what entry parts should be placed in json, if None returns whole json (defaults: None)
        Return:
            entry data dict
        """

        if include_str == None:
            res = {
                'directoryID': self.directory_id,
                'entryName': self.entry_name,
                'username': self.username,
                'password': self.password,
                'note': self.note,
                'lifetime': self.lifetime,
                'relatedWindows': self.related_windows,
                'parameters': self.parameters
            }
            if self.entry_id != None:
                res.update({'entryID': self.entry_id})
            return res
        else:
            required_parts = include_str.replace(' ', '+').split('+')
            res = {}
            if 'directoryID' in required_parts:
                res.update({'directoryID': self.directory_id})
            if 'entryName' in required_parts:
                res.update({'entryName': self.entry_name})
            if 'username' in required_parts:
                res.update({'username': self.username})
            if 'password' in required_parts:
                res.update({'password': self.password})
            if 'note' in required_parts:
                res.update({'note': self.note})
            if 'lifetime' in required_parts:
                res.update({'lifetime': self.lifetime})
            if'relatedWindows' in required_parts:
                res.update({'relatedWindows': self.related_windows})
            if 'parameters' in required_parts:
                res.update({'parameters': self.parameters})
            if self.entry_id != None:
                res.update({'entryID': self.entry_id})
            return res
    
    def get_errors(self) -> ErrorRsp:
        """
        Looks for errors in entry data
        Return:
            ErrorRsp object
        """

        errors = ErrorRsp()
        if len(self.entry_name) > Config.MAX_ENTRY_NAME_LEN:
            errors.add(ErrorRsp.TOO_LONG_STRING, f'Max entry name length is {Config.MAX_ENTRY_NAME_LEN}')
        if not is_entry_name_correct(self.entry_name):
            errors.add(ErrorRsp.INVALID_NAME, 'Invalid entry name')
        if len(self.username) > Config.MAX_ENTRY_USERNAME_LEN:
            errors.add(ErrorRsp.TOO_LONG_STRING, f'Max entry username length is {Config.MAX_ENTRY_USERNAME_LEN}')
        if len(self.password) > Config.MAX_ENTRY_PASSWORD_LEN:
            errors.add(ErrorRsp.TOO_LONG_STRING, f'Max entry password length is {Config.MAX_ENTRY_PASSWORD_LEN}')
        if len(self.note) > Config.MAX_ENTRY_NOTE_LEN:
            errors.add(ErrorRsp.TOO_LONG_STRING, f'Max entry note length is {Config.MAX_ENTRY_NOTE_LEN}')
        for related_window in self.related_windows:
            if len(related_window) > Config.MAX_ENTRY_RELATED_WINDOW_LEN:
                errors.add(ErrorRsp.TOO_LONG_STRING, f'Max entry related window length is {Config.MAX_ENTRY_RELATED_WINDOW_LEN}')
        for parameter in self.parameters:
            name, value = parameter['name'], parameter['value']
            if len(name) > Config.MAX_ENTRY_PARAMETER_NAME_LEN:
                errors.add(ErrorRsp.TOO_LONG_STRING, f'Max parameter name length is {Config.MAX_ENTRY_PARAMETER_NAME_LEN}')
            if len(value) > Config.MAX_ENTRY_PARAMETER_VALUE_LEN:
                errors.add(ErrorRsp.TOO_LONG_STRING, f'Max parameter value length is {Config.MAX_ENTRY_PARAMETER_VALUE_LEN}')
        return errors
    
    def is_complete(self) -> bool:
        """Checks if object contains required all entry data"""
        try:
            if self.entry_name == None or self.username == None or self.password == None or self.note == None or self.lifetime == None:
                return False
        except Exception:
            return False
        return True

    @property
    def entry_id(self) -> int:
        return self.__entry_id
    
    @entry_id.setter
    def entry_id(self, value) -> None:
        if value == None:
            self.__entry_id = None
        else:
            self.__entry_id = int(value)
    
    @property
    def directory_id(self) -> int:
        return self.__directory_id
    
    @directory_id.setter
    def directory_id(self, value) -> None:
        if value == None:
            self.__directory_id = None
        else:
            self.__directory_id = int(value)
    
    @property
    def entry_name(self) -> str:
        return self.__entry_name
    
    @entry_name.setter
    def entry_name(self, value) -> None:
        self.__entry_name = value

    @property
    def username(self) -> str:
        return self.__username
    
    @username.setter
    def username(self, value) -> None:
        self.__username = value
    
    @property
    def password(self) -> str:
        return self.__password
    
    @password.setter
    def password(self, value) -> None:
        self.__password = value
    
    @property
    def note(self) -> str:
        return self.__note
    
    @note.setter
    def note(self, value) -> None:
        self.__note = value
    
    @property
    def lifetime(self) -> int:
        return self.__lifetime
    
    @lifetime.setter
    def lifetime(self, value) -> None:
        self.__lifetime = int(value)
    
    @property
    def related_windows(self) -> list[str]:
        return self.__related_windows
    
    @related_windows.setter
    def related_windows(self, value) -> None:
        if value == None:
            self.__related_windows = []
        else:
            if type(value) != type([]):
                raise ValueError('Related windows should be a list')
            related_windows = []
            for item in value:
                related_windows.append(str(item))
                
            self.__related_windows = related_windows
    
    @property
    def parameters(self) -> list[dict]:
        return self.__parameters
    
    @parameters.setter
    def parameters(self, value) -> None:
        if value == None:
            self.__parameters = []
        else:
            if type(value) != type([]):
                raise ValueError('Parameters should be a list of dicts')
            parameters = []
            for item in value:
                if type(item) != type({}):
                    raise ValueError('All of the parameters in list should be a dicts')
                p_name = item.get('name')
                p_value = item.get('value')
                if p_name == None or p_value == None:
                    raise ValueError('All parameters shoul have str "name" and "value" keys')
                p_name = str(p_name)
                p_value = str(p_value)

                parameters.append({
                    'name': p_name,
                    'value': p_value
                })
                
            self.__parameters = parameters
