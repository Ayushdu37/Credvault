# CredVault — Database Design v2

> **5 microservices · 18 tables · SQL Server · EF Core 10 · 2-week sprint**

| Property | Value |
|---|---|
| Services (v2) | 5 microservices (down from 11) |
| Total tables | 18 tables (down from 28) |
| Engine | SQL Server |
| ORM | EF Core 10 |
| Timeline | 2-week sprint |

---

## What changed from v1?

| Dropped service | Where it went |
|---|---|
| rewards-service | Merged into **billing-service** (RewardTiers, RewardAccounts, RewardTransactions) |
| security-service | Merged into **payment-service** (RiskScores, FraudAlerts) |
| audit-service | Merged into **notification-service** (AuditLogs) |
| analytics-service | Dropped — spending summaries computed on-the-fly from Bills queries |
| support-service | Dropped — no event integration, pure CRUD not needed for demo |
| admin-service | Dropped — read model not needed for demo scope |

All column conventions, cross-service ID rules, and data types are identical to v1.

---

## Global Rules

> **Keep it simple.** This is a demo project. Every table has only what is needed to make the feature work and look good in a presentation. Optimise for buildable-in-two-weeks, not production-perfect.

### Column conventions

- **Id** — `uniqueidentifier`, PK, default `NEWID()`. Every table has one.
- **CreatedAt** — `datetimeoffset NOT NULL`. Added to every table. Set on insert, never changed.
- **UpdatedAt** — `datetimeoffset NULL`. Added to mutable tables. Set on every update.
- **IsDeleted + DeletedAt** — only on three tables where soft-delete is user-visible: `CreditCards`, `Bills`, `Payments`. Everywhere else, hard-delete.
- **Money columns** — `decimal(18,2)`. No float.
- **Dates** — `datetimeoffset` (UTC). Frontend converts to local time.
- **Enums** — `nvarchar(20)` with a CHECK constraint. Simpler than a lookup table for a demo.

### Cross-service IDs

- A `UserId` stored in card-service has **no foreign key** back to identity-service. They are in different databases.
- All cross-service IDs are plain `uniqueidentifier` columns — referential integrity is enforced by events, not the DB.
- These are marked with **✦** in the Notes column.

### Legend

| Symbol | Meaning |
|---|---|
| **PK** | Primary key |
| **FK** | Foreign key within the same database |
| **✦** | Cross-service ID — no hard FK, enforced by events |

---

## 1. identity-service

**Database:** `credvault_identity`  
**Tables:** `Users` · `RefreshTokens` · `OTPCodes`

Handles registration, login, JWT tokens, OTP, and sessions. Unchanged from v1.

### Users

| Column | Type | Constraint | Notes |
|---|---|---|---|
| **Id** | `uniqueidentifier` | PK | `NEWID()` |
| Email | `nvarchar(256)` | UNIQUE NOT NULL | Lowercase. Login username. |
| PasswordHash | `nvarchar(512)` | NOT NULL | BCrypt hash, work factor 12. |
| FirstName | `nvarchar(100)` | NOT NULL | |
| LastName | `nvarchar(100)` | NOT NULL | |
| Role | `nvarchar(20)` | NOT NULL | `User \| Admin \| SupportAgent` |
| IsEmailVerified | `bit` | NOT NULL DEF 0 | Set to 1 after clicking verification link. |
| IsActive | `bit` | NOT NULL DEF 1 | 0 = account disabled by admin. |
| CreatedAt | `datetimeoffset` | NOT NULL | |
| UpdatedAt | `datetimeoffset` | NULL | |

### RefreshTokens

| Column | Type | Constraint | Notes |
|---|---|---|---|
| **Id** | `uniqueidentifier` | PK | `NEWID()` |
| **UserId** | `uniqueidentifier` | FK → Users.Id | Token owner. |
| Token | `nvarchar(512)` | UNIQUE NOT NULL | Hashed value stored. 7-day TTL. |
| ExpiresAt | `datetimeoffset` | NOT NULL | |
| IsRevoked | `bit` | NOT NULL DEF 0 | Set on logout or password reset. |
| CreatedAt | `datetimeoffset` | NOT NULL | |

