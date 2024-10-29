# WeatherForecastAPI

This project is a .NET 8 Web API for storing and retrieving weather forecasts based on latitude and longitude. Application leverages minimal APIs, uses Docker and Docker Compose to orchestrate the application, test service, and SQL Server database.

## Prerequisites

- [.NET SDK 8.0+](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://docs.docker.com/compose/)

## Project Structure

```
/project-root
├── Dockerfile.api                # Dockerfile for the application
├── Dockerfile.tests              # Dockerfile for running tests
├── docker-compose.yml            # Docker Compose configuration
├── WeatherForecastAPI/           # Main application source code
│   ├── WeatherForecastAPI.csproj
│   └── other files...
├── WeatherForecastAPI.Tests/     # Test project source code
│   ├── WeatherForecastAPI.Tests.csproj
│   └── other files...
└── WeatherForecastAPI.sln        # Solution file
```

## Configuration

### Application Settings

The application’s port, logging levels, and database connection strings are configured in `appsettings.json`. Docker Compose overrides the connection string to connect to the SQL Server container.

### Ports

The application listens on:

- HTTP: Port 5000

You can change these ports in `appsettings.json` or in `docker-compose.yml`.

### Environment Variables

Environment variables can be set in `docker-compose.yml`:

- `ASPNETCORE_ENVIRONMENT`: Set to `Production` or `Development`.
- `ConnectionStrings__SqlServerConnection`: SQL Server connection string for the app.

## Running the Application

1. **Build and Start the Application**:

   Run the following command to build and start the app, tests, and database:

   ```bash
   docker-compose up --build
   ```

   This command will:

   - Build the application and test images.
   - Start the application on port `5000` (HTTP).
   - Start a SQL Server container for database storage.

2. **Access the Application**:

   - Swagger UI: [http://localhost:5000/swagger](http://localhost:5000/swagger)
   - API Base URL: [http://localhost:5000](http://localhost:5000)

3. **Running Tests**:

   To run the tests separately, use the following command:

   ```bash
   docker-compose run tests
   ```

   Test results are outputted in `.trx` format in the `TestResults` directory on the host machine.

4. **Stop the Application**:

   Use the following command to stop and remove all services:

   ```bash
   docker-compose down
   ```

## Database

The application uses SQL Server as its database, running in a Docker container defined in `docker-compose.yml`.

### Database Connection String

In `docker-compose.yml`, the `ConnectionStrings__SqlServerConnection` environment variable is set to connect to the `mssql` service, using the default SQL Server credentials:

```yaml
environment:
  - ConnectionStrings__SqlServerConnection=Server=mssql;Database=WeatherForecastDb;User Id=sa;Password=DEUQN-PvR-YB9lDGtaKElA;
```

You can modify these credentials as needed.

## Accessing Swagger

Swagger UI is enabled to test and interact with the API. Once the app is running, you can access Swagger UI at:

- **HTTP**: [http://localhost:5000/swagger](http://localhost:5000/swagger)

Swagger will display available endpoints and allow you to interact with the API directly from the browser.

## Troubleshooting

- **Error: Cannot connect to the database**:
  Ensure the `mssql` container is running and that `ConnectionStrings__SqlServerConnection` is correctly configured.
- **Port Conflict**:
  If port 5000 is already in use, update the `docker-compose.yml` to use different ports for `api`.

## Additional Commands

### Build and Start Containers without Logs

```bash
docker-compose up -d --build
```

### View Application Logs

```bash
docker-compose logs app
```

### Rebuild Only the App Container

```bash
docker-compose up --build app
```

## License

This project is licensed under the MIT License. See the LICENSE file for details.

---

### Author

Jakub Jelonek
