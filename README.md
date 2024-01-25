# Time Series Data Management System

Welcome to the Time Series Data Management System repository! This system is built using ASP.NET Core API, React.js, xUnit, and InfluxDB to efficiently manage and analyze time series data.

## Table of Contents
- [Introduction](#introduction)
- [Features](#features)
- [Installation](#installation)
- [Configuration](#configuration)
- [Usage](#usage)
- [API Endpoints](#api-endpoints)
  - [InfluxFetcherController](#influxfetchercontroller)
  - [InfluxTagsController](#influxtagscontroller)
  - [InfluxWriterController](#influxwritercontroller)
- [Contributing](#contributing)
- [License](#license)

## Introduction

This system is designed to handle time series data efficiently, providing functionalities to fetch data within a given time range, retrieve tag sets in a specified range, and send data to InfluxDB as a JSON file. The combination of ASP.NET Core API and React.js provides a robust and user-friendly interface for managing time series data.

## Features

- **InfluxFetcherController**: Fetch time series data within a given time range.
- **InfluxTagsController**: Retrieve tag sets in a specified time range.
- **InfluxWriterController**: Send time series data to InfluxDB using a JSON file.

## Installation

To install and run this system locally, follow these steps:

1. Clone the repository:

    ```bash
    git clone https://github.com/your-username/time-series-management-system.git
    ```

2. Navigate to the project directory:

    ```bash
    cd time-series-management-system
    ```

3. Install dependencies for both the ASP.NET Core API and the React.js frontend:

    ```bash
    cd TimeSeriesApi
    dotnet restore

    cd ../TimeSeriesClient
    npm install
    ```

## Configuration

Before running the system, make sure to configure the InfluxDB connection settings in the `appsettings.json` file in the `TimeSeriesApi` project.

```json
"InfluxDb": {
  "Url": "http://localhost:8086",
  "Database": "your_database_name",
  "Username": "your_username",
  "Password": "your_password"
}
```

## Usage

1. Run the ASP.NET Core API:

    ```bash
    cd TimeSeriesApi
    dotnet run
    ```

2. Run the React.js frontend:

    ```bash
    cd ../TimeSeriesClient
    npm start
    ```

Visit `http://localhost:3000` in your browser to access the application.

## API Endpoints

### InfluxFetcherController

#### `GET /api/influx/fetch`

Fetch time series data within a given time range.

Parameters:

- `startTime` (required): Start time for data retrieval.
- `endTime` (required): End time for data retrieval.

Example:

```bash
GET /api/influx/fetch?startTime=2024-01-01T00:00:00&endTime=2024-01-10T00:00:00
```

### InfluxTagsController

#### `GET /api/influx/tags`

Retrieve tag sets in a specified time range.

Parameters:

- `startTime` (required): Start time for tag retrieval.
- `endTime` (required): End time for tag retrieval.

Example:

```bash
GET /api/influx/tags?startTime=2024-01-01T00:00:00&endTime=2024-01-10T00:00:00
```

### InfluxWriterController

#### `POST /api/influx/write`

Send time series data to InfluxDB using a JSON file.

Example:

```bash
POST /api/influx/write
Content-Type: application/json

{
  "data": [
    {"timestamp": "2024-01-01T12:00:00", "value": 25.5},
    {"timestamp": "2024-01-01T13:00:00", "value": 30.2},
    {"timestamp": "2024-01-01T14:00:00", "value": 22.8}
  ]
}
```

## Contributing

If you would like to contribute to this project, please follow our [contribution guidelines](CONTRIBUTING.md).

## License

This project is licensed under the [MIT License](LICENSE).
