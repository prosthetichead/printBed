# printBed  :printer::bed:

PrintBed is a self-hosted asset management app designed to help users efficiently sort through a large number of 3D printer model assets. With PrintBed, you can organize, search, and categorize your 3D printer models, making it easier to find the right model for your printing needs.

> [!CAUTION]
> PrintBed is currently a very early ${\color{red}Alpha}$ Use at your own risk. Some basic features are currently broken or may break in the future. 

## Features :nerd_face:	

- **Asset Organization**: Easily organize your 3D printer model assets into categories.
- **Search Functionality**: Quickly search through your collection of models using keywords or tags.
- **Customizable Tags**: Assign custom tags to your models for easy categorization and filtering.
- **User-Friendly Interface**: Intuitive interface for easy navigation and management of your assets.
- **Self-Hosted**: Keep your model assets private by hosting PrintBed on your own server.

## Installation :floppy_disk:
Curently printBed is only avalable via Docker.

Docker Compose
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

## Previews :mag:
![image](https://github.com/prosthetichead/printBed/assets/1934681/acd72138-31ec-4347-8120-4e0929e7eef4)

https://github.com/prosthetichead/printBed/assets/1934681/57e1357a-4d37-4f53-916a-a306a1a6e82a

## Contributing :thumbsup:
While this is a personal project contributions to PrintBed are welcome! If you have suggestions for new features, improvements, or bug fixes, please submit an issue or pull request on GitHub. 

## License :scroll:	
This project is licensed under the [MIT License](LICENSE).

## Acknowledgements :two_hearts:
Icons used in this project are from [Flaticon](https://www.flaticon.com/) and were created by [freepik](https://www.flaticon.com/authors/freepik).
