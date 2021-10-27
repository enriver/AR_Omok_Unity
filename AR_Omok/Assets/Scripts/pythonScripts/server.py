from flask import Flask
from flask import request

app=Flask(__name__)

@app.route('/main')
def main_page():
    return 'Hello World!'


if __name__=="__main__":
    app.run(host='10.50.17.88', port=5000)