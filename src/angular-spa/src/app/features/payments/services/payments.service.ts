import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { delay, map } from 'rxjs/operators';

export type PaymentStatus = 'Completed' | 'Pending' | 'Failed';
export type PaymentMethod = 'Bank Account' | 'Debit Card' | 'UPI';

export interface Payment {
  id: string;
  billId: string;
  amount: number;
  method: PaymentMethod;
  status: PaymentStatus;
  date: string;
  referenceNumber: string;
  description: string;
}

export interface SubmitPaymentPayload {
  billId: string;
  amount: number;
  method: PaymentMethod;
}

@Injectable({ providedIn: 'root' })
export class PaymentsService {

  getPaymentHistory(): Observable<Payment[]> {
    const mockPayments: Payment[] = [
      {
        id: 'pay-001',
        billId: 'stmt-2026-02',
        amount: 3150.20,
        method: 'Bank Account',
        status: 'Completed',
        date: '2026-03-15T10:22:00Z',
        referenceNumber: 'REF20260315ABC',
        description: 'Payment for Feb 2026 Statement',
      },
      {
        id: 'pay-002',
        billId: 'stmt-2026-01',
        amount: 116.00,
        method: 'UPI',
        status: 'Completed',
        date: '2026-02-14T14:05:00Z',
        referenceNumber: 'REF20260214DEF',
        description: 'Minimum due for Jan 2026 Statement',
      },
      {
        id: 'pay-003',
        billId: 'stmt-2025-12',
        amount: 42.00,
        method: 'Debit Card',
        status: 'Failed',
        date: '2026-01-10T09:30:00Z',
        referenceNumber: 'REF20260110GHI',
        description: 'Minimum due for Dec 2025 Statement',
      },
    ];
    return of(mockPayments).pipe(delay(500));
  }

  submitPayment(payload: SubmitPaymentPayload): Observable<{ success: boolean; referenceNumber: string; message: string }> {
    const ref = 'REF' + Date.now();
    return of({
      success: true,
      referenceNumber: ref,
      message: `Payment of ₹${payload.amount.toFixed(2)} submitted successfully.`,
    }).pipe(delay(800));
  }
}
