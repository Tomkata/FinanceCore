# Banking System

A complete banking system built with ASP.NET Core following Clean Architecture, Domain-Driven Design (DDD), and CQRS patterns.

## Features

### Core Banking Operations
- **Customer Management**: Create, view, search, and deactivate customers
- **Account Management**: Open checking, saving, and deposit accounts
- **Transactions**: Deposit, withdraw, and transfer between accounts
- **Account Operations**: Freeze, reactivate, and close accounts
- **Transaction History**: View transaction history with date range filtering

### Technical Features
- **JWT Authentication**: Secure token-based authentication
- **Clean Architecture**: Clear separation of concerns across layers
- **Domain-Driven Design**: Rich domain models with business logic
- **CQRS Pattern**: Separate read and write operations
- **Domain Events**: Event-driven architecture for cross-aggregate operations
- **Repository Pattern**: Data access abstraction
- **Unit of Work**: Transaction management
- **FluentValidation**: Input validation
- **Swagger/OpenAPI**: Interactive API documentation

### Account Types
1. **Checking Account**: Standard account with optional withdraw limits
2. **Saving Account**: Savings account with interest
3. **Deposit Account**: Fixed-term deposit with term-based interest

## Architecture

```
BankingSystem/
├── BankingSystem.Domain/           # Domain layer (entities, value objects, domain services)
├── BankingSystem.Application/      # Application layer (use cases, DTOs, handlers)
├── BankingSystem.Infrastructure/   # Infrastructure layer (repositories, database, identity)
├── BankingSystem.Web/              # Presentation layer (API controllers, frontend)
└── BankingSystem.Tests/            # Tests (domain, integration)
```

### Key Architectural Patterns

**Clean Architecture Layers:**
- **Domain**: Contains business logic, entities, value objects, domain services
- **Application**: Contains use cases, DTOs, validators, handlers
- **Infrastructure**: Contains database, repositories, external services
- **Web**: Contains API controllers, frontend UI

**DDD Concepts:**
- **Aggregates**: Customer (root), Account, Transaction
- **Value Objects**: Address, IBAN, Email, DepositTerm
- **Domain Events**: AccountCreditedEvent, AccountDebitedEvent, TransferInitiatedEvent
- **Domain Services**: TransferDomainService, TransactionDomainService, AccountFactory

## Technology Stack

- **Backend**: ASP.NET Core 8.0
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: ASP.NET Core Identity + JWT
- **Validation**: FluentValidation
- **Mapping**: Mapster
- **Frontend**: Vanilla JavaScript SPA
- **API Documentation**: Swagger/OpenAPI

## Prerequisites

- .NET 8.0 SDK
- SQL Server (LocalDB, Express, or Full)
- Visual Studio 2022 or VS Code (optional)

## Setup Instructions

### 1. Clone the Repository

```bash
git clone <repository-url>
cd FinanceCore
```

### 2. Configure Database Connection

Update the connection string in `BankingSystem.Web/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=BankingSystem;Integrated Security=true;TrustServerCertificate=true"
  }
}
```

### 3. Configure JWT Secret (IMPORTANT!)

⚠️ **Change the JWT Secret before production deployment!**

In `BankingSystem.Web/appsettings.json`:

```json
{
  "Jwt": {
    "Secret": "YourSuperSecretKeyThatIsAtLeast32CharactersLongForHS256Algorithm",
    "Issuer": "BankingSystem",
    "Audience": "BankingSystemUsers",
    "ExpirationMinutes": "60"
  }
}
```

### 4. Create Database and Run Migrations

```bash
cd BankingSystem.Infrastructure

# Create migration for Identity tables
dotnet ef migrations add AddIdentity --startup-project ../BankingSystem.Web

# Apply migrations to database
dotnet ef database update --startup-project ../BankingSystem.Web
```

### 5. Run the Application

```bash
cd ../BankingSystem.Web
dotnet run
```

The application will start on:
- **HTTPS**: `https://localhost:7XXX`
- **HTTP**: `http://localhost:5XXX`

(Port numbers will be displayed in the console)

