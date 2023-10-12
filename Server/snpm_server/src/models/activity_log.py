from datetime import datetime
from sqlalchemy.sql import null

from src.utils.db import SNPMDB, CryptoDB
from src.models import db, User



class ActivityLog(db.Model, SNPMDB):

    __tablename__ = 'activity_log'

    id = db.Column('activity_id', db.Integer, primary_key=True)
    user_id = db.Column(db.Integer, db.ForeignKey('users.user_id'), nullable=False)
    activity_type_id = db.Column(db.Integer, nullable=False)
    ocurred_at = db.Column(db.DateTime, nullable=False)
    __ip = db.Column('ip', db.LargeBinary, nullable=True)
    obfuscated_ip = db.Column(db.String, nullable=True)

    def __init__(self, user :User, activity_type_id :int, ip_address :str, public_ip :bool) -> None:
        self.crypto = user.crypto
        self.user_id = user.id
        if public_ip:
            self.__ip = null()
            if ':' in ip_address:
                # IPv6
                octets = ip_address.split(':')
                obfuscated_octets = octets[:5]
                self.obfuscated_ip = ':'.join(obfuscated_octets)
            else:
                # IPv4
                octets = ip_address.split('.')
                obfuscated_octets = octets[:2]
                self.obfuscated_ip = '.'.join(obfuscated_octets)
        else:
            self.ip = ip_address
        self.activity_type_id = activity_type_id
        self.ocurred_at = datetime.now()

    @property
    def ip(self) -> str:
        if self.__ip == None:
            return None
        return self.crypto.decrypt(self.__ip)

    @ip.setter
    def ip(self, ip :str) -> None:
        self.__ip = self.crypto.encrypt(ip)

    def change_crypto(self, new_crypto :CryptoDB) -> None:
        """Encrypts table with new crypto"""
        ip = self.ip

        self.crypto = new_crypto
        self.ip = ip
