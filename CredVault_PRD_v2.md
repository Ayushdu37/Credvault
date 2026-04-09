# CredVault — Product Requirements Document

> **v2 Architecture · 5-Service Edition**

| Property | Value |
|---|---|
| Document version | 2.0 |
| Architecture | 5-microservice (reduced from 11) |
| Status | Draft — Student Sprint |
| Sprint duration | 2 weeks |
| Tech stack | ASP.NET Core 10 · Angular 19 · SQL Server · RabbitMQ · Redis · Docker |

---

## 1. Executive Summary

CredVault is a credit card management platform that lets users track multiple credit cards, view bills, make payments, earn reward points, and receive real-time notifications — all through a clean Angular SPA backed by .NET microservices.

Version 2 reduces the service count from 11 to 5 by merging services that shared the same patterns with no unique technical concepts. The result is a project that covers every major backend pattern — Clean Architecture, CQRS, Domain Events, the Saga pattern, event-driven messaging, and API Gateway routing — with significantly less code to write and maintain.

### What v2 covers that v1 also covered

- Clean Architecture (Domain · Application · Infrastructure · API layers) across all 5 services
- CQRS with MediatR — Commands, Queries, and Validators in every service
- Domain Events and RabbitMQ pub/sub messaging between services
- Saga pattern with MassTransit state machine in payment-service
- EF Core 10 with migrations and repository pattern
- Ocelot API Gateway as the single entry point for the Angular SPA
- Shared NuGet contracts package consumed by all services
- Angular 19 SPA with standalone components, signals, lazy loading, and route guards
- Docker Compose for local dev and CI environments
- GitHub Actions CI/CD pipeline per service

---

## 2. Product Goals & Success Metrics

### 2.1 Goals

| # | Goal | Why it matters |
|---|---|---|
| G-01 | Users can add and manage multiple credit cards | Core product value — users with several cards need one place to see them all |
| G-02 | Users receive a monthly bill per card with due date tracking | Solves the problem of missing payment deadlines |
| G-03 | Users can pay bills in full, partially, or on a schedule | Flexible payment options reduce payment failures |
| G-04 | Users earn reward points automatically on every payment | Increases engagement and retention |
| G-05 | Users are notified by email on every significant event | Reduces support burden and keeps users informed |
| G-06 | Suspicious payments are flagged before they complete | Demonstrates fraud-detection integration in the payment Saga |
| G-07 | Every action is audit-logged for traceability | Demonstrates append-only event sourcing in the notification service |
| G-08 | The architecture demonstrates all key patterns for assessment | Primary goal of the 2-week sprint |

### 2.2 Success Metrics

| Metric | Target | Measured by |
|---|---|---|
| All 5 services start healthy in Docker Compose | 100% | `docker-compose up` — all containers green |
| Ocelot gateway routes correctly to all 5 services | 100% route coverage | Integration test hitting each `/api/{svc}/*` route |
| Payment Saga reaches Completed state on happy path | < 3 seconds end-to-end | Stopwatch in integration test |
| RabbitMQ events deliver to all subscribers | Zero missed events in local run | NotificationLogs table populated after each event |
| Angular SPA loads and authenticates successfully | Login → Dashboard < 2s | Manual browser test |
| All EF Core migrations run clean from scratch | Zero migration errors | `dotnet ef database update` on fresh DB |

---

## 3. User Personas

### P-01 · Rahul — Multi-card user

**Age 28 · Software engineer · holds 3 credit cards from different banks**

Rahul forgets due dates and often pays the wrong card. He wants one dashboard that shows all his cards, outstanding balances, upcoming bills, and lets him pay without switching between bank apps.

**Key needs:** card list with utilisation, bill calendar, one-click payment, reward point balance.

---

### P-02 · Priya — Rewards optimizer

**Age 32 · Marketing manager · pays everything on credit for the cashback**

Priya wants to know exactly how many reward points she earned from each payment and when she will hit the next tier. She needs the reward history to be transparent and per-payment.

**Key needs:** reward dashboard inside billing, tier progress, points breakdown per payment.

