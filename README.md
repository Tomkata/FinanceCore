<div align="center">

# 🏦 BankingSystem

**A production-grade banking backend built with Clean Architecture, DDD & CQRS**

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12-239120?style=for-the-badge&logo=csharp)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![EF Core](https://img.shields.io/badge/EF_Core-8.0-512BD4?style=for-the-badge&logo=dotnet)](https://learn.microsoft.com/en-us/ef/core/)
[![SQL Server](https://img.shields.io/badge/SQL_Server-2022-CC2927?style=for-the-badge&logo=microsoftsqlserver)](https://www.microsoft.com/en-us/sql-server)
[![Swagger](https://img.shields.io/badge/Swagger-OpenAPI_3-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)](https://swagger.io/)

*Built to demonstrate professional backend architecture, financial system design, and enterprise-grade patterns.*

</div>

---

## 📋 Table of Contents

- [Overview](#-overview)
- [Features](#-features)
- [Architecture](#-architecture)
- [Tech Stack](#-tech-stack)
- [Backend Design](#-backend-design)
- [API Reference](#-api-reference)
- [Database Design](#-database-design)
- [Security](#-security)
- [Project Status](#-project-status)
- [Getting Started](#-getting-started)
- [Future Improvements](#-future-improvements)
- [Why This Project](#-why-this-project)

---

## 🌐 Overview

**BankingSystem** is a RESTful API backend that models the core operations of a real-world bank. It is designed from the ground up with enterprise software principles — not as a tutorial project, but as a demonstration of how financial systems are actually architected.

The system handles:

- Customer registration and lifecycle management
- Multi-type bank accounts with type-specific business rules
- Financial transactions (deposits, withdrawals, transfers)
- **Double-entry bookkeeping** — every movement produces balanced ledger entries
- Domain-event-driven side effects for loose coupling
- Optimistic concurrency control to prevent race conditions

> This is a **backend-only** project. The API is fully explorable via Swagger UI.

---

## ✨ Features

### 👤 Customer Management
- Register customers with full validation including Bulgarian **EGN** (national ID) checksum verification
- Deactivate customers (blocked if they have active accounts)
- Update contact details (address, phone number)
- Prevent duplicate EGN registration at the database level

### 🏧 Account Lifecycle
| Account Type | Key Rules |
|---|---|
| **Checking** | Standard transactional account, no special restrictions |
| **Saving** | Monthly withdrawal limit enforced; resets automatically each month |
| **Deposit** | Fixed term with maturity date; early withdrawal throws a domain exception |

- Open, freeze, reactivate, and close accounts
- A customer cannot have more than one Deposit account
- Inactive customers cannot open new accounts

### 💸 Financial Operations
- **Deposit** funds into any active account
- **Withdraw** with balance and limit checks
- **Transfer** between any two accounts with full validation
- All amounts are validated before any state change occurs

### 📒 Double-Entry Bookkeeping
Every financial operation creates two balanced `TransactionEntry` records:

```
Deposit:   Customer Account  ← Debit (Asset)
           Vault Account     ← Credit (Liability)

Withdrawal: Vault Account    ← Debit (Asset)
            Customer Account ← Credit (Liability)

Transfer:  Sender Account    ← Credit
           Receiver Account  ← Debit
```

### 🔍 Transaction History
- Query full transaction history per account
- Filter by date range
- Search with flexible criteria

---

## 🏗 Architecture

The solution follows **Clean Architecture** with a strict unidirectional dependency rule:

```
┌─────────────────────────────────────────────────────────────┐
│                      BankingSystem.Web                      │
│              ASP.NET Core · Controllers · DI                │
└────────────────────────────┬────────────────────────────────┘
                             │ depends on
┌────────────────────────────▼────────────────────────────────┐
│                   BankingSystem.Application                 │
│         CQRS Handlers · DTOs · Validators · Mapster         │
└────────────────────────────┬────────────────────────────────┘
                             │ depends on
┌────────────────────────────▼────────────────────────────────┐
│                    BankingSystem.Domain                     │
│     Aggregates · Value Objects · Domain Events · Rules      │
└────────────────────────────┬────────────────────────────────┘
                             │ implemented by
┌────────────────────────────▼────────────────────────────────┐
│                 BankingSystem.Infrastructure                │
│       EF Core · Repositories · UnitOfWork · Migrations      │
└─────────────────────────────────────────────────────────────┘
```

**Patterns in use:**

| Pattern | Where |
|---|---|
| Clean Architecture | Solution-level layer separation |
| Domain-Driven Design (DDD) | Aggregates, Value Objects, Domain Events |
| CQRS | Separate command & query handlers per use case |
| Repository Pattern | `ICustomerRepository`, `IAccountRepository`, `ITransactionRepository` |
| Unit of Work | Centralized `SaveChanges` + domain event dispatch |
| Factory Pattern | `IAccountFactory` for account type instantiation |
| Result Pattern | `Result<T>` — explicit errors without exceptions |
| Domain Events | Decoupled side-effects (automatic transaction creation) |
| Optimistic Concurrency | Row-version columns on entities |

---

## 🛠 Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 8 |
| Language | C# 12 |
| ORM | Entity Framework Core 8 |
| Database | SQL Server |
| Validation | FluentValidation 12 |
| Mapping | Mapster 7 |
| DI Auto-Registration | Scrutor 7 |
| API Docs | Swashbuckle / Swagger (OpenAPI 3) |

---

## 🔧 Backend Design

### Domain Layer
The heart of the system. No external framework dependencies — pure business logic.

- **Aggregates:** `Customer`, `Account` *(abstract)*, `Transaction`
- **Concrete Accounts:** `CheckingAccount`, `SavingAccount`, `DepositAccount`
- **Value Objects:** `IBAN` *(mod-97 validated)*, `EGN` *(checksum + birth date extraction)*, `PhoneNumber`, `Address`, `DepositTerm`
- **Domain Events:** `AccountCreditedEvent`, `AccountDebitedEvent`, `TransferInitiatedEvent`
- **Domain Services:** `ITransferDomainService`, `ITransactionDomainService`, `IIbanGenerator`, `IAccountFactory`
- **Exception Hierarchy:** 20+ typed exceptions (`InsufficientFundsException`, `WithdrawLimitReachedException`, `EarlyWithdrawalException`, etc.)

### Application Layer
Orchestrates use cases through CQRS handlers. Each handler is a single-responsibility class.

<details>
<summary><strong>Command Handlers</strong></summary>

| Handler | Responsibility |
|---|---|
| `CreateCustomerHandler` | Validate, check EGN uniqueness, persist |
| `OpenBankAccountHandler` | Create account via Customer aggregate |
| `DepositToBankAccountHandler` | Trigger deposit, emit domain event |
| `WithdrawToBankAccountHandler` | Validate limits, trigger withdrawal |
| `TransferToBankAccountHandler` | Orchestrate cross-account transfer |
| `FreezeBankAccountHandler` | Freeze active account |
| `ReactiveBankAccountHandler` | Reactivate frozen account |
| `BlockBankAccountHandler` | Permanently close account |
| `DeactivateCustomerHandler` | Deactivate customer (guards active accounts) |
| `UpdateAddressHandler` | Update customer address |
| `UpdatePhoneNumberHandler` | Update customer phone |

</details>

<details>
<summary><strong>Query Handlers</strong></summary>

| Handler | Returns |
|---|---|
| `GetCustomerByIdHandler` | `CustomerDto` |
| `GetCustomerByEgnHandler` | `CustomerDto` |
| `GetAccountByIdHandler` | `AccountDto` |
| `GetAccountByIbanHandler` | `AccountDto` |
| `GetAllAccountsForCustomerHandler` | `IEnumerable<AccountSummaryDto>` |
| `GetTransactionByIdHandler` | `TransactionDto` |
| `GetTransactionHistoryForAccountHandler` | `IEnumerable<TransactionDto>` |
| `GetTransactionsByDateHandler` | `IEnumerable<TransactionDto>` |
| `SearchTransactionsHandler` | `IEnumerable<TransactionDto>` |

</details>

### Infrastructure Layer
Persistence and cross-cutting concerns.

- **`ApplicationDbContext`** — EF Core context with `Accounts`, `Customers`, `Transactions`, `TransactionEntries`
- **Table-Per-Type (TPT)** — Each account type maps to its own table
- **Repositories** — `CustomerRepository`, `AccountRepository`, `TransactionRepository`
- **`UnitOfWork`** — Wraps `SaveChangesAsync` in a DB transaction, then dispatches collected domain events
- **`DomainEventDispatcher`** — Reflection-based async handler resolution and invocation
- **Migrations** — 5 tracked migrations including schema init, EGN unique index, and row-version concurrency

### Web Layer
Thin controllers that delegate entirely to application handlers. No business logic here.

- Global exception handling via `UseGlobalExceptionHandler()` middleware
- Scrutor convention-based DI: handlers, validators, and repositories auto-registered by name suffix
- Swagger UI enabled in development

---

## 📡 API Reference

### Customers — `/api/customers`

| Method | Route | Description |
|---|---|---|
| `POST` | `/create` | Register a new customer |
| `POST` | `/openAccount` | Open an account for a customer |
| `POST` | `/deactiveCustomer` | Deactivate a customer |
| `POST` | `/deposit` | Deposit funds |
| `POST` | `/withdraw` | Withdraw funds |
| `POST` | `/transfer` | Transfer between accounts |
| `GET` | `/egn` | Get customer by EGN |
| `GET` | `/{id:guid}` | Get customer by ID |
| `PUT` | `/{customerId}/address` | Update address |
| `PUT` | `/{customerId}/phone` | Update phone number |

### Accounts — `/api/accounts`

| Method | Route | Description |
|---|---|---|
| `POST` | `/close` | Close an account |
| `POST` | `/freeze` | Freeze an account |
| `POST` | `/reactive` | Reactivate a frozen account |
| `GET` | `/id` | Get account by ID |
| `GET` | `/iban` | Get account by IBAN |
| `GET` | `/customer` | Get all accounts for a customer |

### Transactions — `/api/transactions`

| Method | Route | Description |
|---|---|---|
| `GET` | `/{id:guid}` | Get transaction by ID |
| `GET` | `/account/{accountId}` | Full history for an account |
| `GET` | `/account/{accountId}/date-range` | Filter by date range |
| `GET` | `/search` | Search with filters |

> All endpoints are documented and testable via **Swagger UI** at `/swagger`.

---

## 🗄 Database Design

```
Customer ──────────────────────────────────────────────────────┐
│ Id (PK)  · UserName · FirstName · LastName                   │
│ EGN (unique, validated) · PhoneNumber · Address              │
│ Status: Active | Inactive                                    │
└──────────────────────────────────────────────────────────────┘
         │ 1
         │ has many
         ▼ N
Account (Abstract — TPT Inheritance) ──────────────────────────┐
│ Id (PK) · IBAN (unique) · Balance · CustomerId (FK)          │
│ AccountStatus: Active | Blocked | Closed                     │
├─── CheckingAccount   (no extra columns)                      │
├─── SavingAccount     (WithdrawLimits · MonthlyCount · Date)  │
└─── DepositAccount    (DepositTerm · MaturityDate)            │
└──────────────────────────────────────────────────────────────┘
         │ 1
         │ has many
         ▼ N
Transaction ───────────────────────────────────────────────────┐
│ Id (PK) · Type · Status · Description · Date                 │
└──────────────────────────────────────────────────────────────┘
         │ 1
         │ has many (always 2 — double-entry)
         ▼ N
TransactionEntry ──────────────────────────────────────────────┐
│ Id (PK) · AccountId (FK) · TransactionId (FK)                │
│ EntryType: Debit | Credit                                    │
│ LedgerAccountType: Asset | Liability · Amount                │
└──────────────────────────────────────────────────────────────┘
```

**Key constraints:**
- `EGN` has a unique index — no duplicate customers
- `IBAN` has a unique index — globally unique account numbers
- `RowVersion` column on entities for optimistic concurrency
- EF Core **TPT inheritance** — clean separation of account-type-specific data

---

## 🔒 Security

Authentication and authorization are **not yet implemented** — this is a known and planned gap.

**What is in place:**
- `app.UseAuthentication()` and `app.UseAuthorization()` are registered in `Program.cs`
- `ApplicationUser` entity exists in `BankingSystem.Infrastructure/Identity/` ready for ASP.NET Core Identity
- The architecture is fully prepared for JWT bearer token integration

**Planned:** Role-based access control with `Admin`, `Customer`, and `Teller` roles via ASP.NET Core Identity + JWT.

---

## 📊 Project Status

> **Core backend is complete and functional. Some features are still in progress.**

| Area | Status |
|---|---|
| Domain model (aggregates, value objects, events) | ✅ Complete |
| CQRS command & query handlers | ✅ Complete |
| Repository & Unit of Work | ✅ Complete |
| EF Core schema & migrations | ✅ Complete |
| REST API controllers | ✅ Complete |
| FluentValidation for all commands | ✅ Complete |
| Double-entry transaction model | ✅ Complete |
| Domain exception hierarchy | ✅ Complete |
| Global exception middleware | ✅ Complete |
| Swagger / OpenAPI docs | ✅ Complete |
| Authentication & Authorization | 🔄 In progress |
| Identity integration | 🔄 In progress |
| Front-end / UI | ❌ Not started |
| Deployment configuration | ❌ Not started |

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (LocalDB, Express, or Developer Edition)

### 1. Clone the repository

```bash
git clone https://github.com/your-username/BankingSystem.git
cd BankingSystem
```

### 2. Configure the connection string

Edit `BankingSystem.Web/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=BankingSystem;Integrated Security=true;TrustServerCertificate=true"
}
```

### 3. Apply database migrations

```bash
dotnet ef database update --project BankingSystem.Infrastructure --startup-project BankingSystem.Web
```

### 4. Trust the dev certificate *(first time only)*

```bash
dotnet dev-certs https --trust
```

### 5. Run the application

```bash
dotnet run --project BankingSystem.Web --launch-profile https
```

### 6. Open Swagger UI

```
https://localhost:7159/swagger
```

---

## 🔮 Future Improvements

- **JWT Authentication** — Secure endpoints with bearer tokens and role-based access
- **API Versioning** — `/v1/`, `/v2/` strategy for long-term maintainability
- **Audit Log** — Record who did what and when in a dedicated audit table
- **Interest Calculation** — Automated accrual for savings and deposit accounts
- **Notifications** — Email/SMS alerts on transactions via a notification service
- **Rate Limiting** — Throttle sensitive endpoints (transfers, withdrawals)
- **Multi-currency** — Support multiple currencies with exchange rate handling
- **Monthly Statements** — Scheduled report generation per account
- **Health Checks** — `/health` endpoint for infrastructure monitoring
- **Docker Support** — `docker-compose` with SQL Server container
- **CI/CD Pipeline** — GitHub Actions for build, test, and deploy

---

## 💡 Why This Project

Most portfolio projects demonstrate CRUD operations with a thin service layer. This project deliberately goes further.

Banking is one of the most demanding domains in software: money cannot be lost, duplicated, or corrupted — even under concurrent load. Modeling it correctly forces you to think about **aggregate boundaries**, **transactional consistency**, **domain invariants**, and **event-driven side effects** in ways that simple web apps do not.

This project was built to demonstrate:

- **Clean Architecture** — how to enforce strict layer boundaries in a real codebase
- **Domain-Driven Design** — encoding business rules in the domain, not in controllers or services
- **CQRS** — separating read and write models for clarity and scalability
- **Double-entry bookkeeping** — applying real financial industry accounting to software
- **Production thinking** — optimistic concurrency, custom exceptions, result patterns, and validation at every boundary

---

<div align="center">

Built with ASP.NET Core 8 · Clean Architecture · Domain-Driven Design

</div>
