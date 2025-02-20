# WPTools.NET

WPTools.NET: A .NET-based project for WordPress plugin development. This repository contains a set of tools for building and deploying WordPress plugins. While primarily a learning project, the tools are functional and can be used as a starting point for more complex projects.

## Features

- **Configuration-based:** Uses a `composer.json` file to define the files and directories to be copied, as well as include/exclude patterns.
- **Recursive directory copying:** Copies directories and their contents recursively.
- **Customizable:** The configuration file allows you to specify the entry point, output directory, and files to be included or excluded.

## Getting Started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/en-us/download)
- [Git](https://git-scm.com/)

### Installation

1.  Clone the repository:

    ```bash
    git clone [https://github.com/AndreaGelmini/WPTools.NET.git](https://github.com/AndreaGelmini/WPTools.NET.git)
    ```

2.  Navigate to the project directory:

    ```bash
    cd WPTools.NET
    ```

## Usage

1.  Create a `composer.json` file in the root directory of your project. Here's an example:

    ```json
    {
      "name": "your-plugin-name",
      "description": "A description of your WordPress plugin",
      "wp-build-config": {
        "entry": "/",
        "output": "dist",
        "files": ["your-plugin-file.php", "your-plugin-directory"],
        "exclude": ["node_modules", ".git"]
      }
    }
    ```

2.  Build the project to create an executable file:

    ```bash
    dotnet build wp-plugin-builder.csproj -c Release
    ```

    This will generate the output in the [net7.0](http://_vscodecontentref_/1) directory.

3.  Run the `WPPluginBuilder` from the command line, providing the output directory as an argument (optional):

    ```bash
    dotnet run --project wp-plugin-builder.csproj -- [output_directory]
    ```

    Alternatively, you can run the generated executable file directly:

    ```bash
    bin/Release/net7.0/wp-plugin-builder.exe [output_directory]
    ```

    If you don't provide an output directory, the one specified in `composer.json` will be used.

## Contributing

Contributions are welcome! Please open an issue or submit a pull request if you have any suggestions or bug fixes.

### Adding a New Tool

If you would like to add a new tool to this project, please follow these steps:

1. **Fork the repository:**

    Fork the repository to your own GitHub account.

2. **Clone your fork:**

    ```bash
    git clone https://github.com/your-username/WPTools.NET.git
    cd WPTools.NET
    ```

3. **Create a new branch:**

    Create a new branch for your tool:

    ```bash
    git checkout -b add-new-tool
    ```

4. **Implement your tool:**

    Add your new tool to the project. Make sure to follow the existing code style and structure. Update the `composer.json` file if necessary.

5. **Write tests:**

    Write unit tests for your tool to ensure it works as expected.

6. **Update documentation:**

    Update the [README.md](http://_vscodecontentref_/0) file to include information about your new tool. Add usage instructions and examples if applicable.

7. **Commit your changes:**

    Commit your changes with a descriptive commit message:

    ```bash
    git add .
    git commit -m "Add new tool: [tool-name]"
    ```

8. **Push your branch:**

    Push your branch to your forked repository:

    ```bash
    git push origin add-new-tool
    ```

9. **Create a pull request:**

    Go to the original repository on GitHub and create a pull request from your forked repository. Provide a detailed description of your changes and why they should be merged.

10. **Address feedback:**

    Be responsive to any feedback or requests for changes from the project maintainers. Make the necessary updates and push them to your branch.

Thank you for contributing to WPTools.NET!

## License

This project is licensed under the GNU General Public License v3.0. See the [LICENSE](LICENSE) file for details.

The GPL3 license ensures that this project remains open source and accessible to everyone. Any modifications or distributions of this project must also be under the GPL3 license.

## Contact

If you have any questions or comments, please feel free to contact me at github@andreagelmini.it or open a issue
