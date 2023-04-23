from flask import Flask
from flask_restx import Resource, Api
from dotenv import load_dotenv
from os import getenv

from sql import SNPMSqlServer


class SNPMServer(Flask):

    def __init__(self, *args, **kwargs):
        super(SNPMServer, self).__init__(*args, **kwargs)
        load_dotenv('access.env')
        sql_access = {
            'server': getenv('SQL_SERVER'),
            'database': getenv('SQL_SERVER_DATABASE'),
            'username': getenv('SQL_SERVER_USERNAME'),
            'password': getenv('SQL_SERVER_PASS')
        }
        self.__sql = SNPMSqlServer(**sql_access)
    
    @property
    def sql(self) -> SNPMSqlServer:
        return self.__sql


server = SNPMServer(__name__)
api = Api(server)


@api.route('/hello')
class Account(Resource):
    def get(self):
        return {'msg': 'Hello World!'}