---

### P-03 · Admin — System operator

**Internal role · monitors fraud alerts and manages system health**

The admin needs to see fraud alerts raised by the payment service, review audit logs, and confirm that notifications were delivered. No separate admin service is needed for the demo — these are surfaced through existing service APIs.

**Key needs:** fraud alert list (payment-service API), audit log viewer (notification-service API).

---

## 4. Scope

### 4.1 In Scope

- **Identity** — registration, email verification, OTP login, JWT + refresh tokens, password reset
- **Card management** — add, remove, set default, verify, update limit for multiple cards per user
- **Billing** — monthly bill generation per card, minimum due, bill status lifecycle, payment scheduling
- **Rewards** — earn points on payment, tier tracking (Silver/Gold/Platinum), redemption, history
- **Payment** — initiate, complete, reverse; Saga with risk check and fraud detection; transaction log
- **Notifications** — email on every domain event via SendGrid templates; audit log written alongside
- **API Gateway** — Ocelot routing all frontend traffic through a single base URL
- **Angular SPA** — auth, dashboard, cards, billing, payments, rewards summary, profile
- **Docker Compose** — full local environment including RabbitMQ, Redis, SQL Server, SonarQube
- **CI/CD** — GitHub Actions workflow per service and for the Angular SPA

### 4.2 Out of Scope

> The following were in v1 but explicitly removed to reduce scope.

| Dropped | Reason |
|---|---|
| Support tickets & FAQ | Zero event integration, pure CRUD, not needed for pattern demonstration |
| Admin dashboard service | Admin use cases served by existing service APIs |
| Analytics service | Spending summaries derived from billing queries |
| Standalone security service | Risk evaluation and fraud detection merged into payment-service Saga |
| Separate audit service | Audit logging merged into notification-service as a second entity |
| Real payment gateway | Payments are simulated — no Razorpay/Stripe integration |
| Mobile app | Angular web SPA only |
| Multi-language / i18n | English only |

---

## 5. Architecture Overview

### 5.1 Service Map

All Angular SPA requests pass through the Ocelot API Gateway, which routes to one of the five downstream services. Services communicate asynchronously via RabbitMQ. There are **no synchronous HTTP calls between services**.

| Service | Database | Publishes | Subscribes to |
|---|---|---|---|
| identity-service | credvault_identity | `UserRegistered` `PasswordResetRequested` | — |
| card-service | credvault_cards | `CardAdded` `CardExpirySoon` | `PaymentCompleted` |
| billing-service | credvault_billing | `BillGenerated` `BillDueReminder` `RewardEarned` | `PaymentCompleted` |
| payment-service | credvault_payments | `PaymentInitiated` `PaymentCompleted` `PaymentFailed` `FraudDetected` | — |
| notification-service | credvault_notifications | — | `UserRegistered` `BillGenerated` `BillDueReminder` `PaymentCompleted` `PaymentFailed` `FraudDetected` `CardExpirySoon` `RewardEarned` |

### 5.2 Technology Stack

| Layer | Technology | Version / Notes |
|---|---|---|
| Backend services | ASP.NET Core | 10 — one project per service layer |
| ORM | EF Core | 10 — code-first migrations, repository pattern |
| Messaging | RabbitMQ + MassTransit | Docker container · MassTransit for consumers and Saga |
| Saga orchestration | MassTransit StateMachine | payment-service only |
| API Gateway | Ocelot | Latest stable · `ocelot.json` route config |
| Cache | Redis | OTP rate limiting and token blacklist |
| Database | SQL Server | One database per service · 5 databases total |
| Frontend | Angular 19 | Standalone components · signals · lazy-loaded routes |
| Auth | JWT + Refresh tokens | BCrypt passwords · 7-day refresh · OTP via Redis |
| Email | SendGrid | Template-based · 8 templates seeded in EmailTemplates |
| Containerisation | Docker + Compose | One Dockerfile per service · 3 Compose files |
| CI/CD | GitHub Actions | 6 workflow files — one per service + Angular SPA |
| Code quality | SonarQube | Docker container · `sonar-project.properties` per service |

