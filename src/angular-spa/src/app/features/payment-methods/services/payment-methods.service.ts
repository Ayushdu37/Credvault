import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { delay } from 'rxjs/operators';

export interface PaymentMethod {
  id: string;
  type: 'bank_account' | 'debit_card' | 'upi';
  label: string;
  details: string;
  icon: string;
  isDefault: boolean;
  addedOn: string;
}

@Injectable({ providedIn: 'root' })
export class PaymentMethodsService {

  private mockMethods: PaymentMethod[] = [
    {
      id: 'pm-001',
      type: 'bank_account',
      label: 'HDFC Savings Account',
      details: 'XXXX XXXX 4521',
      icon: 'landmark',
      isDefault: true,
      addedOn: '2024-08-15T10:00:00Z',
    },
    {
      id: 'pm-002',
      type: 'bank_account',
      label: 'SBI Current Account',
      details: 'XXXX XXXX 7893',
      icon: 'landmark',
      isDefault: false,
      addedOn: '2024-10-22T14:30:00Z',
    },
    {
      id: 'pm-003',
      type: 'debit_card',
      label: 'ICICI Debit Card',
      details: '•••• 6214',
      icon: 'credit-card',
      isDefault: false,
      addedOn: '2024-12-01T09:15:00Z',
    },
    {
      id: 'pm-004',
      type: 'upi',
      label: 'Google Pay UPI',
      details: 'user@okicici',
      icon: 'smartphone',
      isDefault: false,
      addedOn: '2025-01-10T16:45:00Z',
    },
    {
      id: 'pm-005',
      type: 'upi',
      label: 'PhonePe UPI',
      details: 'user@ybl',
      icon: 'smartphone',
      isDefault: false,
      addedOn: '2025-02-20T11:00:00Z',
    },
  ];

  getPaymentMethods(): Observable<PaymentMethod[]> {
    return of(this.mockMethods).pipe(delay(400));
  }

  deletePaymentMethod(id: string): Observable<string> {
    this.mockMethods = this.mockMethods.filter(m => m.id !== id);
    return of(id).pipe(delay(300));
  }

  setDefault(id: string): Observable<PaymentMethod[]> {
    this.mockMethods = this.mockMethods.map(m => ({
      ...m,
      isDefault: m.id === id,
    }));
    return of(this.mockMethods).pipe(delay(300));
  }
}
