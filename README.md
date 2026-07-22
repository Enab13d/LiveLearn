
# Identity service

*init and set user-secrets*
dotnet user-secrets init --project src/services/Identity/LiveLearn.Identity.API

dotnet user-secrets set "ConnectionStrings:UsersDb" \
  "Host=localhost;Port=5432;Database=livelearn_identity;Username=YOUR_USERNAME;Password=YOUR_PASS" \
  --project src/services/Identity/LiveLearn.Identity.API

*Create migrations*
 dotnet ef migrations add InitialCreate \
    -p src/services/Identity/LiveLearn.Identity.Infrastructure \
    -s src/services/Identity/LiveLearn.Identity.API
    

# Catalog service
*init and set user-secrets*
dotnet user-secrets init --project src/services/Catalog/LiveLearn.Catalog.API

dotnet user-secrets set "ConnectionStrings:CatalogDb" \
  "Host=localhost;Port=5432;Database=livelearn_catalog;Username=YOUR_USERNAME;Password=YOUR_PASS" \
  --project src/services/Catalog/LiveLearn.Catalog.API

dotnet user-secrets set "RabbitMQ:Username" "YOUR_USERNAME" \
--project "src/services/Catalog/LiveLearn.Catalog.API"

dotnet user-secrets set "RabbitMQ:Password" "YOUR_PASS" \
--project "src/services/Catalog/LiveLearn.Catalog.API"

*Create migrations*
 dotnet ef migrations add InitialCreate \
    -p src/services/Catalog/LiveLearn.Catalog.Infrastructure \
    -c WriteDbContext \
    -s src/services/Catalog/LiveLearn.Catalog.API

**Start in dev mode**
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d