### 5.3 Design Principles

- **No synchronous inter-service calls.** Services never call each other over HTTP. All cross-service communication is via RabbitMQ events. If a service needs data from another service, it either stores a cross-service ID (marked ✦) or listens to the relevant event.
- **Clean Architecture enforced in every service.** Each service has four projects: Domain (entities, domain events, interfaces), Application (CQRS commands/queries/validators, Saga), Infrastructure (EF Core, RabbitMQ publishers/consumers), and API (controllers, middleware, DI, Program.cs).
- **CQRS with MediatR.** Every controller dispatches a Command or Query through MediatR. No service layer. No direct repository calls from controllers.
- **Shared Contracts NuGet.** All request/response DTOs and event payload classes live in `CredVault.Shared.Contracts`. Every service references this package. No duplication.
- **One database per service.** Services own their data. There are no cross-database joins. Referential integrity across services is maintained by events.

---

## 6. Service Requirements

### 6.1 identity-service

| | |
|---|---|
| Database | `credvault_identity` |
| Tables | `Users` · `RefreshTokens` · `OTPCodes` |
| Publishes | `UserRegistered` · `PasswordResetRequested` |
| Subscribes | None |
| Unique pattern | JWT + Refresh token rotation · OTP via Redis · Domain Events |

**Functional requirements**

- **FR-ID-01** User can register with email and password. Email must be unique. Password hashed with BCrypt work factor 12.
- **FR-ID-02** Registration fires `UserRegistered` domain event, published to RabbitMQ. notification-service consumes this to send a welcome email.
- **FR-ID-03** User can verify email via a link. `IsEmailVerified` flips to 1.
- **FR-ID-04** User can log in with email + password. Returns a JWT (15-min expiry) and a refresh token (7-day expiry).
- **FR-ID-05** JWT is validated by Ocelot gateway on every request. No service re-validates the token.
- **FR-ID-06** Refresh token endpoint issues a new JWT and rotates the refresh token. Old token is revoked.
- **FR-ID-07** OTP can be sent to registered email for Login or Payment verification. Code is hashed, stored in `OTPCodes`, expires in 5 minutes.
- **FR-ID-08** Password reset flow: `SendOTP` (Purpose=PasswordReset) → `VerifyOTP` → `ResetPassword`. Fires `PasswordResetRequested` event.
- **FR-ID-09** Admin can deactivate a user (`IsActive = 0`). Deactivated users receive 401 on next request.

---

### 6.2 card-service

| | |
|---|---|
| Database | `credvault_cards` |
| Tables | `CreditCards` · `CardIssuers` |
| Publishes | `CardAdded` · `CardExpirySoon` |
| Subscribes | `PaymentCompleted` |
| Unique pattern | Multi-card per user · Cross-service balance update via event |

**Functional requirements**

- **FR-CD-01** Authenticated user can add a credit card. `MaskedNumber`, `ExpiryMonth`, `ExpiryYear`, `CardholderName`, `CreditLimit`, and `BillingCycleStartDay` are required.
- **FR-CD-02** Issuer is auto-detected from BIN prefixes in the `CardIssuers` seed table. `IssuerId` is set on the card.
- **FR-CD-03** Adding a card fires the `CardAdded` domain event. notification-service sends a confirmation email.
- **FR-CD-04** User can have multiple cards. Exactly one card per user can be marked `IsDefault=1`. Setting a new default clears the previous one.
- **FR-CD-05** User can remove a card (soft delete — `IsDeleted=1`, `DeletedAt` set). Card no longer appears in `GetCards` response.
- **FR-CD-06** User can verify a card via a simulated ₹1 micro-auth. Sets `IsVerified=1`.
- **FR-CD-07** User can update `CreditLimit` on any of their cards.
- **FR-CD-08** On consuming `PaymentCompleted`, `OutstandingBalance` on the relevant card is updated.
- **FR-CD-09** A scheduled job (background service) checks for cards expiring within 30 days and fires `CardExpirySoon`.
- **FR-CD-10** `GetCardUtilization` query returns `(OutstandingBalance / CreditLimit) × 100` for each card.