## Using the Application

### Option 1: Web UI (Recommended)

1. Open browser to `https://localhost:7XXX`
2. Click **Register** and create a new account
3. You'll be automatically logged in
4. Navigate through the tabs:
   - **Customers**: Create and search customers
   - **Accounts**: Open and manage accounts
   - **Transactions**: Perform deposits, withdrawals, transfers

### Option 2: Swagger API

1. Open browser to `https://localhost:7XXX/swagger`
2. First, register a user:
   - Expand `POST /api/auth/register`
   - Click "Try it out"
   - Enter email, password, first name, last name
   - Click "Execute"
   - Copy the `token` from the response
3. Click the **Authorize** button (top right)
4. Enter: `Bearer <your-token-here>`
5. Click "Authorize"
6. Now you can test all protected endpoints!

## API Endpoints

### Authentication (Public)
```
POST /api/auth/register  - Register new user
POST /api/auth/login     - Login user
```

### Customers (Protected)
```
POST   /api/customers/create         - Create customer
GET    /api/customers/{id}           - Get customer by ID
GET    /api/customers/egn/{egn}      - Get customer by EGN
POST   /api/customers/open           - Open account for customer
POST   /api/customers/deposit        - Deposit to account
POST   /api/customers/withdraw       - Withdraw from account
POST   /api/customers/transfer       - Transfer between accounts
POST   /api/customers/deactive       - Deactivate customer
```

### Accounts (Protected)
```
POST   /api/accounts/open                    - Open new account
GET    /api/accounts/{id}                    - Get account by ID
GET    /api/accounts/iban/{iban}             - Get account by IBAN
GET    /api/accounts/customer/{customerId}   - Get all accounts for customer
POST   /api/accounts/freeze                  - Freeze account
POST   /api/accounts/reactivate              - Reactivate account
POST   /api/accounts/close                   - Close account
```

### Transactions (Protected)
```
GET    /api/transactions/{id}                            - Get transaction by ID
GET    /api/transactions/account/{accountId}             - Get transaction history
GET    /api/transactions/account/{accountId}/date-range  - Get transactions by date range
GET    /api/transactions/search                          - Search transactions
```

## Example Usage Flow

### 1. Register & Login
```bash
# Register
POST /api/auth/register
{
  "email": "user@example.com",
  "password": "Password123",
  "firstName": "John",
  "lastName": "Doe"
}

# Response includes JWT token
{
  "userId": "...",
  "email": "user@example.com",
  "token": "eyJhbGc..."
}
```

### 2. Create Customer
```bash
POST /api/customers/create
Authorization: Bearer <token>
{
  "username": "johndoe",
  "firstName": "John",
  "lastName": "Doe",
  "egn": "1234567890",
  "phoneNumber": "0123456789",
  "email": "john@example.com",
  "address": {
    "street": "123 Main St",
    "city": "Sofia",
    "zipCode": "1000",
    "country": "Bulgaria"
  }
}

# Response
{
  "id": "customer-guid",
  ...
}
```

### 3. Open Account
```bash
POST /api/accounts/open
Authorization: Bearer <token>
{
  "customerId": "customer-guid",
  "accountType": 0,  // 0=Checking, 1=Saving, 2=Deposit
  "initialBalance": 1000.00,
  "withdrawLimit": 500.00,
  "depositTerm": null
}

# Response
{
  "id": "account-guid",
  "iban": "BG80BNBG...",
  "balance": 1000.00,
  ...
}
```

### 4. Deposit Money
```bash
POST /api/customers/deposit
Authorization: Bearer <token>
{
  "customerId": "customer-guid",
  "accountId": "account-guid",
  "amount": 500.00
}
```

### 5. Transfer Between Accounts
```bash
POST /api/customers/transfer
Authorization: Bearer <token>
{
  "senderCustomerId": "sender-customer-guid",
  "fromAccountId": "sender-account-guid",
  "receiverCustomerId": "receiver-customer-guid",
  "toAccountId": "receiver-account-guid",
  "amount": 200.00
}
```

## Project Structure Details

