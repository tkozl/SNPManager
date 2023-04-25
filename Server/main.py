from dotenv import load_dotenv
from os import getenv
from server import server



if __name__ == '__main__':
    load_dotenv('access.env')
    server.run(debug=True, host='0.0.0.0', ssl_context='adhoc', port=int(getenv('SERVER_PORT')))