---

### 6.3 billing-service

| | |
|---|---|
| Database | `credvault_billing` |
| Tables | `Bills` · `PaymentSchedules` · `RewardTiers` · `RewardAccounts` · `RewardTransactions` |
| Publishes | `BillGenerated` · `BillDueReminder` · `RewardEarned` |
| Subscribes | `PaymentCompleted` |
| Unique pattern | Merged rewards ownership · Scheduled payment execution |

**Functional requirements — billing**

- **FR-BL-01** A bill is generated per card per calendar month. `BillingMonth = YYYY-MM`. `TotalAmount` = sum of all card transactions in that period. `MinimumDue` = 5% of TotalAmount (minimum ₹200).
- **FR-BL-02** Bill generation fires `BillGenerated` event. notification-service sends a bill summary email.
- **FR-BL-03** Bills not paid by `DueDate` transition to `Overdue` status. A `BillDueReminder` event is fired 3 days before `DueDate`.
- **FR-BL-04** On consuming `PaymentCompleted`, `AmountPaid` on the matching bill is incremented. If `AmountPaid >= TotalAmount`, `Status = Paid`. If `AmountPaid > 0` but `< TotalAmount`, `Status = PartiallyPaid`.
- **FR-BL-05** User can schedule a payment for a future date. `Status = Pending`. A background job executes it on `ScheduledDate` by calling payment-service's `InitiatePayment` command internally.
- **FR-BL-06** User can cancel a pending scheduled payment (`Status = Cancelled`).
- **FR-BL-07** `GetSpendingSummary` query returns total billed and total paid per card per month from the Bills table. Replaces analytics-service.

**Functional requirements — rewards (merged from rewards-service)**

- **FR-RW-01** Every user has exactly one `RewardAccount`, created on their first payment.
- **FR-RW-02** On consuming `PaymentCompleted`, billing-service computes reward points (1 point per ₹100 paid) and fires `RewardEarned` event. A `RewardTransaction` row (`Type=Earned`) is inserted.
- **FR-RW-03** `RewardAccount.AvailablePoints` and `TotalEarned` are updated atomically with the `RewardTransaction` insert.
- **FR-RW-04** Tier is recalculated on every point earn: Silver (0+), Gold (1000+), Platinum (5000+). `TierId` on `RewardAccount` is updated.
- **FR-RW-05** User can redeem available points. A `RewardTransaction` row (`Type=Redeemed`, Points negative) is inserted. `AvailablePoints` is decremented.
- **FR-RW-06** `GetRewardAccount` query returns `AvailablePoints`, `TotalEarned`, current Tier, and `CashbackPercent`.

---

### 6.4 payment-service

| | |
|---|---|
| Database | `credvault_payments` |
| Tables | `Payments` · `PaymentSagas` · `Transactions` · `RiskScores` · `FraudAlerts` |
| Publishes | `PaymentInitiated` · `PaymentCompleted` · `PaymentFailed` · `FraudDetected` |
| Subscribes | None |
| Unique pattern | MassTransit Saga · Inline risk evaluation · Compensation on failure |

**Saga state machine — PaymentSaga**

> The Payment Saga is the most technically complex part of the system. It coordinates a multi-step process that spans risk evaluation, payment processing, and downstream notifications — all within a single service.

**States:** `Initiated` → `RiskCheckPassed` → `Processing` → `Completed | Failed`

