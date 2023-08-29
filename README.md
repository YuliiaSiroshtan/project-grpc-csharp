# Weather Microservice with gRPC Server and API Client

This repository contains a .NET solution that implements a weather microservice using gRPC for communication between the server and the client. The solution is divided into two projects: a gRPC server and an API client.

## Prerequisites

Before you begin, make sure you have the following prerequisites installed:

- [.NET SDK](https://dotnet.microsoft.com/download) (version X.X or higher)
- [Docker](https://www.docker.com/) (optional, for running the gRPC server in a Docker container)

## Getting Started

Follow the steps below to get the weather microservice up and running.

### 1. Clone the Repository

```bash
git clone https://github.com/YuliiaSiroshtan/project-grpc-csharp.git
cd project-grpc-csharp
```

### 2. Build the Solution

Navigate to the solution directory and build the projects:

```bash
cd project-grpc-csharp
dotnet build
```

### 3. Run the gRPC Server

You can choose to run the gRPC server directly or within a Docker container.

#### Option 1: Run the Server Directly

Navigate to the gRPC server project and run the application:

```bash
cd WeatherServer
dotnet run
```

#### Option 2: Run the Server in Docker

If you prefer to run the server in a Docker container, use the provided Dockerfile. Build the Docker image and run a container:

```bash
cd WeatherServer
docker build -t weather-server .
docker run -p 50051:50051 weather-server
```

### 4. Run the API Client

Navigate to the API client project and run the application:

```bash
cd ApiClient
dotnet run
```

The API client project includes a controller with CRUD operations that communicate with the gRPC server using a gRPC client.

## Usage

Once the server and client are running, you can use the API client to interact with the weather microservice. The API client provides endpoints for performing CRUD operations on weather data.

- To retrieve weather data, make a GET request to `/weather`.
- To create new weather data, make a POST request to `/weather` with a JSON payload.
- To update weather data, make a PUT request to `/weather` with a JSON payload.
- To delete weather data, make a DELETE request to `/weather/{id}`.

Replace `{id}` with the desired weather data ID.

## Contributing

Contributions are welcome! If you find any issues or want to enhance the functionality of the microservice, feel free to submit a pull request.

## License

This project is licensed under the [MIT License](LICENSE).

---
