dotnet user-secrets init --project src/Services/Identity/LiveLearn.Identity.API

dotnet user-secrets set "ConnectionStrings:UsersDb" \
  "Host=localhost;Port=5432;Database=livelearn_identity;Username=YOUR_USERNAME;Password=YOUR_PASS" \
  --project src/Services/Identity/LiveLearn.Identity.API

**Create migrations**
<!-- Identity service -->
 dotnet ef migrations add InitialCreate \
    -p src/Services/Identity/LiveLearn.Identity.Infrastructure \
    -s src/Services/Identity/LiveLearn.Identity.API
    

**Start in dev mode**
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d