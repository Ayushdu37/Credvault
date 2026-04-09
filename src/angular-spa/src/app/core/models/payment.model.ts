// Mirrors: CredVault.Shared.Contracts.Payment.Responses.PaymentResponse
// Mirrors: CredVault.Shared.Contracts.Payment.Responses.PaymentMethodResponse
import { PaymentStatus, PaymentMethodType } from './enums.model';
export { PaymentStatus, PaymentMethodType };

// === BACKEND DTOs ===

export interface PaymentResponse {
  id: string;
  billId: string;
  cardId: string;
  amount: number;
  paymentMethod: string;
  transactionReference: string | null;
  status: PaymentStatus;
  createdAt: string;
}

export interface PaymentMethodResponse {
  id: string;
  methodType: string;
  displayName: string;
  details: string;
  isDefault: boolean;
  createdAt: string;
}

// === FRONTEND-FRIENDLY TYPES ===

export type PaymentMethodLabel = 'Bank Account' | 'Debit Card' | 'Credit Card' | 'UPI';
export type PaymentStatusLabel = 'Completed' | 'Pending' | 'Failed';

export interface Payment {
  id: string;
  billId: string;
  amount: number;
  method: PaymentMethodLabel;
  status: PaymentStatusLabel;
  date: string;
  referenceNumber: string;
  description: string;
}

export interface PaymentMethod {
  id: string;
  type: 'bank_account' | 'debit_card' | 'credit_card' | 'upi';
  label: string;
  details: string;
  icon: string;
  isDefault: boolean;
  addedOn: string;
}

// === MAPPERS ===

const methodTypeToLabel: Record<string, PaymentMethodLabel | 'Credit Card'> = {
  'UPI': 'UPI',
  'BankTransfer': 'Bank Account',
  'DebitCard': 'Debit Card',
  'NetBanking': 'Bank Account',
  'CreditCard': 'Credit Card',
};

const statusToLabel: Record<PaymentStatus, PaymentStatusLabel> = {
  [PaymentStatus.Completed]: 'Completed',
  [PaymentStatus.Processing]: 'Pending',
  [PaymentStatus.Failed]: 'Failed',
  [PaymentStatus.Refunded]: 'Completed',
};

export function mapPaymentResponseToPayment(payment: PaymentResponse): Payment {
  return {
    id: payment.id,
    billId: payment.billId,
    amount: payment.amount,
    method: (methodTypeToLabel[payment.paymentMethod] as PaymentMethodLabel) || 'Bank Account',
    status: statusToLabel[payment.status] || 'Pending',
    date: payment.createdAt,
    referenceNumber: payment.transactionReference || `REF${payment.id.slice(0, 8).toUpperCase()}`,
    description: `Payment for bill ${payment.billId.slice(0, 8)}`,
  };
}

const methodTypeMap: Record<string, 'bank_account' | 'debit_card' | 'upi' | 'credit_card'> = {
  'UPI': 'upi',
  'BankTransfer': 'bank_account',
  'DebitCard': 'debit_card',
  'NetBanking': 'bank_account',
  'CreditCard': 'credit_card',
};

const methodIconMap: Record<string, string> = {
  'bank_account': 'landmark',
  'debit_card': 'credit-card',
  'credit_card': 'credit-card',
  'upi': 'smartphone',
};

export function mapPaymentMethodResponseToPaymentMethod(pm: PaymentMethodResponse): PaymentMethod {
  const type = methodTypeMap[pm.methodType] || 'bank_account';
  return {
    id: pm.id,
    type,
    label: pm.displayName,
    details: pm.details,
    icon: methodIconMap[type] || 'wallet',
    isDefault: pm.isDefault,
    addedOn: pm.createdAt,
  };
}

// === REQUEST MODELS ===

export interface MakePaymentRequest {
  billId: string;
  cardId: string;
  amount: number;
  paymentMethodId: string;
}

export interface AddPaymentMethodRequest {
  methodType: number;
  displayName: string;
  details: string;
}
