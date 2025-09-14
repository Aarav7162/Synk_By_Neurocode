# Synk (Beta)

Synk is an accessible, beginner-friendly IDE developed by **NeuroCode**. It is designed to simplify Arduino programming for neurodivergent learners by providing a human-friendly syntax that translates into standard Arduino C++ code.  

## Key Features

- **Simplified Syntax**  
  Write commands in plain, intuitive language that automatically convert to `.ino` files.  

- **Arduino Integration**  
  Built-in support for compiling and uploading code to Arduino boards using Arduino CLI.  

- **Interactive Console IDE**  
  Includes line numbering, syntax highlighting, autocomplete, command suggestions, and live validation.  

- **File & Project Management**  
  - Create projects with folders, subfolders, and files (`.nec` or `.ino`).  
  - Import and export files with ease.  
  - Convert `.nec` files to Arduino-compatible `.ino` format.  

- **Help Center**  
  Searchable, categorized documentation for all commands with syntax, usage, and examples.   

- **Additional Tools**  
  - Built-in templates and command snippets.  
  - Optional live compilation preview.  

## Getting Started

1. **Install Dependencies**  
   - [Arduino CLI](https://arduino.github.io/arduino-cli)  
   - .NET 6.0 or higher  

2. **Clone the Repository**  
   ```bash
   git clone https://github.com/neurocode/Synk_By_Neurocode.git
   cd Synk_By_Neurocode

3. **Build and Run**
   ```bash
   dotnet build
   dotnet run

4. **Launch the IDE**
   Start creating `.nec` projects.

5. **Example**

   *Synk*

    ```nec
    start setup
      pinmode 13 output
    end setup

    start loop
      turn on pin 13
      wait 1 second
      turn off pin 13
      wait 1 second
    end loop

## Roadmap

- Expand the NeuroCode command set with advanced features.  
- Web-based IDE with collaborative editing.  
- Synk Academy: integrated tutorials and exercises.  

## Contributing

We welcome contributions. Please open an issue or submit a pull request for discussion before making changes.  

## License

This project is licensed under the MIT License.  

> **⚠️ Beta Notice**  
> Synk is currently in **Beta**. Features are subject to change, and some functionality may be unstable or incomplete.  
> We encourage you to try it out and share feedback, but please do not use it in production environments yet.
