# Tutorial and sample codes for ASP.NET 6 SPA with React

## Migrating from .NET 6

The code base is from this repository for [.NET 5 SPA](https://github.com/charlehsin/net5-react-tutorial). Then we follow this [MSDN guide](https://docs.microsoft.com/en-us/aspnet/core/migration/50-to-60?view=aspnetcore-6.0&tabs=visual-studio-code) to migrate this to .NET 6.

You can find out the changes done during the migration from the commit history.

To get the installed .NET versions

- dotnet --info

## Overview

This follows the following to create the default template codes.

- [React project template](https://docs.microsoft.com/en-us/aspnet/core/client-side/spa/react?view=aspnetcore-6.0&tabs=netcore-cli)

You can check the following for more detailed explanations.

- [Creating SPA using ASP.NET Core and React](https://dev.to/packt/creating-spas-using-asp-net-core-and-react-59a0)

The folder structure is the following

- my-new-app \ ClientApp folder: This is the React app. The build output in build folder will be served.
- my-new-app \ Controllers folder: This has a dummy Web API that the React app can send requests to.
   - To pass any data from the backend to the frontend, one should use the Web API. If the data is needed before the authentication, then one can create an API path without the need for authentication to pass the data that do not need to be protected.
- my-new-app \ Pages folder: This has some standard ASP.NET Razor pages.
- my-new-app \ Program.cs: This is the standard ASP.NET file.
- my-new-app \ Startup.cs: This specifies that this is serving the SPA static files from CilentApp \ build folder. This also sets up the Web API.
- my-new-app \ WeatherForcast.cs: This has the model used by the dummy Web API.

After the app is running, open https://localhost:5001 to see the web site.

## GitHub Actions included

- DOTNET build and test
- CodeQL

## Useful Visual Studio Code extensions

- [C# extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp)
- [Git history extension](https://marketplace.visualstudio.com/items?itemName=donjayamanne.githistory)

## Useful Visual Studio Code references

- [.NET console application using VS Code](https://docs.microsoft.com/en-us/dotnet/core/tutorials/with-visual-studio-code#debug)
- [.NET CLI](https://docs.microsoft.com/en-us/dotnet/core/tools/)
- [XML auto commenting in VS Code](https://stackoverflow.com/questions/34275209/xml-auto-commenting-c-sharp-in-visual-studio-code)
- [Go to definition in VS Code](https://stackoverflow.com/questions/47995468/vscode-c-sharp-go-to-definition-f12-not-working)
- [Roslyn in VS Code](https://www.strathweb.com/2019/04/roslyn-analyzers-in-code-fixes-in-omnisharp-and-vs-code/)

## Useful .NET CLI commands

- (Create the default template for backend and frontend) dotnet new react -o my-new-app
- (Powershell) SET ASPNETCORE_ENVIRONMENT=Development
- (in my-new-app folder) dotnet build
- (in my-new-app folder) dotnet run