| Step | What happens |
|---|---|
| Step 1 | `InitiatePayment` command creates a `Payment` row (`Status=Initiated`) and a `PaymentSaga` row. Publishes `PaymentInitiated`. |
| Step 2 | `EvaluateRisk` command runs inline. Score 0–49 = AutoApproved, 50–74 = OTPRequired, 75+ = Blocked. `RiskScore` row inserted. |
| Step 3a | Score < 75: Saga moves to `RiskCheckPassed`. If OTPRequired, frontend shows OTP prompt. On OTP verify, Saga moves to `Processing`. |
| Step 3b | Score >= 75: Saga moves to `Failed`. `FraudAlert` row inserted. `FraudDetected` event published. Compensation: `Payment.Status = Failed`. |
| Step 4 | `Processing` → `CompletePayment` command. `Payment.Status = Completed`. `PaymentCompleted` event published. |
| Step 5 | On any error in Step 4: `ReversePayment` compensation. `Payment.Status = Reversed`. `PaymentFailed` event published. |

**Functional requirements**

- **FR-PM-01** User can initiate a payment for a bill (Full, Partial, or Scheduled type).
- **FR-PM-02** Every payment runs through the Saga. No payment skips risk evaluation.
- **FR-PM-03** A `Transaction` row is always inserted on payment completion or reversal, providing an immutable audit trail.
- **FR-PM-04** `GetPayments` and `GetTransactions` support pagination (`PagedResponse` from Shared.Contracts).
- **FR-PM-05** `PaymentCompleted` event is consumed by card-service (balance update), billing-service (bill status + rewards), and notification-service (email).
- **FR-PM-06** `FraudDetected` event is consumed by notification-service which sends a fraud alert email to the user.
- **FR-PM-07** `GetRiskScore` query returns the risk score and decision for any given payment.
- **FR-PM-08** `ReversePayment` command is exposed as an API endpoint for admin use cases.

---

### 6.5 notification-service

| | |
|---|---|
| Database | `credvault_notifications` |
| Tables | `EmailTemplates` · `NotificationLogs` · `AuditLogs` |
| Publishes | None |
| Subscribes | All 8 domain events — see table below |
| Unique pattern | Pure consumer · Template-based email · Merged audit logging |

**Event-to-template mapping**

| Event consumed | Email template used | Audit action logged |
|---|---|---|
| `UserRegistered` | WelcomeEmail | `User.Created` |
| `CardAdded` | CardAdded (in-app only) | `Card.Created` |
| `BillGenerated` | BillGenerated | `Bill.Created` |
| `BillDueReminder` | BillDueReminder | `Bill.ReminderSent` |
| `PaymentCompleted` | PaymentConfirmation | `Payment.Completed` |
| `PaymentFailed` | PaymentFailed (in-app only) | `Payment.Failed` |
| `FraudDetected` | FraudAlert | `Payment.FraudDetected` |
| `RewardEarned` | RewardEarned | `Reward.Earned` |
| `CardExpirySoon` | CardExpirySoon | `Card.ExpirySoon` |

**Functional requirements**

- **FR-NT-01** For each consumed event, exactly one `NotificationLog` row is inserted (`Status=Sent or Failed`) and exactly one `AuditLog` row is inserted.
- **FR-NT-02** Email is sent via SendGrid. On SendGrid failure, `Status=Failed` and `FailureReason` is stored. No retry in v2 — logged only.
- **FR-NT-03** `EmailTemplates` are seeded at startup if the table is empty. Templates support `{{variable}}` placeholder substitution.
- **FR-NT-04** `GetNotificationLogs` query returns paginated notification history for a given `UserId`.
- **FR-NT-05** `GetAuditLogs` query returns paginated audit history, filterable by `EntityType` and `UserId`.
- **FR-NT-06** `AuditLogs` are append-only. No update or delete operations on this table, ever.

---

## 7. API Gateway

Ocelot sits between the Angular SPA and all five services. The frontend has one base URL. Ocelot's `ocelot.json` defines the route map.

| Frontend route prefix | Downstream service | Port (local) | Auth required |
|---|---|---|---|
| `/api/identity/*` | identity-service | 5001 | No (auth endpoints) |
| `/api/cards/*` | card-service | 5002 | Yes — JWT |
| `/api/billing/*` | billing-service | 5003 | Yes — JWT |
| `/api/payments/*` | payment-service | 5004 | Yes — JWT |
| `/api/notify/*` | notification-service | 5005 | Yes — JWT |

**Gateway responsibilities**

