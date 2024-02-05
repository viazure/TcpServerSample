# TCPServerSample

This project is a learning exercise to implement a simple TCP service using .NET 8. The main goal of the project is to understand the basics of TCP communication within the .NET ecosystem.

## Features

- **TCP Server**: A basic TCP server that listens for client connections and handles incoming data.
- **Logging**: Utilizes Serilog for detailed and structured logging.
- **Configuration**: Reads the service startup port and the encoding format for received messages from a configuration file.

## Getting Started

To get started with this project, clone the repository and open it in your preferred .NET IDE (such as Visual Studio).

```bash
git clone https://github.com/viazure/TcpServerSample.git
```

Make sure you have .NET 8 installed on your machine. You can download it from the [official .NET website](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).

Once the project is open, you can run the service by executing:

```bash
dotnet run
```

## Dependencies

- [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0): The framework used for building the service.
- [Serilog](https://serilog.net/): A logging library providing simple, structured logging.
