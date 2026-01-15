# printBed  :printer::bed:

printBed is a self-hosted asset management app designed to help users efficiently sort through a large number of 3D printer model assets. With printBed, you can organize, search, and categorize your 3D printer models, making it easier to find the right model for your printing needs.

> [!CAUTION]
> **printBed is currently in Alpha.**
>
> This project is under active development. You are welcome to use it, but please be aware that features may change or break without notice. If you encounter any bugs or issues, please report them [here](https://github.com/prosthetichead/printBed/issues).


## Features

- **Asset Organization**: Easily organize your 3D printer model assets into categories.
- **Search Functionality**: Quickly search through your collection of models using keywords or tags.
- **Customizable Tags**: Assign custom tags to your models for easy categorization and filtering.
- **User-Friendly Interface**: Intuitive interface for easy navigation and management of your assets.
- **Self-Hosted**: Keep your model assets private by hosting printBed on your own server.

## Installation
Currently printBed is only available via Docker.

Docker Compose
```yaml
services:  
  printBed:
    container_name: printBed
    image: prosthetichead/printbed:latest
    volumes:
      - [DB Path]:/appdata
      - [Print Files Path]:/print-files
    ports: 
      - 4040:8080
```
* Replace [DB Path] with local location of database for example ~/appdata/printBed
* Replace [Print Files Path] with local path for storing print files for example ~/mnt/storage/3d-prints

## Previews
https://github.com/user-attachments/assets/22264c2f-4ed2-4af3-8f90-2dab5c7ac3a2

https://github.com/user-attachments/assets/615c3851-fb94-4c9e-9519-8f827558b997

## Contributing
While this is a personal project contributions to printBed are welcome! If you have suggestions for new features, improvements, or bug fixes, please submit an issue or pull request on GitHub. 


## Acknowledgements
Icons used in this project are from [Flaticon](https://www.flaticon.com/) and were created by [freepik](https://www.flaticon.com/authors/freepik).
