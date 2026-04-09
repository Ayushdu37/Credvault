import { Component, inject, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { AsyncPipe, CurrencyPipe, DatePipe, NgClass, NgFor, NgIf } from '@angular/common';
import { Router } from '@angular/router';
import { PaymentsActions } from '../../../store/payments/payments.actions';
import { selectAllPayments, selectPaymentsLoading } from '../../../store/payments/payments.selectors';
import { Payment, PaymentStatus } from '../services/payments.service';
import { CardComponent } from '../../../shared/components/card/card.component';
import { ButtonComponent } from '../../../shared/components/button/button.component';
import { SpinnerComponent } from '../../../shared/components/spinner/spinner.component';
import { EmptyStateComponent } from '../../../shared/components/empty-state/empty-state.component';
import { LucideAngularModule } from 'lucide-angular';

@Component({
  selector: 'app-payment-history',
  standalone: true,
  imports: [
    AsyncPipe, CurrencyPipe, DatePipe, NgFor, NgIf, NgClass,
    CardComponent, ButtonComponent, SpinnerComponent, EmptyStateComponent,
    LucideAngularModule,
  ],
  templateUrl: './payment-history.component.html',
  styleUrls: ['./payment-history.component.css'],
})
export class PaymentHistoryComponent implements OnInit {
  private store = inject(Store);
  private router = inject(Router);

  payments$ = this.store.select(selectAllPayments);
  loading$ = this.store.select(selectPaymentsLoading);

  ngOnInit(): void {
    this.store.dispatch(PaymentsActions.loadPaymentHistory());
  }

  makePayment(): void {
    this.router.navigate(['/payments/pay']);
  }

  viewPayment(payment: Payment): void {
    this.router.navigate(['/payments', payment.id]);
  }

  getStatusClass(status: PaymentStatus): string {
    const map: Record<PaymentStatus, string> = {
      Completed: 'badge--completed',
      Pending:   'badge--pending',
      Failed:    'badge--failed',
    };
    return map[status];
  }

  getMethodIcon(method: string): string {
    const icons: Record<string, string> = {
      'Bank Account': 'landmark',
      'Debit Card':   'credit-card',
      'UPI':          'smartphone',
    };
    return icons[method] || 'wallet';
  }
}
