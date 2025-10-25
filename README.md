# HouseHelp Backend

Comprehensive ASP.NET Core Web API for housing-society house-help management.

## Getting Started

### Prerequisites
- .NET SDK 8.0
- Docker (for Postgres/Redis/Seq/Hangfire dashboard)

### Run infrastructure
```
docker compose up -d
```

### Apply EF Core migrations
```
dotnet tool install --global dotnet-ef
dotnet ef database update --project src/Infrastructure --startup-project src/Api
```

### Run tests
```
dotnet test
```

### Run API
```
dotnet run --project src/Api
```

API documentation available at `/swagger` once the app is running.

SignalR hub is exposed at `/ws`.

### Resident UI demo

A production-inspired resident-facing dashboard is bundled with the API host. After starting the API with the commands above, open [https://localhost:5001/ui/resident-dashboard.html](https://localhost:5001/ui/resident-dashboard.html) (or the HTTP equivalent if TLS is disabled).

- Request an OTP for the seeded resident `+910000000001` and verify with `123456`.
- Choose the pre-provisioned flat and search for helpers using the controls on the page.
- Book a helper directly from the table – the screen talks to the same REST APIs consumed by the mobile app clients.

The UI automatically refreshes JWTs and displays the booking payload returned by the backend.

## Configuration

Sample configuration is provided in `src/Api/appsettings.json`.

## Project Layout
- `src/Domain` – Entities, enums, repository and domain service contracts.
- `src/Application` – Business services, policies, validators.
- `src/Infrastructure` – EF Core DbContext, repositories, Redis locks, fake OTP and Razorpay implementations, Hangfire jobs.
- `src/Realtime` – SignalR hub definitions.
- `src/Contracts` – API DTOs.
- `src/Api` – ASP.NET Core host, controllers, Program wiring.
- `tests` – Unit and integration tests.

## TODOs
- Secure Hangfire dashboard with proper authorization.
- Replace fake OTP and Razorpay providers with production integrations.
- Implement pricing engine and invoice generation.
