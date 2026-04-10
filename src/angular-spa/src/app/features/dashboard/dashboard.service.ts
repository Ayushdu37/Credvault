import { Injectable } from '@angular/core';
import { Observable, forkJoin, map, tap, catchError, of } from 'rxjs';
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
    console.log('[DashboardService] Fetching summary...');
    // Fetch all dashboard data in parallel
    const cardSummary$ = this.api.get<CardSummaryResponse>('/api/cards/utilization')
      .pipe(
        tap(res => console.log('[DashboardService] Card utilization response:', res)),
        map(res => res.data!),
        catchError(err => {
          console.error('[DashboardService] Card summary failed:', err);
          // Return default values on error
          return of({
            totalCards: 0,
            totalCreditLimit: 0,
            totalOutstandingBalance: 0,
            totalAvailableCredit: 0,
            utilizationPercentage: 0
          } as CardSummaryResponse);
        })
      );

    const recentBills$ = this.api.get<PaginatedResponse<BillResponse>>('/api/bills')
      .pipe(
        tap(res => console.log('[DashboardService] Bills response:', res)),
        map(res => res.data?.items ?? []),
        catchError(err => {
          console.error('[DashboardService] Bills failed:', err);
          return of([] as BillResponse[]);
        })
      );

    const recentPayments$ = this.api.get<PaginatedResponse<PaymentResponse>>('/api/payments')
      .pipe(
        tap(res => console.log('[DashboardService] Payments response:', res)),
        map(res => res.data?.items ?? []),
        catchError(err => {
          console.error('[DashboardService] Payments failed:', err);
          return of([] as PaymentResponse[]);
        })
      );

    const rewards$ = this.api.get<RewardAccountResponse>('/api/rewards')
      .pipe(
        tap(res => console.log('[DashboardService] Rewards response:', res)),
        map(res => res.data!),
        catchError(err => {
          console.warn('[DashboardService] Rewards failed (optional), using default:', err);
          // Return default reward points on error
          return of({ availablePoints: 0 } as RewardAccountResponse);
        })
      );

    return forkJoin({
      cardSummary: cardSummary$,
      recentBills: recentBills$,
      recentPayments: recentPayments$,
      rewards: rewards$,
    }).pipe(
      map(({ cardSummary, recentBills, recentPayments, rewards }) => {
        console.log('[DashboardService] All data loaded:', { cardSummary, recentBills, recentPayments, rewards });
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
      }),
      catchError(error => {
        console.error('[DashboardService] Error in getSummary:', error);
        throw error;
      })
    );
  }
}