### OTPCodes

| Column | Type | Constraint | Notes |
|---|---|---|---|
| **Id** | `uniqueidentifier` | PK | `NEWID()` |
| **UserId** | `uniqueidentifier` | FK → Users.Id | Who requested the OTP. |
| CodeHash | `nvarchar(512)` | NOT NULL | Hashed 6-digit code. |
| Purpose | `nvarchar(30)` | NOT NULL | `Login \| Payment \| PasswordReset` |
| ExpiresAt | `datetimeoffset` | NOT NULL | 5 minutes from generation. |
| IsUsed | `bit` | NOT NULL DEF 0 | Consumed on first successful verify. |
| CreatedAt | `datetimeoffset` | NOT NULL | |

---

## 2. card-service

**Database:** `credvault_cards`  
**Tables:** `CreditCards` · `CardIssuers`

Stores credit cards, issuer info, and utilisation. Unchanged from v1.

### CreditCards

| Column | Type | Constraint | Notes |
|---|---|---|---|
| **Id** | `uniqueidentifier` | PK | `NEWID()` |
| **UserId** | `uniqueidentifier` | NOT NULL | ✦ Owner from identity-service. |
| MaskedNumber | `nvarchar(20)` | NOT NULL | e.g. `**** **** **** 1234` |
| CardholderName | `nvarchar(200)` | NOT NULL | Name printed on card. |
| ExpiryMonth | `tinyint` | NOT NULL | 1–12 |
| ExpiryYear | `smallint` | NOT NULL | 4-digit year. |
| **IssuerId** | `uniqueidentifier` | FK → CardIssuers.Id | Detected issuer. |
| CreditLimit | `decimal(18,2)` | NOT NULL | User-entered limit in INR. |
| OutstandingBalance | `decimal(18,2)` | NOT NULL DEF 0 | Updated on PaymentCompleted events. |
| BillingCycleStartDay | `tinyint` | NOT NULL | Day of month 1–28. |
| IsDefault | `bit` | NOT NULL DEF 0 | One default card per user. |
| IsVerified | `bit` | NOT NULL DEF 0 | True after ₹1 micro-auth. |
| IsDeleted | `bit` | NOT NULL DEF 0 | Soft delete. |
| DeletedAt | `datetimeoffset` | NULL | |
| CreatedAt | `datetimeoffset` | NOT NULL | |
| UpdatedAt | `datetimeoffset` | NULL | |

### CardIssuers

| Column | Type | Constraint | Notes |
|---|---|---|---|
| **Id** | `uniqueidentifier` | PK | `NEWID()` |
| Name | `nvarchar(50)` | UNIQUE NOT NULL | `Visa \| MasterCard \| Amex \| RuPay` |
| CardLength | `tinyint` | NOT NULL | 16 for Visa/MC/RuPay, 15 for Amex. |
| BinPrefixes | `nvarchar(200)` | NOT NULL | Comma-separated BIN prefixes for detection. |
| CreatedAt | `datetimeoffset` | NOT NULL | Seeded at DB init. |

---

## 3. billing-service

**Database:** `credvault_billing`  
**Tables:** `Bills` · `PaymentSchedules` · `RewardTiers` · `RewardAccounts` · `RewardTransactions`

> **Why rewards tables live here:** rewards-service was dropped in v2. Reward earning is a direct side-effect of a bill being paid, so billing-service publishes the `RewardEarned` event and owns the reward state. No extra service, no extra database hop.

### Bills

