"""Models errors"""


class ModelError(Exception):
    pass


class EmailCurrentlyInUseError(ModelError):
    pass


class DirectoryAlreadyExists(ModelError):
    pass


class EntryAlreadyExists(ModelError):
    pass


class EntryParameterAlreadyExists(ModelError):
    pass


class EntryRelatedWindowAlreadyExists(ModelError):
    pass
