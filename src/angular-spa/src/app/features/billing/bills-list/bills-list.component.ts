import { Component, inject, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { AsyncPipe, CurrencyPipe, DatePipe, NgClass, NgFor, NgIf } from '@angular/common';
import { Router } from '@angular/router';
import { BillingActions } from '../../../store/billing/billing.actions';
import { selectAllBills, selectBillingLoading } from '../../../store/billing/billing.selectors';
import { BillingStatement, BillStatusLabel } from '../../../core/models/billing.model';
import { CardComponent } from '../../../shared/components/card/card.component';
import { ButtonComponent } from '../../../shared/components/button/button.component';
import { SpinnerComponent } from '../../../shared/components/spinner/spinner.component';
import { EmptyStateComponent } from '../../../shared/components/empty-state/empty-state.component';
import { LucideAngularModule } from 'lucide-angular';

@Component({
  selector: 'app-bills-list',
  standalone: true,
  imports: [
    AsyncPipe, CurrencyPipe, DatePipe, NgFor, NgIf, NgClass,
    CardComponent, ButtonComponent, SpinnerComponent, EmptyStateComponent,
    LucideAngularModule,
  ],
  templateUrl: './bills-list.component.html',
  styleUrls: ['./bills-list.component.css'],
})
export class BillsListComponent implements OnInit {
  private store = inject(Store);
  private router = inject(Router);

  bills$ = this.store.select(selectAllBills);
  loading$ = this.store.select(selectBillingLoading);

  ngOnInit(): void {
    this.store.dispatch(BillingActions.loadBills({ page: 1, pageSize: 10 }));
  }

  viewBill(bill: BillingStatement): void {
    this.router.navigate(['/billing', bill.id]);
  }

  payBill(bill: BillingStatement, event: Event): void {
    event.stopPropagation();
    this.router.navigate(['/payments/pay'], { queryParams: { billId: bill.id } });
  }

  getStatusClass(status: BillStatusLabel): string {
    const statusMap: Record<BillStatusLabel, string> = {
      'Paid': 'badge--paid',
      'Pending': 'badge--pending',
      'Overdue': 'badge--overdue',
      'Due': 'badge--partially-paid',
    };
    return statusMap[status];
  }

  getUnpaidCount(bills: BillingStatement[]): number {
    return bills.filter(b => b.status !== 'Paid').length;
  }
}