| Column | Type | Constraint | Notes |
|---|---|---|---|
| **Id** | `uniqueidentifier` | PK | `NEWID()` |
| **UserId** | `uniqueidentifier` | NOT NULL | ✦ Card owner. |
| **CardId** | `uniqueidentifier` | NOT NULL | ✦ Card this bill belongs to. |
| TotalAmount | `decimal(18,2)` | NOT NULL | Full outstanding amount for the period. |
| MinimumDue | `decimal(18,2)` | NOT NULL | Minimum payment to avoid late fee. |
| AmountPaid | `decimal(18,2)` | NOT NULL DEF 0 | Running total paid. Updated by PaymentCompleted events. |
| DueDate | `datetimeoffset` | NOT NULL | Payment due date. |
| BillingMonth | `nvarchar(7)` | NOT NULL | `YYYY-MM` period this bill covers. |
| Status | `nvarchar(20)` | NOT NULL | `Pending \| Paid \| Overdue \| PartiallyPaid` |
| IsDeleted | `bit` | NOT NULL DEF 0 | Soft delete. |
| DeletedAt | `datetimeoffset` | NULL | |
| CreatedAt | `datetimeoffset` | NOT NULL | Auto-generated by scheduled job. |
| UpdatedAt | `datetimeoffset` | NULL | |

### PaymentSchedules

| Column | Type | Constraint | Notes |
|---|---|---|---|
| **Id** | `uniqueidentifier` | PK | `NEWID()` |
| **BillId** | `uniqueidentifier` | FK → Bills.Id | Bill being scheduled. |
| **UserId** | `uniqueidentifier` | NOT NULL | ✦ User who scheduled it. |
| Amount | `decimal(18,2)` | NOT NULL | Amount to pay on scheduled date. |
| ScheduledDate | `datetimeoffset` | NOT NULL | Future execution date. |
| Status | `nvarchar(20)` | NOT NULL | `Pending \| Executed \| Cancelled` |
| CreatedAt | `datetimeoffset` | NOT NULL | |
| UpdatedAt | `datetimeoffset` | NULL | |

### RewardTiers

| Column | Type | Constraint | Notes |
|---|---|---|---|
| **Id** | `uniqueidentifier` | PK | `NEWID()` |
| Name | `nvarchar(20)` | UNIQUE NOT NULL | `Silver \| Gold \| Platinum` |
| MinPoints | `int` | NOT NULL | Min lifetime points to reach this tier. |
| CashbackPercent | `decimal(4,2)` | NOT NULL | `0.50 \| 1.00 \| 2.00` |
| CreatedAt | `datetimeoffset` | NOT NULL | Seeded at DB init. |

### RewardAccounts

| Column | Type | Constraint | Notes |
|---|---|---|---|
| **Id** | `uniqueidentifier` | PK | `NEWID()` |
| **UserId** | `uniqueidentifier` | UNIQUE NOT NULL | ✦ One account per user. |
| **TierId** | `uniqueidentifier` | FK → RewardTiers.Id | Current tier. |
| AvailablePoints | `int` | NOT NULL DEF 0 | Points ready to redeem. |
| TotalEarned | `int` | NOT NULL DEF 0 | Lifetime earned — used for tier calculation. |
| CreatedAt | `datetimeoffset` | NOT NULL | |
| UpdatedAt | `datetimeoffset` | NULL | |

### RewardTransactions

| Column | Type | Constraint | Notes |
|---|---|---|---|
| **Id** | `uniqueidentifier` | PK | `NEWID()` |
| **RewardAccountId** | `uniqueidentifier` | FK → RewardAccounts.Id | Account affected. |
| **PaymentId** | `uniqueidentifier` | NULL | ✦ Source payment for Earned rows. |
| Type | `nvarchar(20)` | NOT NULL | `Earned \| Redeemed \| Expired` |
| Points | `int` | NOT NULL | Positive = credit. Negative = debit. |
| Description | `nvarchar(200)` | NULL | e.g. `Earned 25 pts on ₹2,500 payment` |
| CreatedAt | `datetimeoffset` | NOT NULL | Immutable row. |

---

## 4. payment-service

**Database:** `credvault_payments`  
**Tables:** `Payments` · `PaymentSagas` · `Transactions` · `RiskScores` · `FraudAlerts`

