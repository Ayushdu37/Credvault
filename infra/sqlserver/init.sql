-- Create databases for each microservice
-- Run this AFTER SQL Server container is healthy

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'credvault_identity')
    CREATE DATABASE credvault_identity;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'credvault_cards')
    CREATE DATABASE credvault_cards;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'credvault_billing')
    CREATE DATABASE credvault_billing;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'credvault_payments')
    CREATE DATABASE credvault_payments;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'credvault_notifications')
    CREATE DATABASE credvault_notifications;
GO

PRINT 'All CredVault databases created successfully!';
GO
