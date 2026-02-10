# Refuge WPF

# Introduction

Windows Presentation Foundation application for an animal shelter.
Based on course from IPEFA.

## Installation

1. Download the project
````bash
$ git clone https://github.com/iracanyes/RefugeWPF 
````
2. Create the environment files from template files ``RefugeWPF/.env.dist`` and ``RefugeWPF/RefugeWPF/.env.dist`` 
to their corresponding file ``RefugeWPF/.env`` and ``RefugeWPF/RefugeWPF/.env``. Change all required values  inside those files.
````bash
$ cd RefugeWPF
$ cp .env.dist .env && cp RefugeWPF/.env.dist RefugeWPF/.env
````

3. Start Docker, go to the root directory of the project and run the following command to build and run DB dependency
````bash
$ docker-compose up -d 
````
4. Use DB IDE tools like DataGrid (from Jetbrains) or a shell to connect to the database
5. Run all DB script inside the directory ``data`` to populate your database with tables, the stored procedures and triggers used  by the application.
6. Finally, you can build and run the application. 