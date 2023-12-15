# Server application of SNPManager application

## Running application

This application has two running modes: development mode and production mode. To run it in development mode, just run wsgi.py in python3. Running server in production mode is more comlpex and it is described below.

## Configuring environment

To tun this server in production mode you must first configure the environment on the Linux system. The recommended way to run application is to use Nginx and uWSGI tools. To do it this way follow the instructions below:

1. Clone git repo somewhere on tour system and instal python dependencies from requirements.txt file.
2. Create uwsgi init file in snpm_server directory, for example `uwsgi.ini`. Example file contet:
```
[uwsgi]
module = wsgi:app

master = true
processes = 5

socket = /tmp/snpm_server.sock
chmod-socket = 660
vacuum = true

die-on-term = true
```
With this config uWSGI should run `app` from `wsgi.py` file and use `/tmp/snpm_server.sock` to handle network traffic. To make server visible from the web, we need to connect Nginx with the same socket.
3. Before setting up Nginx, we can prepare a systemd unit file to allow Linux's init system to automatically start uWSGI and serve the Flask application whenever the server boots. Create a file `/etc/systemd/system/snpm-server.service` with following content, cpmpleted with your own data:
```
[Unit]
Description=SNPManager server app
After=network.target

[Service]
User=<YOUR USERNAME>
Group=www-data
WorkingDirectory=<snpm_server DIRECTORY PATH>

Environment="SERVER_SECRET_KEY="
Environment="SERVER PORT="
Environment="SERVER ADDRESS="
Environment="DATABASE URI="
Environment="MAIL SMTP SERVER="
Environment="MAIL PORT="
Environment="MAIL USERNAME="
Environment="MAIL PASSWORD="

[Install]
WantedBy=multi-user.target
```
In this example we are storing environment variables in service file. You can modify app to collect them for example from your cloud environment.

4. Run ```sudo systemctl start snpm-server``` and ```sudo systemctl enable myproject``` to run the server and set Linux to run it automatically whenever the server boots.
5. Now uWSGI server shold be up and running, waiting for requests on the socket. To make it visible in the web configure Nginx to proxy requests. After installing Nginx, create `snpm-server` in `/etc/nginx/sites-available` directory with following content:
```
server {
    listen <YOUR SERVER PORT>;
    server_name your_domain www.your_domain;

    location / {
        include uwsgi_params;
        uwsgi_pass unix:/tmp/snpm_server.sock;
    }
}
```
To enable the Nginx server block configuration you’ve just created, link the file to the sites-enabled directory: ```sudo ln -s /etc/nginx/sites-available/myproject /etc/nginx/sites-enabled```

In case of any any problems with configuration, use this tutorial:
https://www.digitalocean.com/community/tutorials/how-to-serve-flask-applications-with-uswgi-and-nginx-on-ubuntu-18-04
