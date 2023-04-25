import smtplib
from html2text import html2text
from email.mime.text import MIMEText
from email.mime.multipart import MIMEMultipart
from email.utils import formataddr
from ssl import create_default_context


class Mail:

    def __init__(self, server :str, port :int, username :str, password :str):
        self.__server = server
        self.__port = port
        self.__username = username
        self.__password = password
    
    def send(self, receiver :str, sender_name :str, subject :str, content :str, is_html :bool):
        message = MIMEMultipart("alternative")
        if is_html:
            message.attach(MIMEText(html2text(content), 'plain'))
            message.attach(MIMEText(content, 'html'))
        else:
            message.attach(MIMEText(content, 'plain'))
        
        
        message['Subject'] = subject
        message['From'] = formataddr((sender_name, self.__username))
        message['To'] = receiver
        
        context = create_default_context()
        with smtplib.SMTP_SSL(self.__server, self.__port, context=context) as server:
            server.login(self.__username, self.__password)
            server.send_message(message)