- JWT validation on all protected routes — services trust the gateway's auth check and do not re-validate.
- Request routing via `ocelot.json` — no logic in code, all config-driven.
- Rate limiting headers forwarded downstream (`X-Rate-Limit`).
- `CorrelationId` header injected on every forwarded request for audit tracing.

---

## 8. Event Catalog

All events are JSON messages published to RabbitMQ. Schemas are defined in `contracts/events/`. Every event payload includes a `CorrelationId` for distributed tracing.

| Event name | Published by | Consumed by | Trigger |
|---|---|---|---|
| `UserRegistered` | identity-service | notification-service | Successful registration |
| `PasswordResetRequested` | identity-service | notification-service | OTP sent for password reset |
| `CardAdded` | card-service | notification-service | New card saved successfully |
| `CardExpirySoon` | card-service | notification-service | Card expires within 30 days |
| `BillGenerated` | billing-service | notification-service | Monthly bill created |
| `BillDueReminder` | billing-service | notification-service | 3 days before DueDate |
| `RewardEarned` | billing-service | notification-service | PaymentCompleted consumed, points computed |
| `PaymentInitiated` | payment-service | notification-service | Saga Step 1 start |
| `PaymentCompleted` | payment-service | card-service · billing-service · notification-service | Saga Step 4 success |
| `PaymentFailed` | payment-service | notification-service | Saga compensation triggered |
| `FraudDetected` | payment-service | notification-service | Risk score >= 75 |

---

## 9. Angular SPA Requirements

Angular 19 SPA consuming all five service APIs via the Ocelot gateway. All feature modules are lazy-loaded. Auth state managed via signals. JWT stored in memory — never `localStorage`.

### 9.1 Feature modules

| Feature module | Route | Calls service | Guard |
|---|---|---|---|
| auth/login | `/login` | identity-service | None |
| auth/register | `/register` | identity-service | None |
| auth/verify-email | `/verify-email` | identity-service | None |
| auth/mfa | `/mfa` | identity-service | None |
| auth/reset-password | `/reset-password` | identity-service | None |
| dashboard | `/dashboard` | billing-service · card-service | AuthGuard |
| cards/card-list | `/cards` | card-service | AuthGuard · KycGuard |
| cards/add-card | `/cards/add` | card-service | AuthGuard · KycGuard |
| cards/card-detail | `/cards/:id` | card-service | AuthGuard |
| billing/bills-list | `/billing` | billing-service | AuthGuard |
| billing/bill-detail | `/billing/:id` | billing-service | AuthGuard |
| billing/rewards | `/billing/rewards` | billing-service | AuthGuard |
| payments/pay-bill | `/payments/pay/:billId` | payment-service | AuthGuard |
| payments/payment-history | `/payments/history` | payment-service | AuthGuard |
| profile | `/profile` | identity-service | AuthGuard |

### 9.2 Core services

| File | Responsibility |
|---|---|
| `auth.service.ts` | Login, register, OTP, refresh token rotation, logout. Stores JWT in memory signal. |
| `api.service.ts` | Base HTTP client wrapping `HttpClient`. All feature services extend this. |
| `token.service.ts` | Manages JWT lifecycle, expiry detection, and refresh scheduling. |
| `auth.interceptor.ts` | Attaches `Bearer` token to every outgoing request. |
| `error.interceptor.ts` | Catches 401 (auto-refresh), 403 (redirect to login), 5xx (toast error). |
| `auth.guard.ts` | Blocks unauthenticated navigation. |
| `role.guard.ts` | Blocks users without the required Role claim. |
| `kyc.guard.ts` | Blocks navigation to card routes until email is verified. |

### 9.3 Shared components

| Component | Usage |
|---|---|
| `ButtonComponent` | Reusable styled button with loading state slot |
| `CardComponent` | Generic content card wrapper used across all features |
| `SpinnerComponent` | Full-page and inline loading indicator |
| `NavbarComponent` | Top navigation with user name, card count badge, notification bell |
| `CurrencyInrPipe` | Formats numbers as ₹X,XX,XXX.XX |
| `RelativeDatePipe` | Converts UTC datetimeoffset to '2 days ago', 'tomorrow', etc. |

