# Database Schema and Setup Guide

## Overview

This section outlines the structure of the database used by the Secure Network Password Manager application. The database, created using Microsoft SQL Server Management Studio, plays a pivotal role in securely storing user-related data.

## Database Structure

### Tables

1. **users:** Stores user account information.
2. **encryption:** Dictionary table with cryptographic algorithm names.
3. **activity log:** Logs user account-related activities.
4. **activity type:** Dictionary table for the activity log.
5. **special directories:** IDs of special directories for user accounts.
6. **special directories types:** Dictionary table for special directories.
7. **directories:** User directories, each within a special directory.
8. **entries:** User entries, each within a directory.
9. **parameters:** Parameters associated with each entry.
10. **passwords:** Passwords assigned to entries (latest entry serves as the current password, others as history).
11. **related windows:** List of windows associated with an entry (used for auto-populating login data).

### Data Types

Due to the security-oriented approach of the project, many columns contain encrypted data. Columns with encrypted information have a data type of `varbinary`, representing binary data. This means that the information is stored in binary format, and its actual type is assigned by the server application responsible for encryption and decryption.

## Views

To optimize data access, virtual tables, known as views, have been created. These views contain explicitly provided data that can be inferred from existing tables, simplifying SQL queries significantly. The following views have been created for the solution:

1. **entry current password:** Displays the latest password from the passwords table for individual entries.
2. **entry old passwords:** Shows old passwords from the passwords table for individual entries.
3. **entry user password:** Links the entries and passwords tables with user IDs.
4. **incorrect logins 24h:** Presents the count of unsuccessful login attempts uninterrupted by successful attempts in the last 24 hours.
5. **last incorrect logins:** Displays the count of unsuccessful login attempts uninterrupted by successful attempts.
6. **locked users:** Lists locked user accounts.
7. **unlocked users:** Lists unlocked user accounts.
8. **users directories:** Displays the directories table with a column containing user IDs.
9. **users entries:** Presents the entries table with a column containing user IDs.

## Database Setup

To set up the database for the Secure Network Password Manager application, follow these steps:

1. Execute the SQL script provided in the `database-setup.sql` file in this directory.
2. Configure the database connection details in the server application.

With these steps completed, the application will be ready to securely store and manage user data within the defined database structure.