> **Why risk and fraud tables live here:** security-service was dropped in v2. Risk evaluation is triggered by payment-service during the Saga (`EvaluateRisk` command) and fraud detection is a direct outcome of a payment failing the risk check. Keeping these tables here avoids a round-trip to a separate service and keeps the Saga self-contained.

### Payments

| Column | Type | Constraint | Notes |
|---|---|---|---|
| **Id** | `uniqueidentifier` | PK | `NEWID()` |
| **UserId** | `uniqueidentifier` | NOT NULL | ✦ Paying user. |
| **CardId** | `uniqueidentifier` | NOT NULL | ✦ Card used. |
| **BillId** | `uniqueidentifier` | NOT NULL | ✦ Bill being paid. |
| Amount | `decimal(18,2)` | NOT NULL | Amount paid in this payment. |
| PaymentType | `nvarchar(20)` | NOT NULL | `Full \| Partial \| Scheduled` |
| Status | `nvarchar(20)` | NOT NULL | `Initiated \| Completed \| Failed \| Reversed` |
| FailureReason | `nvarchar(300)` | NULL | Reason text on failure. |
| IsDeleted | `bit` | NOT NULL DEF 0 | Soft delete. |
| DeletedAt | `datetimeoffset` | NULL | |
| CreatedAt | `datetimeoffset` | NOT NULL | |
| UpdatedAt | `datetimeoffset` | NULL | |

### PaymentSagas

| Column | Type | Constraint | Notes |
|---|---|---|---|
| **Id** | `uniqueidentifier` | PK | Same ID used as MassTransit CorrelationId. |
| **PaymentId** | `uniqueidentifier` | FK → Payments.Id | The payment being coordinated. |
| CurrentState | `nvarchar(50)` | NOT NULL | `Initiated \| RiskCheckPassed \| Processing \| Completed \| Failed` |
| RiskScore | `decimal(5,2)` | NULL | Score from EvaluateRisk (0–100). Replaces security-service call. |
| RewardPointsGranted | `int` | NULL | Points given after completion. |
| CompensationReason | `nvarchar(300)` | NULL | Populated on rollback path. |
| CreatedAt | `datetimeoffset` | NOT NULL | |
| UpdatedAt | `datetimeoffset` | NULL | |

### Transactions

| Column | Type | Constraint | Notes |
|---|---|---|---|
| **Id** | `uniqueidentifier` | PK | `NEWID()` |
| **PaymentId** | `uniqueidentifier` | FK → Payments.Id | Originating payment. |
| **UserId** | `uniqueidentifier` | NOT NULL | ✦ Transaction owner. |
| Amount | `decimal(18,2)` | NOT NULL | |
| Type | `nvarchar(20)` | NOT NULL | `Payment \| Reversal` |
| Description | `nvarchar(300)` | NULL | e.g. `Bill payment — HDFC ****1234` |
| CreatedAt | `datetimeoffset` | NOT NULL | Immutable row. |

### RiskScores

| Column | Type | Constraint | Notes |
|---|---|---|---|
| **Id** | `uniqueidentifier` | PK | `NEWID()` |
| **UserId** | `uniqueidentifier` | NOT NULL | ✦ User being evaluated. |
| **PaymentId** | `uniqueidentifier` | NULL | ✦ Payment checked. |
| Score | `decimal(5,2)` | NOT NULL | 0–100. Higher = riskier. |
| Decision | `nvarchar(20)` | NOT NULL | `AutoApproved \| OTPRequired \| Blocked` |
| CreatedAt | `datetimeoffset` | NOT NULL | Immutable row. |

### FraudAlerts

| Column | Type | Constraint | Notes |
|---|---|---|---|
| **Id** | `uniqueidentifier` | PK | `NEWID()` |
| **UserId** | `uniqueidentifier` | NOT NULL | ✦ Flagged user. |
| **PaymentId** | `uniqueidentifier` | NULL | ✦ Payment that triggered the alert. |
| AlertType | `nvarchar(50)` | NOT NULL | `HighAmount \| UnusualTime \| MultipleCards` |
| RiskScore | `decimal(5,2)` | NOT NULL | Score at detection time. |
| Status | `nvarchar(20)` | NOT NULL | `Open \| Resolved \| FalsePositive` |
| CreatedAt | `datetimeoffset` | NOT NULL | |
| UpdatedAt | `datetimeoffset` | NULL | |

