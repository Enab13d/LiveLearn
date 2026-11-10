
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

# Assessment service
*init and set user-secrets*
dotnet user-secrets init --project src/services/Assessment/LiveLearn.Assessment.API

dotnet user-secrets set "ConnectionStrings:AssessmentDb" \
  "Host=localhost;Port=5432;Database=livelearn_assessment;Username=YOUR_USERNAME;Password=YOUR_PASS" \
  --project src/services/Assessment/LiveLearn.Assessment.API

dotnet user-secrets set "RabbitMQ:Username" "YOUR_USERNAME" \
--project "src/services/Assessment/LiveLearn.Assessment.API"

dotnet user-secrets set "RabbitMQ:Password" "YOUR_PASS" \
--project "src/services/Assessment/LiveLearn.Assessment.API"

*Create migrations*
 dotnet ef migrations add InitialCreate \
    -p src/services/Assessment/LiveLearn.Assessment.Infrastructure \
    -c WriteDbContext \
    -s src/services/Assessment/LiveLearn.Assessment.API

# Enrollment service
*init and set user-secrets*
dotnet user-secrets init --project src/services/Enrollment/LiveLearn.Enrollment.API

dotnet user-secrets set "ConnectionStrings:EnrollmentDb" \
  "Host=localhost;Port=5432;Database=livelearn_enrollment;Username=YOUR_USERNAME;Password=YOUR_PASS" \
  --project src/services/Enrollment/LiveLearn.Enrollment.API

dotnet user-secrets set "RabbitMQ:Username" "YOUR_USERNAME" \
--project "src/services/Enrollment/LiveLearn.Enrollment.API"

dotnet user-secrets set "RabbitMQ:Password" "YOUR_PASS" \
--project "src/services/Enrollment/LiveLearn.Enrollment.API"

*Create migrations*
 dotnet ef migrations add InitialCreate \
    -p src/services/Enrollment/LiveLearn.Enrollment.Infrastructure \
    -c WriteDbContext \
    -s src/services/Enrollment/LiveLearn.Enrollment.API

# Ordering service
*init and set user-secrets*
dotnet user-secrets init --project src/services/Ordering/LiveLearn.Ordering.API

dotnet user-secrets set "ConnectionStrings:OrderingDb" \
  "Host=localhost;Port=5432;Database=livelearn_ordering;Username=YOUR_USERNAME;Password=YOUR_PASS" \
  --project src/services/Ordering/LiveLearn.Ordering.API

dotnet user-secrets set "RabbitMQ:Username" "YOUR_USERNAME" \
--project "src/services/Ordering/LiveLearn.Ordering.API"

dotnet user-secrets set "RabbitMQ:Password" "YOUR_PASS" \
--project "src/services/Ordering/LiveLearn.Ordering.API"

*Create migrations*
 dotnet ef migrations add InitialCreate \
    -p src/services/Ordering/LiveLearn.Ordering.Infrastructure \
    -c WriteDbContext \
    -s src/services/Ordering/LiveLearn.Ordering.API

# Payment service
*init and set user-secrets*
dotnet user-secrets init --project src/services/Payment/LiveLearn.Payment.API

dotnet user-secrets set "ConnectionStrings:PaymentDb" \
  "Host=localhost;Port=5432;Database=livelearn_payment;Username=YOUR_USERNAME;Password=YOUR_PASS" \
  --project src/services/Payment/LiveLearn.Payment.API

dotnet user-secrets set "RabbitMQ:Username" "YOUR_USERNAME" \
--project "src/services/Payment/LiveLearn.Payment.API"

dotnet user-secrets set "RabbitMQ:Password" "YOUR_PASS" \
--project "src/services/Payment/LiveLearn.Payment.API"

dotnet user-secrets set "Stripe:SecretKey" "sk_test_..." \
--project "src/services/Payment/LiveLearn.Payment.API"

*Create migrations*
 dotnet ef migrations add InitialCreate \
    -p src/services/Payment/LiveLearn.Payment.Infrastructure \
    -c WriteDbContext \
    -s src/services/Payment/LiveLearn.Payment.API

## Stripe webhooks (local)
No Dashboard destination and no ngrok needed: the Stripe CLI tunnels events to localhost.
Use a sandbox (test mode) account.

*Install and log in*
brew install stripe/stripe-cli/stripe

stripe login

*Forward events* (nginx :80 → gateway `/payment/...` → Payment `/webhooks/stripe`)
stripe listen \
  --events checkout.session.completed,checkout.session.expired,charge.dispute.created \
  --forward-to http://localhost/payment/webhooks/stripe

*Set the signing secret*
`stripe listen` prints `Your webhook signing secret is whsec_...` — it stays the same for this
device until you log in again. Put it into `.env` (docker) or user-secrets (`Stripe:PlatformWebhookSecret`):

STRIPE_SECRET_KEY=sk_test_...

STRIPE_PLATFORM_WEBHOOK_SECRET=whsec_...

STRIPE_CONNECT_WEBHOOK_SECRET=whsec_... *(same value until payout onboarding, US-PA04)*

STRIPE_THIN_WEBHOOK_SECRET=whsec_... *(same value until payout onboarding, US-PA04)*

*Notes*
- Events reach the service only while `stripe listen` runs.
- Stripe.net 53.1.0 pins API version `2026-10-28.endive`; snapshot events must be rendered in that
  version or parsing rejects them — set the sandbox account's API version to match.
- Test cards: `4242 4242 4242 4242` (succeeds), `4000 0000 0000 0002` (declined); any future date, any CVC.
- Trigger an event without paying: `stripe trigger checkout.session.completed`.
- A permanent destination (Dashboard / Workbench) needs a public HTTPS URL, e.g.
  `https://<ngrok-host>/payment/webhooks/stripe`, and has its own signing secret.

**Start in dev mode**
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d


