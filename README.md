# Time Series Data Management System (API)

Welcome to the Time Series Data Management System repository! This system is built using ASP.NET Core 7 Web API, xUnit, and InfluxDB to efficiently manage sending and retrieving time series data with InfluxDB.

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

## Introduction

This system is designed to handle time series data efficiently, providing functionalities to fetch data within a given time range, retrieve tag sets in a specified range, and seamlessly transmitting any form of data to InfluxDB in the form of a JSON file.

## Features

- **InfluxFetcherController**: Fetch time series data within a given time range.
- **InfluxTagsController**: Retrieve tag sets in a specified time range.
- **InfluxWriterController**: Send time series data to InfluxDB using a JSON file.

## Installation

To install and run this system locally, follow these steps:

1. Clone the repository:

    ```bash
    git clone https://github.com/ELHabibA/TimeSeriesData-Backend.git
    ```

2. Navigate to the project directory:

    ```bash
    cd TimeSeriesData
    ```

3. Install dependencies for the ASP.NET Core API:

    ```bash
    cd TimeSeriesData
    dotnet restore


4. Prepare and run InfluxDB locally:

   - Download and install InfluxDB from the official website: [InfluxDB Downloads](https://docs.influxdata.com/influxdb/v2/install/).
   
   - Start InfluxDB using the appropriate command for your operating system.

     For example, on Unix-based systems:

     ```bash
     influxd
     ```

     On Windows:

     ```cmd
     influxd.exe
     ```

   - Create a database for your project. You can do this using the InfluxDB command line or the web-based InfluxDB UI.
  

## Configuration

Before running the system, make sure to configure the InfluxDB connection settings in the `secret.json` file in the `TimeSeriesData/Functions`.

```json
{
  "InfluxDB": {
      "ServerUrl": "http://localhost:8086/",
      "Token": "your-influxdb-token"
  }
}
```

## Usage

 Run the ASP.NET Core API:

    ```bash
    cd TimeSeriesData
    dotnet run
    ```

## API Endpoints

### InfluxFetcherController

#### `GET /api/InfluxFetcher`

Fetch time series data within a given time range.

### InfluxTagsController

#### `GET /api/InfluxTags`

Retrieve tag sets in a specified time range.

### InfluxWriterController

#### `POST /api/influxWrite`

Send time series data to InfluxDB using a JSON file.

Example:

```bash
POST /api/influxWrite
Content-Type: application/json

  [
  {
    "measurement": "string",
    "fields": {
      "additionalProp1": "string",
      "additionalProp2": "string",
      "additionalProp3": "string"
    },
    "tags": {
      "additionalProp1": "string",
      "additionalProp2": "string",
      "additionalProp3": "string"
    },
    "timestamp": "2024-01-25T21:50:48.225Z"
  }
]
```
