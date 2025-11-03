# Technical Lead Assessment - C# ASP.NET Core Backend

This is a C# ASP.NET Core backend for a Technical Lead Assessment project to manage orders. The system allows businesses to manage their orders and provides pagination for efficiently handling of large datasets. This project has been converted from Node.js/Express to C# ASP.NET Core.

## Table of Contents

- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
- [Backend](#backend)
  - [Technology Stack](#technology-stack)
  - [Project Structure](#project-structure)
  - [Pagination](#pagination)
- [API Documentation](#api-documentation)
- [Deployment](#deployment)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

## Getting Started

These instructions will help you set up and run the project on your local machine for development and testing purposes.

### Prerequisites

Before you begin, ensure you have met the following requirements:

- **.NET SDK 9.0 or later**: You need to have .NET SDK installed. You can download it from [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download).

- **MongoDB**: MongoDB is used as the database for this project. You can download and install it from [https://www.mongodb.com/try/download/community](https://www.mongodb.com/try/download/community).

### Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/faaiz-hadaina/duplo-backend.git
   ```

2. Navigate to the project directory:

   ```bash
   cd duplo-backend
   ```

3. Restore the required NuGet packages:

   ```bash
   dotnet restore
   ```

4. Configure the application settings:

   Update `appsettings.json` with your MongoDB connection string and other configuration settings:

   ```json
   {
     "MongoDbSettings": {
       "ConnectionString": "your-mongodb-connection-string",
       "DatabaseName": "your-database-name"
     },
     "JwtSettings": {
       "Secret": "your-jwt-secret-key",
       "Issuer": "DuploBackend",
       "Audience": "DuploBackend"
     }
   }
   ```

5. Run the application:

   ```bash
   dotnet run
   ```

   The backend will start on `http://localhost:5000` (or the port specified in `launchSettings.json`).

   For development with auto-reload:
   
   ```bash
   dotnet watch run
   ```

## Backend

The backend of the Order Management System is built using C# and ASP.NET Core. It handles order management, authentication, pagination, and interacts with a MongoDB database to store order data.

### Technology Stack

- **ASP.NET Core 9.0**: Web framework
- **MongoDB.Driver**: MongoDB database driver
- **JWT Bearer Authentication**: Token-based authentication
- **BCrypt.Net**: Password hashing
- **AspNetCoreRateLimit**: API rate limiting

### Project Structure

```
DuploBackend/
├── Controllers/          # API Controllers
│   ├── BusinessAuthController.cs
│   ├── DepartmentAuthController.cs
│   └── OrderController.cs
├── Models/              # Data models
│   ├── Business.cs
│   ├── Department.cs
│   ├── Transaction.cs
│   └── DTOs/           # Data Transfer Objects
├── Services/           # Business logic services
│   ├── AuthService.cs
│   └── OrderService.cs
├── Data/               # Database context
│   ├── MongoDbContext.cs
│   └── MongoDbSettings.cs
├── Middleware/         # Custom middleware
│   └── JwtAuthorizationHandler.cs
├── Utils/              # Utility functions
│   ├── CalculateScoreUtils.cs
│   └── GenerateRandomUtils.cs
└── Program.cs          # Application entry point
```

### Pagination

The backend includes pagination functionality for listing orders and businesses. Pagination parameters such as `page` and `limit` can be specified in API requests to control the number of results returned. This allows for efficient retrieval of large datasets.

## API Documentation

You can find the API documentation for this project [here](https://documenter.getpostman.com/view/17842680/2s9YJZ5R4f). It provides detailed information on the available endpoints for order listing and management.

### Main Endpoints

- `POST /api/department/register` - Register a new department
- `POST /api/department/login` - Department login
- `POST /api/business/register` - Register a new business
- `POST /api/business/login` - Business login
- `GET /api/business` - Get all businesses (with pagination)
- `POST /api/orders` - Process an order (requires department authentication)
- `GET /api/credit-score` - Get credit score (requires business authentication)
- `GET /api/order-details/{businessID}` - Get order details for a business

## Deployment

To deploy the app to a live server, follow these steps:

1. Set up a production-ready server environment with .NET 9.0 runtime and MongoDB.

2. Build the application for production:

   ```bash
   dotnet publish -c Release -o ./publish
   ```

3. Deploy the built application to your hosting service (e.g., Azure App Service, AWS, or any server with .NET runtime).

4. Configure environment variables or `appsettings.Production.json` with your production settings:
   - MongoDB connection string
   - JWT secret key
   - Other configuration values

5. Set up a production-ready MongoDB database and update the connection string in your configuration.

## Contributing

Contributions to this project are welcome! If you would like to contribute, please follow these guidelines:

- Fork the repository.
- Create a new branch for your feature or bug fix.
- Make your changes and test thoroughly.
- Submit a pull request with a clear description of your changes.

## License

This project is licensed under the [License Name] License - see the [LICENSE.md](LICENSE.md) file for details.

## Contact

If you have any questions or feedback, please feel free to contact us at [faaiz.hadaina@gmail.com].
