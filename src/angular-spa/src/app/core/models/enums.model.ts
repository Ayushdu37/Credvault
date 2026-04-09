// === Mirrors: CredVault.Shared.Contracts.Enums ===

export enum UserRole {
  User = 0,
  Admin = 1,
}

export enum UserStatus {
  PendingVerification = 0,
  Active = 1,
  Suspended = 2,
  Deactivated = 3,
}

export enum CardIssuer {
  Visa = 0,
  MasterCard = 1,
  Amex = 2,
  RuPay = 3,
  Discover = 4,
  DinersClub = 5,
}

export enum BillStatus {
  Pending = 0,
  Paid = 1,
  Overdue = 2,
  PartiallyPaid = 3,
}

export enum PaymentStatus {
  Processing = 0,
  Completed = 1,
  Failed = 2,
  Refunded = 3,
}

export enum PaymentMethodType {
  UPI = 0,
  BankTransfer = 1,
  DebitCard = 2,
  NetBanking = 3,
}
