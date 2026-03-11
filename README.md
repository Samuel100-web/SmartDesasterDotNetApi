### API Screenshot (Part 1)
<img width="2048" height="15050" alt="apiscreenshoot" src="https://github.com/user-attachments/assets/eee2521c-a8fb-4a21-ae83-c69a9e55c4a2" />
This is the backend API service for the **SmartDesaster** platform, a disaster management and emergency response system. It is built using a clean architecture with .NET.

## Tech Stack
- **Framework:** .NET 10.0 (ASP.NET Core API)
- **Database:** SQL Server / Entity Framework Core
- **Architecture:** Repository Pattern & Unit of Work
- **Authentication:** JWT (JSON Web Tokens)
- **Tooling:** Swagger/OpenAPI for documentation

## Features
- **Identity Management:** User registration, login, and role-based access control (NGOs, Admins, Citizens).
- **Incident Reporting:** Create, update, and track disaster incidents.
- **NGO Integration:** Manage NGO registrations and disaster response setups.
- **Resource Management:** Track emergency resources and categories.
- **Donation Tracking:** Manage donations and resource distribution.
- **External Sync:** Background workers for syncing disaster data.

## Project Structure
The solution consists of three main projects:
- `SmartResponse.API`: The entry point, controllers, and configuration.
- `SmartResponse.Core`: Entities, DTOs, interfaces, and business logic.
- `SmartResponse.Infrastructure`: Data access (AppDbContext), migrations, and service implementations.

## How to Run Locally
1. Clone the repository.
2. Update the `ConnectionStrings` in `appsettings.json`.
3. Run `dotnet ef database update` in the Infrastructure project.
4. Run `dotnet run --project SmartResponse.API`.
