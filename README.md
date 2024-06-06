# Forumcord

Forumcord is a WPF-based desktop application built with .NET 8, designed to provide a Discord-like experience for interacting with various forum software (XenForo, myBB, phpBB) through an embedded Chromium browser.

## Table of Contents
- [Features](#features)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Usage](#usage)
- [Project Structure](#project-structure)
- [Customization](#customization)
- [Contributing](#contributing)
- [License](#license)

## Features

- **Embedded Chromium Browser:** Uses CefSharp for displaying and interacting with forums.
- **Custom Forum Sources:** Add, edit, and manage multiple forum sources with custom icons.
- **Persistent Data:** Maintains cookies, history, and other browsing data across sessions.
- **Customizable Alerts:** Collects and displays user alerts from forums.
- **Context Menus:** Custom context menus for managing forum sources.
- **Favicon Fetching:** Automatically downloads and caches favicons for forums.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (or any other IDE supporting .NET 8 and WPF)
- [CefSharp.Wpf](https://github.com/cefsharp/CefSharp) NuGet package

## Installation

### Using Visual Studio

1. **Clone the Repository:**
    ```bash
    git clone https://github.com/tcgm/Forumcord.git
    cd Forumcord
    ```

2. **Open the Project in Visual Studio:**
    - Launch Visual Studio.
    - Select `File` > `Open` > `Project/Solution`.
    - Navigate to the cloned repository folder and open `Forumcord.sln`.

3. **Restore NuGet Packages:**
    - Visual Studio should automatically restore the required NuGet packages when the solution is opened. If not, right-click on the solution in the Solution Explorer and select `Restore NuGet Packages`.

4. **Build the Project:**
    - Press `Ctrl+Shift+B` to build the solution or go to `Build` > `Build Solution`.

5. **Run the Application:**
    - Press `F5` to run the application or click on the `Start` button.

### Using .NET CLI

1. **Clone the Repository:**
    ```bash
    git clone https://github.com/tcgm/Forumcord.git
    cd Forumcord
    ```

2. **Install Dependencies:**
    Restore the NuGet packages required by the project:
    ```bash
    dotnet restore
    ```

3. **Build the Project:**
    Build the project to ensure all dependencies are correctly configured:
    ```bash
    dotnet build
    ```

4. **Run the Application:**
    Start the application:
    ```bash
    dotnet run
    ```

## Usage

1. **Add Forum Sources:**
    - Use the interface to add new forum sources by specifying the name, URL, and optionally, an icon path.
    - Right-click on a source to edit or remove it.

2. **View Alerts:**
    - Alerts from forums are collected in the background.
    - Open the Alerts panel to view all collected alerts, with new ones highlighted.

## Project Structure

- `Forumcord/`
  - `App.xaml` / `App.xaml.cs`: Application entry point.
  - `MainWindow.xaml` / `MainWindow.xaml.cs`: Main window containing the browser and source list.
  - `SourceEntryControl.xaml` / `SourceEntryControl.xaml.cs`: Custom control for displaying forum sources.
  - `EditSourceWindow.xaml` / `EditSourceWindow.xaml.cs`: Window for editing source properties.
  - `AlertsControl.xaml` / `AlertsControl.xaml.cs`: UserControl for displaying user alerts.
  - `Models/Source.cs`: Model representing a forum source.
  - `Helpers/`: Contains helper classes and methods (e.g., for downloading favicons).

## Customization

- **Default Icons:**
  - The default icon for sources is `forumwhite.png`. You can replace this with your own image.
  - Ensure the image is set as an Embedded Resource in the project properties.

- **Context Menus:**
  - Customize the context menu by editing the `OpenContextMenu` method in `SourceEntryControl.xaml.cs`.

- **Alert Collection:**
  - Modify `AlertsControl.xaml.cs` to customize how alerts are collected and displayed.
  - Update the alert URLs and parsing logic as necessary for different forum software versions.

## Contributing

1. **Fork the Repository:**
    - Create your own fork of the repository on GitHub.

2. **Create a Branch:**
    - Create a new branch for your feature or bugfix.
    ```bash
    git checkout -b feature/your-feature
    ```

3. **Commit Your Changes:**
    - Commit your changes with a descriptive message.
    ```bash
    git commit -am 'Add new feature'
    ```

4. **Push to Your Fork:**
    - Push the branch to your forked repository.
    ```bash
    git push origin feature/your-feature
    ```

5. **Submit a Pull Request:**
    - Open a pull request on the original repository.

## License

This project is licensed under the GNU v3.0 License. See the LICENSE file for details.