---

## 5. notification-service

**Database:** `credvault_notifications`  
**Tables:** `EmailTemplates` · `NotificationLogs` · `AuditLogs`

> **Why AuditLogs lives here:** audit-service was dropped in v2. Notification-service is already the system's heaviest event consumer (subscribes to 8 events). Adding an AuditLog table alongside NotificationLogs is trivial — same event, two rows written in one handler. No extra service, no extra database.

### EmailTemplates

| Column | Type | Constraint | Notes |
|---|---|---|---|
| **Id** | `uniqueidentifier` | PK | `NEWID()` |
| TemplateKey | `nvarchar(100)` | UNIQUE NOT NULL | `WelcomeEmail \| BillGenerated \| PaymentConfirmation \| OTPEmail \| FraudAlert \| BillDueReminder \| RewardEarned \| CardExpirySoon` |
| Subject | `nvarchar(300)` | NOT NULL | May contain `{{variable}}` placeholders. |
| HtmlBody | `nvarchar(max)` | NOT NULL | HTML template with `{{variable}}` placeholders. |
| CreatedAt | `datetimeoffset` | NOT NULL | |
| UpdatedAt | `datetimeoffset` | NULL | |

### NotificationLogs

| Column | Type | Constraint | Notes |
|---|---|---|---|
| **Id** | `uniqueidentifier` | PK | `NEWID()` |
| **UserId** | `uniqueidentifier` | NOT NULL | ✦ Recipient. |
| RecipientEmail | `nvarchar(256)` | NOT NULL | Email address at send time. |
| TemplateKey | `nvarchar(100)` | NOT NULL | Which template was used. |
| Status | `nvarchar(20)` | NOT NULL | `Sent \| Failed` |
| FailureReason | `nvarchar(300)` | NULL | SendGrid error if failed. |
| CorrelationId | `uniqueidentifier` | NULL | Trace ID from original request. |
| CreatedAt | `datetimeoffset` | NOT NULL | Immutable row. |

### AuditLogs

| Column | Type | Constraint | Notes |
|---|---|---|---|
| **Id** | `uniqueidentifier` | PK | `NEWID()` |
| EventName | `nvarchar(100)` | NOT NULL | RabbitMQ event name or internal action. |
| SourceService | `nvarchar(100)` | NOT NULL | Which service fired the event. |
| **UserId** | `uniqueidentifier` | NULL | ✦ Subject user. NULL for system events. |
| EntityType | `nvarchar(50)` | NULL | `Payment \| Card \| User \| Bill` |
| **EntityId** | `uniqueidentifier` | NULL | ✦ ID of the entity being acted on. |
| Action | `nvarchar(50)` | NOT NULL | `Created \| Updated \| Deleted \| StatusChanged` |
| IsSuccess | `bit` | NOT NULL | 0 for failed operations. |
| CorrelationId | `uniqueidentifier` | NULL | Trace ID from original HTTP request. |
| CreatedAt | `datetimeoffset` | NOT NULL | Immutable event timestamp. |

---

## Table Count Summary

| Service | Database | Tables |
|---|---|---|
| identity-service | credvault_identity | Users, RefreshTokens, OTPCodes |
| card-service | credvault_cards | CreditCards, CardIssuers |
| billing-service | credvault_billing | Bills, PaymentSchedules, RewardTiers, RewardAccounts, RewardTransactions |
| payment-service | credvault_payments | Payments, PaymentSagas, Transactions, RiskScores, FraudAlerts |
| notification-service | credvault_notifications | EmailTemplates, NotificationLogs, AuditLogs |
| **Total** | **5 databases** | **18 tables** |
