# Vizus Backend

ASP.NET Core Web API for **Vizus**, an eye clinic with an optical shop. Built with Clean Architecture and CQRS.

> Frontend: [vizus-frontend](https://github.com/Veselin47/vizus-frontend)

## Features

- JWT authentication with ASP.NET Identity and Admin/Client roles
- Product catalog and categories, CRUD with image upload
- Doctors with working hours and days; **availability engine** that generates free 30-minute slots
- Appointment booking with email confirmation link, status management and cancellation emails
- Orders with Stripe Checkout and a webhook that marks orders as paid
- Delivery options with live Econt city and office lookup
- Transactional emails (MailKit / SMTP)

## Architecture

```
Vizus.Domain          Entities, enums, repository interfaces (no dependencies)
Vizus.Application     Commands and queries (MediatR), DTOs, validators, service interfaces
Vizus.Infrastructure  EF Core, repositories, Identity, Stripe, Econt, email
Vizus.API             Controllers, middleware, DI composition root
```

Dependencies point inward: Domain knows nothing about the other layers, and Application only depends on Domain. External services (Stripe, email, Econt) sit behind interfaces defined in Application.

**Tech:** .NET 10 · EF Core · SQL Server · MediatR · FluentValidation · Serilog · Stripe.net · MailKit · Swagger

## Design decisions

- **Prices are never trusted from the client.** The order handler reads current prices and stock from the database.
- **Order items are snapshots** (name, price, image), so later catalog changes do not rewrite history.
- **Webhook is idempotent.** Stripe can deliver an event more than once; an order already marked `Paid` is skipped, so the customer gets one email.
- **Double-booking is prevented in the database**, not only in code: a unique index on `(DoctorId, StartTime)` filtered to non-cancelled appointments. A cancelled slot can be booked again, and a race between two patients returns `409 Conflict`.
- **Appointments are never deleted**, only cancelled, to keep an audit trail.
- **Email is best effort.** A failed email is logged and never rolls back a successful booking or payment.
- **Global exception middleware** maps exceptions to HTTP codes (validation 400, not found 404, conflict 409).

## Getting started

Prerequisites: .NET 10 SDK, SQL Server (Express is fine), and optionally the Stripe CLI.

**1. Connection string** in `Vizus.API/appsettings.json` (`ConnectionStrings:DefaultConnection`).

**2. Secrets.** Nothing sensitive is committed; use user secrets:

```bash
cd Vizus.API
dotnet user-secrets set "Jwt:Key" "a-long-random-string-of-at-least-32-characters"
dotnet user-secrets set "Stripe:SecretKey" "sk_test_..."
dotnet user-secrets set "Stripe:WebhookSecret" "whsec_..."
dotnet user-secrets set "Stripe:PublicBaseUrl" "https://your-public-url"
dotnet user-secrets set "Email:SmtpHost" "smtp.gmail.com"
dotnet user-secrets set "Email:SmtpPort" "587"
dotnet user-secrets set "Email:Username" "you@gmail.com"
dotnet user-secrets set "Email:Password" "app-password"
dotnet user-secrets set "Frontend:BaseUrl" "http://localhost:5173"
```

`Stripe:PublicBaseUrl` must be publicly reachable (for example an ngrok URL) for Stripe to show product images at checkout.

**3. Database**

```bash
dotnet ef database update --project Vizus.Infrastructure --startup-project Vizus.API
```

**4. Run**

```bash
dotnet run --project Vizus.API
```

Swagger UI: `https://localhost:7144/swagger`. Roles and an admin user are seeded on first start.

**5. Stripe webhooks locally**

```bash
stripe listen --forward-to https://localhost:7144/api/stripe/webhook
```

Copy the `whsec_...` it prints into `Stripe:WebhookSecret` and restart the API.

## API overview

| Area | Endpoints |
|---|---|
| Auth | `POST /api/auth/register`, `POST /api/auth/login` |
| Products | `GET/POST /api/products`, `PUT/DELETE /api/products/{id}` |
| Categories | `GET/POST /api/categories`, `PUT/DELETE /api/categories/{id}` |
| Doctors | `GET/POST /api/doctors`, `PUT/DELETE /api/doctors/{id}` |
| Appointments | `GET /api/appointments/available-slots`, `POST /api/appointments`, `POST /api/appointments/confirm`, `PUT /api/appointments/{id}/status`, `GET /api/appointments` (admin) |
| Orders | `POST /api/orders`, `POST /api/stripe/webhook` |
| Shipping | `GET /api/shipping/econt/cities`, `GET /api/shipping/econt/offices` |
| Upload | `POST /api/upload/image` (admin) |

Write operations on catalog data require the `Admin` role.

## Known limitations and roadmap

- Unit and integration tests are still to be written (the availability logic is the first candidate)
- `POST /api/appointments/{id}/cancel` does not yet verify that the appointment belongs to the caller
- Unconfirmed (`Pending`) appointments hold their slot indefinitely; a background job should release them
- The seeded admin credentials are for development only and must come from configuration before deployment
- Not deployed yet
