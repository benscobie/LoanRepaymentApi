# LoanRepaymentApi Instructions

LoanRepaymentApi is a .NET 8 ASP.NET Core Web API that calculates UK student loan repayment projections. It provides REST endpoints for loan calculations with comprehensive rate limiting, CORS support, and health monitoring.

Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

## Working Effectively

### Build and Test (NEVER CANCEL - Set 60+ minute timeouts)
- **Initial setup**: No special setup required - .NET 8 is the only dependency
- **Restore dependencies**: `dotnet restore` -- takes ~20 seconds first time, ~2 seconds when cached. NEVER CANCEL. Set timeout to 60+ seconds.
- **Build (Debug)**: `dotnet build --no-restore` -- takes ~11 seconds first time, ~2 seconds when cached. NEVER CANCEL. Set timeout to 60+ seconds.
- **Run all tests**: `dotnet test --no-build --verbosity normal` -- takes ~3-6 seconds for 90 tests (85 unit + 5 integration). NEVER CANCEL. Set timeout to 60+ seconds.
- **Build with code coverage**: `dotnet test --no-build --verbosity normal --collect:"XPlat Code Coverage" --settings CodeCoverage.runsettings --results-directory ./build-artifacts/coverage` -- takes ~5 seconds. NEVER CANCEL. Set timeout to 60+ seconds.

### Release Configuration (Limited)
- **Release build**: `dotnet build --configuration Release` -- FAILS due to Sentry authentication requirements. Only use Debug configuration.
- **Docker build**: Docker build FAILS due to missing Sentry auth token secrets. Do not attempt Docker builds.

### Run the Application
- **Start development server**: `cd src/LoanRepaymentApi && dotnet run` 
  - Application starts in ~3 seconds
  - Runs on http://localhost:5183 and https://localhost:7183
  - Swagger UI available at https://localhost:7183/swagger (Development mode only)
  - Health check: http://localhost:5183/health returns "Healthy"

### API Testing and Validation
- **Health endpoint**: `curl -s http://localhost:5183/health` returns "Healthy"
- **Sample API call**: `curl -s http://localhost:5183/ukstudentloans/assumptions` returns JSON with default assumptions
- **Integration test samples**: Use JSON files in `test/LoanRepaymentApi.Integration.Tests/UkStudentLoans/` for realistic API testing
- **Example calculation**: 
  ```bash
  curl -s -X POST http://localhost:5183/ukstudentloans/calculate \
    -H "Content-Type: application/json" \
    -d @test/LoanRepaymentApi.Integration.Tests/UkStudentLoans/calculate-type1-request.json
  ```

## Validation
- **ALWAYS run through complete build-test cycle after making changes**:
  1. `dotnet restore` (if dependencies changed)
  2. `dotnet build --no-restore`  
  3. `dotnet test --no-build --verbosity normal`
  4. Start application with `dotnet run` and test health endpoint
- **Manual validation scenarios after changes**:
  - Test the health endpoint returns "Healthy"
  - Test the assumptions endpoint returns valid JSON
  - For calculation changes: Test with sample requests from integration tests
  - Verify Swagger UI loads at https://localhost:7183/swagger in Development mode
- **NEVER skip validation** - even for small changes, always run the full build-test-start cycle
- **No UI to validate** - this is a pure API service, validation is via HTTP endpoints

## Common Tasks

### Project Structure
```
LoanRepaymentApi.sln                    # Main solution file
src/LoanRepaymentApi/                   # Main API project
├── Program.cs                          # Application entry point and configuration
├── UkStudentLoans/                     # Core business logic for UK student loans
│   ├── UkStudentLoansController.cs     # Main API controller
│   └── Calculation/                    # Loan calculation engine
└── appsettings*.json                   # Configuration files

test/LoanRepaymentApi.Unit.Tests/       # 85 unit tests
test/LoanRepaymentApi.Integration.Tests/ # 5 integration tests  
└── UkStudentLoans/                     # Sample JSON request/response files
```

### Key Endpoints
- `GET /health` - Health check (no auth required)
- `GET /ukstudentloans/assumptions` - Default calculation assumptions  
- `POST /ukstudentloans/calculate` - Main loan calculation endpoint (rate limited)
- `/swagger` - API documentation (Development mode only)

### Common Code Locations
- **Main controller**: `src/LoanRepaymentApi/UkStudentLoans/UkStudentLoansController.cs`
- **Business logic**: `src/LoanRepaymentApi/UkStudentLoans/Calculation/`
- **Configuration**: `src/LoanRepaymentApi/Program.cs` (dependency injection, middleware setup)
- **Test data**: `test/LoanRepaymentApi.Integration.Tests/UkStudentLoans/*.json`
- **Application settings**: `src/LoanRepaymentApi/appsettings.json` (rate limiting, CORS configuration)

### Build Information
- **Framework**: .NET 8 ASP.NET Core Web API
- **Test frameworks**: xUnit with Moq, AutoFixture, and AwesomeAssertions
- **Key NuGet packages**: FluentValidation, AutoMapper, NodaTime, AspNetCoreRateLimit, Sentry
- **Total build time**: ~2-11 seconds for Debug configuration (fast when cached)
- **Total test time**: ~3-6 seconds for all 90 tests
- **Warning**: 1 nullable reference warning in Program.cs (non-blocking)

### Limitations
- **Release builds require Sentry configuration** - stick to Debug builds for development
- **Docker builds fail** due to missing Sentry auth token secrets - use `dotnet run` instead
- **Swagger only available in Development environment** - not accessible in Production/Release mode
- **Rate limiting active** - POST /ukstudentloans/calculate limited to 60 requests per minute per IP

### CI/CD Information  
- **GitHub workflows**: `.github/workflows/dotnet.yml` (build and test), `lint.yml` (linting)
- **Code coverage**: Uses CodeCoverage.runsettings for test coverage collection
- **Linting**: Uses super-linter in CI - no local linting commands needed