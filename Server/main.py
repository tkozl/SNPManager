from api import server



if __name__ == '__main__':
    server.run(debug=True, host='0.0.0.0', ssl_context='adhoc', port=443)
