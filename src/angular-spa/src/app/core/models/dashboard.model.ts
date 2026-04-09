// Dashboard aggregation model (no direct backend DTO — aggregates multiple backend responses)
import { CardSummaryResponse } from './card.model';
import { BillResponse } from './billing.model';
import { PaymentResponse } from './payment.model';

export interface ActivityItem {
  id: string;
  description: string;
  amount: number;
  date: string;
  type: 'charge' | 'payment';
}

export interface DashboardSummary {
  cardSummary: CardSummaryResponse;
  recentBills: BillResponse[];
  recentPayments: PaymentResponse[];
  rewardPoints: number;

  // Convenience fields derived from cardSummary (used directly in templates)
  totalBalance: number;
  availableCredit: number;
  totalCreditLimit: number;
  recentActivity: ActivityItem[];
}
