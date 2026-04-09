import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { delay, map } from 'rxjs/operators';

export type BillStatus = 'Paid' | 'Due' | 'Overdue' | 'Pending';

export interface BillingStatement {
  id: string;
  statementDate: string;       // ISO date string
  dueDate: string;
  closingBalance: number;
  minimumDue: number;
  status: BillStatus;
  cardLast4: string;
  lineItems: BillingLineItem[];
}

export interface BillingLineItem {
  id: string;
  description: string;
  amount: number;
  date: string;
  category: string;
}

@Injectable({ providedIn: 'root' })
export class BillingService {

  getBills(): Observable<BillingStatement[]> {
    const mockBills: BillingStatement[] = [
      {
        id: 'stmt-2026-03',
        statementDate: '2026-03-31',
        dueDate: '2026-04-18',
        closingBalance: 4250.75,
        minimumDue: 85.00,
        status: 'Due',
        cardLast4: '4242',
        lineItems: [
          { id: 'li-1', description: 'Amazon Web Services', amount: 84.50, date: '2026-03-28', category: 'Software' },
          { id: 'li-2', description: 'Uber Eats', amount: 32.14, date: '2026-03-25', category: 'Dining' },
          { id: 'li-3', description: 'Netflix', amount: 15.99, date: '2026-03-20', category: 'Entertainment' },
          { id: 'li-4', description: 'Starbucks', amount: 6.50, date: '2026-03-18', category: 'Coffee' },
          { id: 'li-5', description: 'Apple iCloud', amount: 2.99, date: '2026-03-15', category: 'Software' },
        ],
      },
      {
        id: 'stmt-2026-02',
        statementDate: '2026-02-28',
        dueDate: '2026-03-18',
        closingBalance: 3150.20,
        minimumDue: 63.00,
        status: 'Paid',
        cardLast4: '4242',
        lineItems: [
          { id: 'li-6', description: 'Swiggy', amount: 45.00, date: '2026-02-22', category: 'Dining' },
          { id: 'li-7', description: 'Spotify', amount: 9.99, date: '2026-02-15', category: 'Entertainment' },
          { id: 'li-8', description: 'Google One', amount: 2.79, date: '2026-02-10', category: 'Software' },
        ],
      },
      {
        id: 'stmt-2026-01',
        statementDate: '2026-01-31',
        dueDate: '2026-02-18',
        closingBalance: 5800.00,
        minimumDue: 116.00,
        status: 'Paid',
        cardLast4: '4242',
        lineItems: [
          { id: 'li-9', description: 'Amazon Purchase', amount: 199.00, date: '2026-01-20', category: 'Shopping' },
          { id: 'li-10', description: 'Flight - Air India', amount: 320.00, date: '2026-01-12', category: 'Travel' },
        ],
      },
      {
        id: 'stmt-2025-12',
        statementDate: '2025-12-31',
        dueDate: '2026-01-18',
        closingBalance: 2100.50,
        minimumDue: 42.00,
        status: 'Overdue',
        cardLast4: '8888',
        lineItems: [
          { id: 'li-11', description: 'Dining Out', amount: 75.00, date: '2025-12-24', category: 'Dining' },
        ],
      },
    ];
    return of(mockBills).pipe(delay(500));
  }

  getBillById(id: string): Observable<BillingStatement | undefined> {
    return this.getBills().pipe(
      map(bills => bills.find(b => b.id === id)),
      delay(200),
    );
  }
}
