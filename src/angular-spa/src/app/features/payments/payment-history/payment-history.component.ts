import { Component, inject, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { AsyncPipe, CurrencyPipe, DatePipe, NgClass, NgFor, NgIf } from '@angular/common';
import { Router } from '@angular/router';
import { PaymentsActions } from '../../../store/payments/payments.actions';
import { selectAllPayments, selectPaymentsLoading } from '../../../store/payments/payments.selectors';
import { Payment, PaymentStatusLabel, PaymentMethodLabel } from '../../../core/models/payment.model';
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
    this.store.dispatch(PaymentsActions.loadPaymentHistory({ page: 1, pageSize: 10 }));
  }

  makePayment(): void {
    this.router.navigate(['/payments/pay']);
  }

  viewPayment(payment: Payment): void {
    this.router.navigate(['/payments', payment.id]);
  }

  getStatusClass(status: PaymentStatusLabel): string {
    const map: Record<PaymentStatusLabel, string> = {
      'Completed': 'badge--completed',
      'Pending': 'badge--processing',
      'Failed': 'badge--failed',
    };
    return map[status] || 'badge--processing';
  }

  getMethodIcon(method: PaymentMethodLabel): string {
    const icons: Record<PaymentMethodLabel, string> = {
      'Bank Account': 'landmark',
      'Debit Card': 'credit-card',
      'Credit Card': 'credit-card',
      'UPI': 'smartphone',
    };
    return icons[method] || 'wallet';
  }
}
