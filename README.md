Plan de Formacion - Backend

API en .NET 10 para manejar usuarios, productos y pedidos. Arquitectura hexagonal, SQL Server con Entity Framework y login con JWT por cookie.

Como correrlo:

dotnet build
dotnet ef database update --project Infrastructure --startup-project PlanDeFormacionBack
dotnet run --project PlanDeFormacionBack

Se abre Swagger solo. Para probar endpoints con login usa el perfil https (https://localhost:7033), porque la cookie solo viaja por HTTPS.

Con Docker:

docker compose up --build

La API queda en http://localhost:8080/swagger

Pruebas:

dotnet test
