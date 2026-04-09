import { Injectable } from '@angular/core';
import { Observable, forkJoin, map } from 'rxjs';
import { ApiService } from '../../core/services/api.service';
import { CardSummaryResponse } from '../../core/models/card.model';
import { BillResponse } from '../../core/models/billing.model';
import { PaymentResponse } from '../../core/models/payment.model';
import { RewardAccountResponse } from '../../core/models/billing.model';
import { PaginatedResponse } from '../../core/models/api-response.model';
import { DashboardSummary, ActivityItem } from '../../core/models/dashboard.model';

@Injectable({ providedIn: 'root' })
export class DashboardService {

  constructor(private api: ApiService) { }

  getSummary(): Observable<DashboardSummary> {
    // Fetch all dashboard data in parallel
    const cardSummary$ = this.api.get<CardSummaryResponse>('/api/cards/utilization')
      .pipe(map(res => res.data!));

    const recentBills$ = this.api.get<PaginatedResponse<BillResponse>>('/api/bills')
      .pipe(map(res => res.data?.items ?? []));

    const recentPayments$ = this.api.get<PaginatedResponse<PaymentResponse>>('/api/payments')
      .pipe(map(res => res.data?.items ?? []));

    const rewards$ = this.api.get<RewardAccountResponse>('/api/rewards')
      .pipe(map(res => res.data!));

    return forkJoin({
      cardSummary: cardSummary$,
      recentBills: recentBills$,
      recentPayments: recentPayments$,
      rewards: rewards$,
    }).pipe(
      map(({ cardSummary, recentBills, recentPayments, rewards }) => {
        // Build recent activity from bills and payments
        const activity: ActivityItem[] = [
          ...recentBills.slice(0, 3).map(b => ({
            id: b.id,
            description: `Bill for ${b.billingMonth}`,
            amount: b.totalAmount,
            date: b.dueDate,
            type: 'charge' as const,
          })),
          ...recentPayments.slice(0, 3).map(p => ({
            id: p.id,
            description: `Payment — ${p.transactionReference || p.id.slice(0, 8)}`,
            amount: p.amount,
            date: p.createdAt,
            type: 'payment' as const,
          })),
        ].sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime());

        return {
          cardSummary,
          recentBills: recentBills.slice(0, 5),
          recentPayments: recentPayments.slice(0, 5),
          rewardPoints: rewards.availablePoints,
          // Convenience fields for dashboard template
          totalBalance: cardSummary.totalOutstandingBalance,
          availableCredit: cardSummary.totalAvailableCredit,
          totalCreditLimit: cardSummary.totalCreditLimit,
          recentActivity: activity,
        };
      })
    );
  }
}
