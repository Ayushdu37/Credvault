import { Injectable } from '@angular/core';
import { delay, Observable, of } from 'rxjs';

export interface DashboardSummary {
  totalCreditLimit: number;
  totalBalance: number;
  availableCredit: number;
  recentActivity: ActivityItem[];
  creditUtilizationData: number[]; // e.g., last 6 months
  balanceTrendData: number[];
  months: string[];
}

export interface ActivityItem {
  id: string;
  description: string;
  amount: number;
  date: string;
  category: string;
  type: 'charge' | 'payment';
}

@Injectable({ providedIn: 'root' })
export class DashboardService {
  getSummary(): Observable<DashboardSummary> {
    const mockData: DashboardSummary = {
      totalCreditLimit: 25000,
      totalBalance: 4250.75,
      availableCredit: 20749.25,
      months: ['Oct', 'Nov', 'Dec', 'Jan', 'Feb', 'Mar'],
      creditUtilizationData: [15, 18, 22, 19, 16, 17], // percentage mapped later
      balanceTrendData: [3200, 3800, 4800, 4100, 3950, 4250.75],
      recentActivity: [
        { id: '1', description: 'Apple Payment', amount: -150.0, date: '2026-04-02T14:32:00Z', category: 'Payment', type: 'payment' },
        { id: '2', description: 'Amazon Web Services', amount: 84.5, date: '2026-04-01T09:12:00Z', category: 'Software', type: 'charge' },
        { id: '3', description: 'Uber Eats', amount: 32.14, date: '2026-03-31T20:45:00Z', category: 'Dining', type: 'charge' },
        { id: '4', description: 'Starbucks', amount: 6.5, date: '2026-03-31T08:15:00Z', category: 'Coffee', type: 'charge' }
      ]
    };
    return of(mockData).pipe(delay(600)); // Simulate network latency
  }
}
