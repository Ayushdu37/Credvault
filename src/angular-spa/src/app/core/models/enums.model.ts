// === Mirrors: CredVault.Shared.Contracts.Enums ===
// All enums exactly match backend definitions

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
  CreditCard = 4
}

export enum RewardTierName {
  Silver = 0,
  Gold = 1,
  Platinum = 2,
}

export enum RewardTransactionType {
  Earned = 0,
  Redeemed = 1,
  Expired = 2,
}

export enum NotificationType {
  PaymentSuccess = 0,
  PaymentFailed = 1,
  BillGenerated = 2,
  BillOverdue = 3,
  RewardEarned = 4,
  RewardRedeemed = 5,
  General = 6,
}

export enum PaymentScheduleStatus {
  Pending = 0,
  Executed = 1,
  Cancelled = 2,
  Failed = 3,
}

export enum OTPPurpose {
  Login = 0,
  Payment = 1,
  PasswordReset = 2,
}
