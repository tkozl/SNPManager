import pyodbc



class SNPMSqlServer:
    
    def __init__(self, server :str, database :str, username :str, password :str) -> None:
        self.__server = server
        self.__database = database
        self.__username = username
        self.__password = password
        self.__cnxn = None
        self.connect()
    
    def __quit__(self) -> None:
       self.disconnect()
    
    @property
    def server(self) -> str:
        return self.__server
    
    @property
    def database(self) -> str:
        return self.__database
    
    @property
    def username(self) -> str:
        return self.__username
    
    @property
    def password(self) -> str:
        return self.__password
    
    def connect(self) -> None:
        self.__cnxn = pyodbc.connect('DRIVER={SQL Server};SERVER='+self.server+';DATABASE='+self.database+';UID='+self.username+';PWD='+self.password)
    
    def disconnect(self) -> None:
        if self.__cnxn != None:
            self.__cnxn.close()
            self.__cnxn = None
    
    def get_query(self, query :str) -> list[list]:
        res = []
        with self.__cnxn.cursor() as cursor:
            cursor.execute(query)
            for row in cursor:
                res.append(list(row))
        return res
