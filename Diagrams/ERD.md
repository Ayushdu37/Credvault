# CredVault — Logic ER Diagram (v2)

```mermaid
erDiagram
    %% ==========================================
    %% 1. IDENTITY SERVICE (credvault_identity)
    %% ==========================================
    USERS {
        uniqueidentifier Id PK
        nvarchar Email
        nvarchar PasswordHash
        nvarchar FirstName
        nvarchar LastName
        nvarchar Role
        bit IsEmailVerified
        bit IsActive
        datetimeoffset CreatedAt
    }

    REFRESH_TOKENS {
        uniqueidentifier Id PK
        uniqueidentifier UserId FK
        nvarchar Token
        datetimeoffset ExpiresAt
        bit IsRevoked
    }

    OTP_CODES {
        uniqueidentifier Id PK
        uniqueidentifier UserId FK
        nvarchar CodeHash
        nvarchar Purpose
        datetimeoffset ExpiresAt
        bit IsUsed
    }

    USERS ||--o{ REFRESH_TOKENS : "has"
    USERS ||--o{ OTP_CODES : "receives"

    %% ==========================================
    %% 2. CARD SERVICE (credvault_cards)
    %% ==========================================
    CARD_ISSUERS {
        uniqueidentifier Id PK
        nvarchar Name
        tinyint CardLength
        nvarchar BinPrefixes
    }

    CREDIT_CARDS {
        uniqueidentifier Id PK
        uniqueidentifier UserId "✦"
        nvarchar MaskedNumber
        nvarchar CardholderName
        tinyint ExpiryMonth
        smallint ExpiryYear
        uniqueidentifier IssuerId FK
        decimal CreditLimit
        decimal OutstandingBalance
        bit IsDefault
        bit IsVerified
    }

    CARD_ISSUERS ||--o{ CREDIT_CARDS : "issues"

    %% ==========================================
    %% 3. BILLING SERVICE (credvault_billing)
    %% ==========================================
    BILLS {
        uniqueidentifier Id PK
        uniqueidentifier UserId "✦"
        uniqueidentifier CardId "✦"
        decimal TotalAmount
        decimal MinimumDue
        decimal AmountPaid
        datetimeoffset DueDate
        nvarchar BillingMonth
        nvarchar Status
    }

    PAYMENT_SCHEDULES {
        uniqueidentifier Id PK
        uniqueidentifier BillId FK
        uniqueidentifier UserId "✦"
        decimal Amount
        datetimeoffset ScheduledDate
        nvarchar Status
    }

    REWARD_TIERS {
        uniqueidentifier Id PK
        nvarchar Name
        int MinPoints
        decimal CashbackPercent
    }

    REWARD_ACCOUNTS {
        uniqueidentifier Id PK
        uniqueidentifier UserId "✦"
        uniqueidentifier TierId FK
        int AvailablePoints
        int TotalEarned
    }

    REWARD_TRANSACTIONS {
        uniqueidentifier Id PK
        uniqueidentifier RewardAccountId FK
        uniqueidentifier PaymentId "✦"
        nvarchar Type
        int Points
    }

    BILLS ||--o{ PAYMENT_SCHEDULES : "has"
    REWARD_TIERS ||--o{ REWARD_ACCOUNTS : "categorizes"
    REWARD_ACCOUNTS ||--o{ REWARD_TRANSACTIONS : "logs"

    %% ==========================================
    %% 4. PAYMENT SERVICE (credvault_payments)
    %% ==========================================
    PAYMENTS {
        uniqueidentifier Id PK
        uniqueidentifier UserId "✦"
        uniqueidentifier CardId "✦"
        uniqueidentifier BillId "✦"
        decimal Amount
        nvarchar PaymentType
        nvarchar Status
    }

    PAYMENT_SAGAS {
        uniqueidentifier Id PK
        uniqueidentifier PaymentId FK
        nvarchar CurrentState
        decimal RiskScore
        int RewardPointsGranted
    }

    TRANSACTIONS {
        uniqueidentifier Id PK
        uniqueidentifier PaymentId FK
        uniqueidentifier UserId "✦"
        decimal Amount
        nvarchar Type
    }

    RISK_SCORES {
        uniqueidentifier Id PK
        uniqueidentifier UserId "✦"
        uniqueidentifier PaymentId "✦"
        decimal Score
        nvarchar Decision
    }

    FRAUD_ALERTS {
        uniqueidentifier Id PK
        uniqueidentifier UserId "✦"
        uniqueidentifier PaymentId "✦"
        nvarchar AlertType
        nvarchar Status
    }

    PAYMENTS ||--o| PAYMENT_SAGAS : "triggers"
    PAYMENTS ||--o{ TRANSACTIONS : "records"
    PAYMENTS ||--o| RISK_SCORES : "evaluated_by"
    PAYMENTS ||--o| FRAUD_ALERTS : "flags"

    %% ==========================================
    %% 5. NOTIFICATION SERVICE (credvault_notifications)
    %% ==========================================
    EMAIL_TEMPLATES {
        uniqueidentifier Id PK
        nvarchar TemplateKey
        nvarchar Subject
        nvarchar HtmlBody
    }

    NOTIFICATION_LOGS {
        uniqueidentifier Id PK
        uniqueidentifier UserId "✦"
        nvarchar RecipientEmail
        nvarchar TemplateKey
        nvarchar Status
    }

    AUDIT_LOGS {
        uniqueidentifier Id PK
        uniqueidentifier UserId "✦"
        nvarchar EventName
        nvarchar SourceService
        nvarchar EntityType
        uniqueidentifier EntityId "✦"
        nvarchar Action
        bit IsSuccess
    }

    %% ==========================================
    %% LOGICAL CROSS-SERVICE RELATIONSHIPS
    %% ==========================================
    USERS ..o{ CREDIT_CARDS : "logical_owner"
    CREDIT_CARDS ..o{ BILLS : "logical_source"
    BILLS ..o{ PAYMENTS : "logical_target"
    PAYMENTS ..o{ REWARD_TRANSACTIONS : "logical_trigger"
```
