"""Models errors"""


class ModelError(Exception):
    pass


class EmailCurrentlyInUseError(ModelError):
    pass
