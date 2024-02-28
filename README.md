
${\color{red}WARNING}$ PrintBed is currently a very early ${\color{red}Alpha}$

Use at your own risk. Some basic features are currently broken or may break in the future. 

Docker Compose Example to get started
```
version: "2.1"
services:  
  printbed:
    container_name: printbed
    image: prosthetichead/printbed:latest
    volumes:
      - [DB Path]:/appdata
      - [Print Files Path]:/print-files
    ports: 
      - 4040:8080
```
* Replece [DB Path] with local location of database for example ~/appdata/printbed
* Replece [Print Files Path] with local path for storing print files for example ~/mnt/storage/3d-prints

