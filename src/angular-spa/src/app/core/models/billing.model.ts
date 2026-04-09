// Mirrors: CredVault.Shared.Contracts.Billing.Responses.BillResponse
// Mirrors: CredVault.Shared.Contracts.Billing.Responses.PaymentScheduleResponse
// Mirrors: CredVault.Shared.Contracts.Billing.Responses.RewardAccountResponse
// Mirrors: CredVault.Shared.Contracts.Billing.Responses.RewardTransactionResponse
import { BillStatus, RewardTransactionType } from './enums.model';
export { BillStatus, RewardTransactionType };

// === BACKEND DTOs ===

export interface BillResponse {
  id: string;
  cardId: string;
  totalAmount: number;
  minimumDue: number;
  amountPaid: number;
  remaining: number;
  dueDate: string;
  billingMonth: string;
  status: BillStatus;
  createdAt: string;
}

export interface PaymentScheduleResponse {
  id: string;
  billId: string;
  scheduledDate: string;
  amount: number;
  status: string;
  createdAt: string;
}

export interface RewardAccountResponse {
  id: string;
  tierName: string;
  cashbackPercent: number;
  availablePoints: number;
  totalEarned: number;
  pointsToNextTier: number;
  nextTierName: string;
}

export interface RewardTransactionResponse {
  id: string;
  type: RewardTransactionType;
  points: number;
  description: string | null;
  createdAt: string;
}

// === FRONTEND-FRIENDLY TYPES (what templates expect) ===

export type BillStatusLabel = 'Paid' | 'Due' | 'Overdue' | 'Pending';

export interface BillingLineItem {
  id: string;
  description: string;
  amount: number;
  date: string;
  category: string;
}

export interface BillingStatement {
  id: string;
  statementDate: string;
  dueDate: string;
  closingBalance: number;
  minimumDue: number;
  status: BillStatusLabel;
  cardLast4: string;
  lineItems: BillingLineItem[];
}

export interface RewardTier {
  id: string;
  name: string;
  minPoints: number;
  cashbackPercent: number;
}

export interface RewardAccount {
  id: string;
  userId: string;
  tierId: string;
  tierName: string;
  availablePoints: number;
  totalEarned: number;
  cashbackPercent: number;
}

export interface RewardTransaction {
  id: string;
  rewardAccountId: string;
  paymentId: string | null;
  type: 'Earned' | 'Redeemed';
  points: number;
  description: string;
  createdAt: string;
}

// === MAPPERS ===

export function mapBillResponseToStatement(bill: BillResponse, last4 = '0000'): BillingStatement {
  const statusMap: Record<BillStatus, BillStatusLabel> = {
    [BillStatus.Paid]: 'Paid',
    [BillStatus.Pending]: 'Pending',
    [BillStatus.Overdue]: 'Overdue',
    [BillStatus.PartiallyPaid]: 'Due',
  };
  return {
    id: bill.id,
    statementDate: bill.billingMonth + '-01', // approximate
    dueDate: bill.dueDate,
    closingBalance: bill.totalAmount,
    minimumDue: bill.minimumDue,
    status: statusMap[bill.status] || 'Pending',
    cardLast4: last4,
    lineItems: [],
  };
}

export function mapRewardAccountResponse(response: RewardAccountResponse): RewardAccount {
  return {
    id: response.id,
    userId: '',
    tierId: '',
    tierName: response.tierName,
    availablePoints: response.availablePoints,
    totalEarned: response.totalEarned,
    cashbackPercent: response.cashbackPercent,
  };
}

export function mapRewardTransactionResponse(tx: RewardTransactionResponse): RewardTransaction {
  const typeLabel = tx.type === RewardTransactionType.Earned ? 'Earned' :
    tx.type === RewardTransactionType.Redeemed ? 'Redeemed' : 'Earned';
  return {
    id: tx.id,
    rewardAccountId: '',
    paymentId: null,
    type: typeLabel as 'Earned' | 'Redeemed',
    points: tx.type === RewardTransactionType.Redeemed ? -Math.abs(tx.points) : tx.points,
    description: tx.description || '',
    createdAt: tx.createdAt,
  };
}

// === REQUEST MODELS ===

export interface SchedulePaymentRequest {
  billId: string;
  amount: number;
  scheduledDate: string;
}

export interface RedeemRewardsRequest {
  points: number;
}

export interface GenerateBillRequest {
  cardId: string;
  billingMonth: string;
}
