from flask import Flask, request, jsonify, make_response


app = Flask(__name__)

@app.route('/', methods=['GET'])
def hello_world():
    return make_response(jsonify({'response': 'hello world'}), 200)


if __name__ == '__main__':
    app.run(debug=True, host='0.0.0.0', ssl_context='adhoc', port=443)
