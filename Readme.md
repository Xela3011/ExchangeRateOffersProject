# Exchange Rate Offers API

A **.NET 9** Web API that queries multiple exchange rate providers in parallel, compares results, and returns the **best currency conversion offer**.  
Built using **Clean Architecture**, **SOLID principles**, and fully covered with **unit tests**.

---

##  Features
- **Parallel API calls** to multiple providers (JSON & XML formats)
- Resilient error handling — works even if one or more providers fail
- Clean architecture & SOLID principles
- Configuration-driven API URLs (`appsettings.json`)
- Full **xUnit** test suite with **Moq** for mocking dependencies
- Docker-ready deployment

---

##  Project Structure
src/Api
# Web API project (controllers, Program.cs)

src/Core
Application # Application layer (interfaces, services, models)
Infrastructure/ # Providers for API1, API2, API3

src/Test
# Unit tests (service, providers, controller)

## Configuration
Set your API endpoints in `appsettings.{env}.json`:

```json
{
  "Api1": { "url": "https://json1.com/api" },
  "Api2": { "url": "https://xml.com/api" },
  "Api3": { "url": "https://json3.com/api" }
}
```

## Running Locally

1. Clone Repo
2. Restore Dependencies
```
dotnet restore
```
3. Run the APi
``` 
dotnet run --project src/api
```
4.Test the Api
```
curl -X POST https://localhost:{port}/api/exchangeRate/best-offer \
  -H "Content-Type: application/json" \
  -d '{"sourceCurrency":"USD","targetCurrency":"EUR","amount":100}'
```

## Running Tests
```
dotnet test
```

## Running with Docker
```
docker build -t exchange-rate-offers .
docker run -p 5000:80 exchange-rate-offers
```