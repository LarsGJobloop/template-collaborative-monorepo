# Example Web API Service

An example ASP.NET API service with health check endpoint.

## Getting Started

From the repository root, navigate to this folder:

```bash
cd src/example-web-api-service
```

Restore dependencies:

```bash
dotnet restore
```

Start the API:

```bash
dotnet run --project Api
```

The API will be available at:
- HTTP: [http://localhost:5285](http://localhost:5285)
- HTTPS: [https://localhost:7141](https://localhost:7141)

The health check endpoint is available at `/healthz`.

## Running Tests

Run the test suite:

```bash
dotnet test
```
