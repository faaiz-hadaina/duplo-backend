# Node.js to C# Conversion Notes

This document outlines the conversion from Node.js/Express backend to C# ASP.NET Core.

## Overview

The project has been successfully converted from a Node.js/Express application to a C# ASP.NET Core 9.0 application while maintaining the same functionality and API structure.

## Conversion Mapping

### 1. **Entry Point**
- **Node.js**: `index.js`
- **C#**: `Program.cs`

### 2. **Models**
| Node.js (Mongoose) | C# (MongoDB.Driver) |
|-------------------|---------------------|
| `app/modules/business/business.model.js` | `Models/Business.cs` |
| `app/modules/department/department.model.js` | `Models/Department.cs` |
| `app/modules/transaction/transaction.model.js` | `Models/Transaction.cs` |

### 3. **Controllers**
| Node.js | C# |
|---------|-----|
| `app/modules/auth/business/business.controller.js` | `Controllers/BusinessAuthController.cs` |
| `app/modules/auth/department/department.controller.js` | `Controllers/DepartmentAuthController.cs` |
| `app/modules/order/order.controller.js` | `Controllers/OrderController.cs` |

### 4. **Services**
| Node.js | C# |
|---------|-----|
| `app/modules/order/order.service.js` | `Services/OrderService.cs` |
| `app/modules/auth/auth.service.js` (implicit) | `Services/AuthService.cs` |
| `app/modules/auth/business/business.service.js` | Integrated into `Middleware/JwtAuthorizationHandler.cs` |
| `app/modules/auth/department/department.service.js` | Integrated into `Middleware/JwtAuthorizationHandler.cs` |

### 5. **Utilities**
| Node.js | C# |
|---------|-----|
| `app/utils/calculateScore.utils.js` | `Utils/CalculateScoreUtils.cs` |
| `app/utils/generateRandom.utils.js` | `Utils/GenerateRandomUtils.cs` |
| `app/service/winston.service.js` | Built-in `ILogger<T>` interface |

### 6. **Middleware & Configuration**
| Node.js | C# |
|---------|-----|
| JWT middleware (custom) | `Microsoft.AspNetCore.Authentication.JwtBearer` |
| `express-rate-limit` | `AspNetCoreRateLimit` |
| CORS configuration | Built-in CORS middleware |
| `.env` file | `appsettings.json` |

## Technology Stack Changes

### Removed (Node.js)
- Express.js
- Mongoose (MongoDB ODM)
- Morgan (logging)
- Winston (logging)
- bcrypt
- jsonwebtoken
- cors
- express-rate-limit
- express-paginate

### Added (C#)
- ASP.NET Core 9.0
- MongoDB.Driver (official C# driver)
- Microsoft.Extensions.Logging (built-in)
- BCrypt.Net-Next
- Microsoft.AspNetCore.Authentication.JwtBearer
- AspNetCoreRateLimit

## Key Architectural Changes

1. **Dependency Injection**: C# uses built-in DI container, configured in `Program.cs`

2. **Authentication**: 
   - Node.js used custom middleware
   - C# uses policy-based authorization with custom authorization handlers

3. **Logging**: 
   - Node.js used Winston for structured logging
   - C# uses built-in `ILogger<T>` interface

4. **Database Access**:
   - Node.js used Mongoose ODM with schema definitions
   - C# uses MongoDB.Driver with POCO classes and attributes

5. **Configuration**:
   - Node.js used `.env` file with dotenv package
   - C# uses `appsettings.json` with built-in configuration system

6. **Async/Await**:
   - Both use async/await pattern
   - C# uses `Task<T>` return types
   - Node.js uses Promises

## API Endpoints (Unchanged)

All API endpoints remain the same:

- `POST /api/department/register`
- `POST /api/department/login`
- `POST /api/business/register`
- `POST /api/business/login`
- `GET /api/business`
- `POST /api/orders`
- `GET /api/credit-score`
- `GET /api/order-details/{businessID}`

## Configuration Files

### Node.js
```
.env
package.json
```

### C#
```
appsettings.json
appsettings.Development.json
DuploBackend.csproj
```

## Running the Application

### Node.js
```bash
npm install
npm run dev
```

### C#
```bash
dotnet restore
dotnet run
# or for development
dotnet watch run
```

## Deployment

### Node.js
Deploy to Node.js hosting service (Heroku, AWS, etc.)

### C#
```bash
dotnet publish -c Release -o ./publish
```
Then deploy the `publish` folder to any server with .NET 9.0 runtime.

## Benefits of C# Version

1. **Type Safety**: Strong typing throughout the application
2. **Performance**: Generally faster than Node.js for CPU-intensive operations
3. **Tooling**: Better IDE support with IntelliSense
4. **Ecosystem**: Access to vast .NET ecosystem
5. **Enterprise Support**: Better suited for enterprise environments
6. **Built-in Features**: More features included out-of-the-box (DI, configuration, logging)

## Maintained Functionality

All original functionality has been preserved:
- User registration and login (both business and department)
- JWT authentication
- Order processing
- Credit score calculation
- Order details retrieval
- Pagination support
- Rate limiting
- CORS support
- MongoDB integration
