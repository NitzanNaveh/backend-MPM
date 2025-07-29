# ProjectManager.Api

A .NET 8 Web API project for managing projects and tasks with JWT authentication.

## Project Structure

```
ProjectManager.Api/
├── Controllers/
│   ├── AuthController.cs          # Authentication endpoints
│   ├── ProjectsController.cs      # Project CRUD operations
│   └── TasksController.cs         # Task CRUD operations
├── Dtos/
│   ├── Auth/
│   │   ├── RegisterRequest.cs     # User registration DTO
│   │   └── LoginRequest.cs        # User login DTO
│   ├── Projects/
│   │   ├── CreateProjectDto.cs    # Project creation DTO
│   │   └── ProjectResponseDto.cs  # Project response DTO
│   └── Tasks/
│       ├── CreateTaskDto.cs       # Task creation DTO
│       └── TaskResponseDto.cs     # Task response DTO
├── Entities/
│   ├── User.cs                    # User entity
│   ├── Project.cs                 # Project entity
│   └── TaskItem.cs                # Task entity
├── Services/
│   ├── AuthService.cs             # Authentication service
│   ├── ProjectsService.cs         # Project business logic
│   └── TasksService.cs            # Task business logic
├── Data/
│   ├── AppDbContext.cs            # Entity Framework context
│   └── Migrations/                # Database migrations (auto-generated)
├── Middleware/
│   └── JwtMiddleware.cs           # JWT token validation
├── Properties/
│   └── launchSettings.json        # Development configuration
├── Program.cs                     # Application entry point
├── appsettings.json               # Application configuration
└── ProjectManager.Api.csproj      # Project file
```

## Features

- **JWT Authentication**: Secure user authentication with JWT tokens
- **Entity Framework Core**: Database access with SQL Server
- **Dependency Injection**: Proper service registration and injection
- **RESTful API**: Standard REST endpoints for all operations
- **Data Validation**: Input validation using Data Annotations
- **Error Handling**: Proper error responses and exception handling
- **CORS Support**: Cross-origin resource sharing enabled
- **Swagger Documentation**: API documentation with Swagger UI

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code

### Installation

1. Clone the repository
2. Navigate to the project directory
3. Update the connection string in `appsettings.json`
4. Run the following commands:

```bash
# Restore packages
dotnet restore

# Create database migrations
dotnet ef migrations add InitialCreate

# Apply migrations to database
dotnet ef database update

# Run the application
dotnet run
```

### Configuration

Update the following settings in `appsettings.json`:

- **Connection String**: Update the database connection string
- **JWT Settings**: Modify JWT key, issuer, and audience for production
- **Logging**: Configure logging levels as needed

## API Endpoints

### Authentication

- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Login user
- `POST /api/auth/validate` - Validate JWT token

### Projects

- `GET /api/projects` - Get all projects for authenticated user
- `GET /api/projects/{id}` - Get project by ID
- `POST /api/projects` - Create new project
- `PUT /api/projects/{id}` - Update project
- `DELETE /api/projects/{id}` - Delete project

### Tasks

- `GET /api/tasks` - Get all tasks for authenticated user
- `GET /api/tasks/project/{projectId}` - Get tasks by project
- `GET /api/tasks/{id}` - Get task by ID
- `POST /api/tasks` - Create new task
- `PUT /api/tasks/{id}` - Update task
- `DELETE /api/tasks/{id}` - Delete task

## Authentication

All endpoints except authentication endpoints require a valid JWT token in the Authorization header:

```
Authorization: Bearer <your-jwt-token>
```

## Database Schema

### Users
- Id (Primary Key)
- FirstName
- LastName
- Email (Unique)
- PasswordHash
- CreatedAt
- UpdatedAt

### Projects
- Id (Primary Key)
- Title
- Description
- OwnerId (Foreign Key to Users)
- CreatedAt

### Tasks
- Id (Primary Key)
- Title
- DueDate
- Status (IsCompleted)
- ProjectId (Foreign Key to Projects)
- CreatedAt# backend-MPM
