# Time Series Data Management System

Welcome to the Time Series Data Management System repository! This system is built using ASP.NET Core API, xUnit, and InfluxDB to efficiently manage and analyze time series data.

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

This system is designed to handle time series data efficiently, providing functionalities to fetch data within a given time range, retrieve tag sets in a specified range, and send data to InfluxDB as a JSON file.

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
