from dotenv import load_dotenv
load_dotenv('access.env')

from os import getenv
from server import server



if __name__ == '__main__':
    server.run(debug=True, host='0.0.0.0', ssl_context='adhoc', port=int(getenv('SERVER_PORT')))