---

## 10. Non-Functional Requirements

> These apply to the demo scope. Production hardening is explicitly out of scope.

| NFR | Requirement | How achieved |
|---|---|---|
| NFR-01 · Security | Passwords hashed with BCrypt work factor 12. JWTs signed with RS256 key pair. OTPs hashed in DB. | identity-service Domain layer |
| NFR-02 · Security | No inter-service HTTP calls. Malicious gateway injection has no lateral attack surface. | RabbitMQ-only inter-service comms |
| NFR-03 · Availability | All 5 services + infra start cleanly from a single `docker-compose up` command. | `docker-compose.yml` + health checks |
| NFR-04 · Traceability | Every HTTP request carries a `CorrelationId` injected by Ocelot. AuditLogs and NotificationLogs store it. | Ocelot header injection + notification-service |
| NFR-05 · Maintainability | No service has more than 4 `.csproj` files. Clean Architecture layer boundaries enforced by project references. | Solution structure + `.csproj` dependencies |
| NFR-06 · Testability | Every Command and Query Handler can be unit tested in isolation via MediatR without spinning up infrastructure. | Clean Architecture + CQRS separation |
| NFR-07 · Data integrity | Cross-service IDs use `uniqueidentifier`. No foreign keys across database boundaries. Event-driven consistency. | DB design + RabbitMQ consumers |
| NFR-08 · Observability | SonarQube runs as a Docker container. Each service has `sonar-project.properties`. | `infra/sonarqube/` + CI pipeline |

---

## 11. Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|---|---|---|---|
| Saga state machine complexity causes payment-service to take longest to build | High | High | Build and test the Saga first (Day 1–3). Use MassTransit's built-in visualiser. Keep Saga states to 4 only. |
| RabbitMQ consumer ordering issues cause notification emails to fire before DB row is committed | Medium | Medium | Use outbox pattern on all publishers. Write DB row first, publish event after commit. |
| EF Core migrations conflict when all 5 services are developed in parallel | Medium | Low | Each service has its own `DbContext` and its own database. Zero migration conflicts are possible by design. |
| Angular JWT expiry during long Saga (> 15 min) causes payment to fail silently | Low | High | `error.interceptor.ts` auto-refreshes on 401. Saga timeout set to 10 min — well within the 15-min JWT window. |
| Docker Compose out-of-memory on student laptops | Low — v2 only has 9 containers | Medium | v2 removes 6 service containers from v1. 9 containers (5 services + gateway + RabbitMQ + SQL + Redis) is manageable. |

---

## 12. Sprint Timeline

2-week sprint. Backend-first order: identity → card → billing → payment → notification. Angular SPA built last when all APIs are stable.

| Day | Focus | Deliverable | Services touched |
|---|---|---|---|
| 1–2 | Foundation | Solution skeleton, Shared.Contracts NuGet, Docker Compose infra, Ocelot gateway, `init.sql` | All (setup only) |
| 3–4 | Identity | Registration, login, JWT, OTP, refresh token, `UserRegistered` event firing | identity-service |
| 5 | Cards | Add/remove/list cards, `CardAdded` event, `PaymentCompleted` consumer updating balance | card-service |
| 6–7 | Billing + Rewards | Bill generation, `PaymentCompleted` consumer, reward point earning, `RewardEarned` event | billing-service |
| 8–9 | Payment + Saga | Full Saga: initiate → risk check → OTP → complete/fail, `FraudDetected` event | payment-service |
| 10 | Notification | All 8 consumers wired, email templates seeded, AuditLog written per event | notification-service |
| 11–12 | Angular SPA | Auth → dashboard → cards → billing → payments → rewards. Guards, interceptors, pipes. | angular-spa |
| 13 | Integration testing | Docker Compose full run, API smoke tests, event flow end-to-end verification | All |
| 14 | Polish + docs | README, Postman collection, SonarQube baseline, presentation prep | All |
