# Secure Network Password Manager

## Overview

This repository contains the source code for the "Secure Network Password Manager" solution. The system is designed as a client-server architecture, featuring a server-side component handling client requests and a client-side component interacting directly with end users. It operates in a centralized manner, with user data operations executed on the server, and client applications responsible for facilitating user interactions with the server.

The application server maintains a connection with a database that stores all user account information. Unlike the database, the server is accessible over the Internet, enabling client applications to send queries to it. Upon receiving a request, the server performs specific operations based on the received query and the permissions of the requesting user. A relevant response is then sent back to the client. The server is responsible for ensuring the security of data storage and enabling secure access to it.

The client application aims to enhance user comfort while using the solution. It enables system management through a user-friendly graphical interface, eliminating the need for manually sending network queries. It supports all server-offered functionalities, allowing users to create and delete accounts, manage accounts, as well as create and edit entries and directories. The application also includes additional features to improve the system's usability, such as automatic password completion.


## Security

Ensuring the utmost protection for user data, the Secure Network Password Manager employs a robust encryption mechanism. All sensitive user data stored in the database are encrypted using unique cryptographic keys known only to the respective users. Keys are calculated basing on users passwords, and salted with users email addresses. This approach enhances the security posture by ensuring that even in the unlikely event of a data breach, the encrypted data remains unreadable without the specific cryptographic keys.

The Secure Network Password Manager adopts a defense-in-depth strategy, combining encryption with other security measures, such as secure communication protocols. This comprehensive approach aims to create a resilient environment where user data remains confidential and secure throughout its lifecycle within the system. Users can trust that their sensitive information is safeguarded by state-of-the-art encryption, providing peace of mind as they leverage the features of the password management solution.

## Features

- **Centralized Architecture:** The system operates as a client-server architecture, ensuring efficient management of user data with a centralized approach.

- **Secure Data Storage:** User data is stored securely in the database, and all sensitive information is encrypted using unique cryptographic keys known only to the respective users.

- **User-Friendly Interface:** The client application offers a user-friendly graphical interface, enhancing the overall user experience and simplifying system interaction.

- **Account Management:** Users can create, delete, and edit their accounts, providing flexibility and control over their credentials.

- **Entry and Directory Management:** The system supports the creation and editing of entries and directories, facilitating organized password management.

- **Additional Usability Features:** The client application includes features like automatic password completion to increase the overall usability and convenience of the system.

- **Security Measures:** The system prioritizes security, implementing encryption for data at rest and in transit, secure key management, and a defense-in-depth strategy to safeguard user information.