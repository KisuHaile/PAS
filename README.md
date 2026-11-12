# PAS


go to master branch if you want to see the role that you are going to do

and after that you can go to your branch and push what you do in that


PAS is a Property and Asset Management System built for managing inventory, properties, stock operations, requisitions, and user roles in a unified platform.

This repository contains two main parts:

- `PAS/` — the backend API built with ASP.NET Core and .NET 9
- `PAS-Frontend/` — the Angular web application for the user interface

## Overview

PAS supports operational and administrative workflows for organizations that need to:

- manage assets and properties
- track warehouse and shelf locations
- monitor stock levels and inventory movements
- process requisitions and issuing workflows
- manage users, roles, and permissions
- review audit activity and reports

## Key Features

- Authentication and user management
- Role-based access control
- Employee and user profile management
- Property and asset tracking
- Inventory and stock monitoring
- Warehouse and storage management
- Supplier and receiving management
- Store requisition and issue processing
- Approval workflows
- Reporting and dashboard views
- Audit trail support

## Repository Structure

```text
PAS/
├── PAS.API/                 # ASP.NET Core API project
├── PAS.Application/         # Application layer, business logic, commands, DTOs
├── PAS.Domain/              # Core entities and domain logic
├── PAS.Infrastructure/      # Infrastructure concerns
├── PAS.Persistence/         # EF Core DbContext, migrations, and seed data
├── PAS.Tests/               # Test projects (if present in your branch)
├── README.md                # Backend project documentation
├── PAS.sln                  # Solution file
└── ...

PAS-Frontend/
├── src/                     # Angular application source
├── public/                  # Static public assets
├── angular.json             # Angular workspace config
├── package.json             # Frontend package metadata
├── proxy.conf.json          # Local API proxy setup
├── tsconfig.json            # TypeScript config
├── README.md                # Frontend documentation
└── ...
```

## Technology Stack

### Backend

- .NET 9
- ASP.NET Core Web API
- Entity Framework Core
- ASP.NET Core Identity
- MediatR
- FluentValidation
- SQL Server / relational database support

### Frontend

- Angular 21
- TypeScript
- RxJS
- Bootstrap
- Primeng
- Chart libraries for dashboard reporting

## Prerequisites

Before running the project locally, install:

- .NET 9 SDK
- Node.js 20+ and npm
- SQL Server (or another supported relational database)
- Git

## Backend Setup

From the repository root:

```bash
dotnet restore
cd PAS

dotnet run --project PAS.API
```

This starts the API server. By default, it is typically available at:

- http://localhost:5000
- or the configured ASP.NET launch URL for your environment

If the project uses database migrations or seed data, they will usually run through the persistence layer when the app starts.

## Frontend Setup

Open a separate terminal and run:

```bash
cd PAS-Frontend
npm install
npm start
```

This starts the Angular app, usually on:

- http://localhost:4200

## Building the Frontend

```bash
cd PAS-Frontend
npm run build
```

## Running Tests

Frontend:

```bash
cd PAS-Frontend
npm test
```

Backend test commands depend on the solution/test projects present in your local branch.

## Default Roles

The application includes seed logic for user roles such as:

- Admin
- Manager
- StoreKeeper
- Employee

You can extend the role list or permissions based on your business rules in the application and persistence projects.

## Development Notes

- The backend follows a layered architecture using Domain, Application, Persistence, and API concerns.
- The frontend is a standalone Angular client that communicates with the backend API.
- Role and permission logic are central to access control and should be kept consistent across both backend validation and frontend views.

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Commit and push
5. Open a pull request for review

## License

This project does not currently declare a license in the repository metadata. If you want to publish or distribute it publicly, add a license file and update this section accordingly.

## Project Status

PAS is a working application repository with both backend and frontend components, structured for property, asset, store, and workflow management workflows.
