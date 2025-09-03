# RealEstate Technical Test (.NET 8, SQL Server, C#, NUnit)

This solution implements the required services:

- Create Property Building
- Add Image from property
- Change Price
- Update property
- List property with filters

## Stack

- .NET 8 (works with .NET 5+ by lowering TargetFramework if needed)
- ASP.NET Core Web API
- EF Core + SQL Server
- NUnit + FluentAssertions
- Swagger
- JWT Bearer (simple demo secret)

## Run locally

1. Ensure SQL Server is available.
2. Create DB + tables (option A: raw SQL): open `database/CreateDatabase.sql` in SSMS and run it.
   - Or (option B: EF Core Migrations): From `src/RealEstate.Api` you can initialize migrations:
     ```powershell
     dotnet tool install --global dotnet-ef
     dotnet ef migrations add Initial --project ..\RealEstate.Infrastructure --startup-project .
     dotnet ef database update --project ..\RealEstate.Infrastructure --startup-project .
     ```
3. Configure connection string in `src/RealEstate.Api/appsettings.json` if needed.
4. Run the API:
   ```powershell
   dotnet build RealEstate.sln
   dotnet run --project src/RealEstate.Api/RealEstate.Api.csproj
   ```
5. Open Swagger: https://localhost:63533/swagger/index.html

## Endpoints

- `POST /api/properties` (Create Property Building) **[Authorize]**
- `POST /api/properties/{id}/images` (Add Image from property) **[Authorize]**
- `PUT /api/properties/{id}/price` (Change Price) **[Authorize]**
- `PUT /api/properties/{id}` (Update property) **[Authorize]**
- `GET /api/properties` (List property with filters)
- `GET /api/properties/{id}` (Get by id)

### Query params (list)

`city, state, minBedrooms, minBathrooms, minArea, maxArea, minPrice, maxPrice, status, sortBy=price|area|createdAt, desc=true|false, page, pageSize`

## Security

- JWT Bearer configured. Replace `Jwt:Secret` and integrate your Identity Provider.
- Mutating endpoints require `[Authorize]`.
- Validation uses DataAnnotations and returns 400 for invalid input.

## Performance

- Read queries use `AsNoTracking` and pagination.
- DB indexes on `Properties.Code` and `(PropertyId, IsCover)`.
- Use SQL Server column types for decimals to avoid precision issues.

## Tests

Run:
```powershell
dotnet test tests/RealEstate.Tests/RealEstate.Tests.csproj
```

## Notes

- Images are stored by URL (CDN/S3/Blob). Persisting binary content is out of scope.
- Consider Azure Blob Storage or S3 and generate pre-signed URLs in production.
