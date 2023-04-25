import pyodbc



class SNPMSqlServer:
    
    def __init__(self, server :str, database :str, username :str, password :str, sql_driver :str) -> None:
        self.__server = server
        self.__database = database
        self.__username = username
        self.__password = password
        self.__sql_driver = sql_driver
        self.__cnxn = None
        self.connect()
    
    def __quit__(self) -> None:
       self.disconnect()
    
    def connect(self) -> None:
        self.__cnxn = pyodbc.connect('DRIVER={'+self.__sql_driver+'};SERVER='+self.__server+';DATABASE='+self.__database+';UID='+self.__username+';PWD='+self.__password+';TrustServerCertificate=yes')
    
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
