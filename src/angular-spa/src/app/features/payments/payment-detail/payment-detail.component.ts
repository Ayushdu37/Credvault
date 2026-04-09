import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule, CurrencyPipe, DatePipe } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';
import { CardComponent } from '../../../shared/components/card/card.component';
import { ButtonComponent } from '../../../shared/components/button/button.component';

@Component({
  selector: 'app-payment-detail',
  standalone: true,
  imports: [CommonModule, CurrencyPipe, DatePipe, LucideAngularModule, CardComponent, ButtonComponent],
  templateUrl: './payment-detail.component.html',
  styleUrls: ['./payment-detail.component.css']
})
export class PaymentDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  paymentId = '';

  // Mock payment detail
  payment: any = null;

  private mockPayments: any[] = [
    {
      id: 'pay-001',
      date: '2026-03-15T10:30:00Z',
      amount: 12500.00,
      method: 'Bank Account',
      status: 'Completed',
      referenceNumber: 'TXN-20260315-X7K9',
      description: 'Payment towards Platinum Credit Card',
      cardLast4: '4242',
      billId: 'stmt-2026-02',
      billMonth: 'February 2026',
      paidFrom: 'HDFC Savings Account (••4521)',
    },
    {
      id: 'pay-002',
      date: '2026-02-20T14:15:00Z',
      amount: 8200.00,
      method: 'UPI',
      status: 'Completed',
      referenceNumber: 'TXN-20260220-P3M1',
      description: 'Auto-pay for Gold Rewards Card',
      cardLast4: '8888',
      billId: 'stmt-2026-01',
      billMonth: 'January 2026',
      paidFrom: 'Google Pay UPI (user@okicici)',
    },
    {
      id: 'pay-003',
      date: '2026-01-18T09:45:00Z',
      amount: 5800.00,
      method: 'Debit Card',
      status: 'Failed',
      referenceNumber: 'TXN-20260118-R2N8',
      description: 'Payment towards Platinum Credit Card',
      cardLast4: '4242',
      billId: 'stmt-2025-12',
      billMonth: 'December 2025',
      paidFrom: 'ICICI Debit Card (••6214)',
    },
  ];

  ngOnInit(): void {
    this.paymentId = this.route.snapshot.paramMap.get('id') || '';
    this.payment = this.mockPayments.find(p => p.id === this.paymentId) || this.mockPayments[0];
  }

  goBack(): void {
    this.router.navigate(['/payments']);
  }

  getStatusClass(status: string): string {
    const map: Record<string, string> = {
      Completed: 'status--completed',
      Pending: 'status--pending',
      Failed: 'status--failed',
    };
    return map[status] || '';
  }

  getMethodIcon(method: string): string {
    const icons: Record<string, string> = {
      'Bank Account': 'landmark',
      'Debit Card': 'credit-card',
      'UPI': 'smartphone',
    };
    return icons[method] || 'wallet';
  }
}