### Domain Layer (`BankingSystem.Domain`)
- **Aggregates/**: Customer, Account, Transaction entities
- **ValueObjects/**: Address, IBAN, Email, DepositTerm
- **DomainServices/**: TransferDomainService, AccountFactory
- **Events/**: Domain events for cross-aggregate communication
- **Exceptions/**: Custom domain exceptions

### Application Layer (`BankingSystem.Application`)
- **UseCases/**: Command and query handlers (CQRS)
- **DTOs/**: Data transfer objects for API
- **Common/Interfaces**: Service interfaces
- **DomainEventHandlers/**: Event subscribers

### Infrastructure Layer (`BankingSystem.Infrastructure`)
- **Data/**: DbContext and EF configurations
- **Repositories/**: Repository implementations
- **Identity/**: ApplicationUser, JWT token service
- **Migrations/**: Database migrations

### Web Layer (`BankingSystem.Web`)
- **Controllers/**: REST API controllers
- **Middleware/**: Exception handling
- **wwwroot/**: Frontend SPA (HTML, CSS, JavaScript)

## Testing

### Run Tests
```bash
cd BankingSystem.Tests.Domain
dotnet test

cd ../BankingSystem.Tests.Integration
dotnet test
```

### Test Coverage
- Domain unit tests
- Integration tests for transfer flows
- Validator tests

## Security

### Authentication
- JWT token-based authentication
- Token expiration: 60 minutes (configurable)
- Secure password hashing with ASP.NET Core Identity

### Best Practices
- ✅ All API endpoints (except auth) require authentication
- ✅ Input validation with FluentValidation
- ✅ SQL injection prevention (EF Core parameterized queries)
- ✅ Exception handling middleware
- ⚠️ **Change JWT Secret in production**
- ⚠️ **Use HTTPS in production**
- ⚠️ **Configure CORS properly for production**

## Database Schema

### Main Tables
- **Customers**: Customer information
- **Accounts**: Bank accounts (TPH inheritance)
- **Transactions**: Financial transactions
- **TransactionEntries**: Double-entry bookkeeping
- **AspNetUsers**: Identity users
- **AspNetRoles**: User roles

### Relationships
- Customer 1:N Accounts
- Transaction 1:N TransactionEntries
- TransactionEntry N:1 Account
- ApplicationUser N:1 Customer (optional link)

## Troubleshooting

### Database Connection Issues
```bash
# Test SQL Server connection
sqlcmd -S . -E -Q "SELECT @@VERSION"

# If using named instance
Server=.\\SQLEXPRESS;Database=BankingSystem;...
```

### Migration Issues
```bash
# Remove last migration
dotnet ef migrations remove --project BankingSystem.Infrastructure --startup-project BankingSystem.Web

# Reset database
dotnet ef database drop --project BankingSystem.Infrastructure --startup-project BankingSystem.Web
dotnet ef database update --project BankingSystem.Infrastructure --startup-project BankingSystem.Web
```

### Port Already in Use
```bash
# Change ports in BankingSystem.Web/Properties/launchSettings.json
# Or let .NET assign random ports:
dotnet run --urls "https://localhost:0;http://localhost:0"
```

### JWT Token Expired
- Tokens expire after 60 minutes
- Login again to get a new token
- Adjust `ExpirationMinutes` in appsettings.json

## Future Enhancements

### Optional Features (Not Required for Junior Level)
- ✨ Email notifications for transactions
- ✨ PDF account statements
- ✨ Role-based authorization (Admin, Teller, Customer)
- ✨ Two-factor authentication
- ✨ Password reset functionality
- ✨ Audit trail
- ✨ Transaction limits and fraud detection
- ✨ Docker containerization
- ✨ CI/CD pipeline

## License

This is an educational project for learning purposes.

## Authors

- Banking system implementation following DDD and Clean Architecture principles
- Built with ASP.NET Core 8.0

## Support

For issues and questions:
1. Check the Troubleshooting section
2. Review the API documentation in Swagger
3. Check the console output for error messages
4. Review application logs

---

**Happy Banking! 🏦💰**
