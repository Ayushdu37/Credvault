import { Component, inject, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { ActivatedRoute, Router } from '@angular/router';
import { AsyncPipe, CurrencyPipe, DatePipe, NgClass, NgFor, NgIf } from '@angular/common';
import { map, switchMap } from 'rxjs';
import { BillingActions } from '../../../store/billing/billing.actions';
import { selectBillById, selectBillingLoading } from '../../../store/billing/billing.selectors';
import { BillingStatement, BillStatus } from '../services/billing.service';
import { CardComponent } from '../../../shared/components/card/card.component';
import { ButtonComponent } from '../../../shared/components/button/button.component';
import { SpinnerComponent } from '../../../shared/components/spinner/spinner.component';
import { LucideAngularModule } from 'lucide-angular';
import { ModalService } from '../../../shared/components/modal/modal.service';
import { ScheduleAutopayModalComponent, ScheduleAutopayData } from '../schedule-autopay-modal/schedule-autopay-modal.component';

@Component({
  selector: 'app-bill-detail',
  standalone: true,
  imports: [
    AsyncPipe, CurrencyPipe, DatePipe, NgFor, NgIf, NgClass,
    CardComponent, ButtonComponent, SpinnerComponent, LucideAngularModule,
  ],
  templateUrl: './bill-detail.component.html',
  styleUrls: ['./bill-detail.component.css'],
})
export class BillDetailComponent implements OnInit {
  private store = inject(Store);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private modalService = inject(ModalService);

  loading$ = this.store.select(selectBillingLoading);

  bill$ = this.route.paramMap.pipe(
    map(params => params.get('id') || ''),
    switchMap(id => this.store.select(selectBillById(id)))
  );

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id') || '';
    this.store.dispatch(BillingActions.loadBillDetail({ id }));
  }

  goBack(): void {
    this.router.navigate(['/billing']);
  }

  payBill(bill: BillingStatement): void {
    this.router.navigate(['/payments/pay'], { queryParams: { billId: bill.id } });
  }

  scheduleAutopay(bill: BillingStatement): void {
    const dialogRef = this.modalService.openCustom<ScheduleAutopayModalComponent, ScheduleAutopayData>(
      ScheduleAutopayModalComponent,
      {
        cardId: 'card-1', // Mock ID as it's not present on BillingStatement
        cardLast4: bill.cardLast4
      }
    );

    dialogRef.closed.subscribe((result: any) => {
      if (result?.success) {
        // Here we would typically dispatch an action like:
        // this.store.dispatch(BillingActions.configureAutopay({ payload: result.autopayConfig }));
        // Mocking refresh to sync parity
        this.store.dispatch(BillingActions.loadBillDetail({ id: bill.id }));
      }
    });
  }

  // Mock scheduled payment for this bill
  scheduledPayment = {
    id: 'sched-001',
    amount: 85.00,
    scheduledDate: '2026-04-15',
    method: 'HDFC Bank Account (••4521)',
  };

  // Mock payments made against this bill
  billPayments = [
    { id: 'pay-001', date: '2026-03-20', amount: 2000.00, method: 'Bank Account', status: 'Completed', reference: 'TXN-20260320-001' },
    { id: 'pay-002', date: '2026-03-28', amount: 1500.00, method: 'UPI', status: 'Completed', reference: 'TXN-20260328-002' },
  ];

  cancelScheduledPayment(): void {
    const dialogRef = this.modalService.openConfirm({
      title: 'Cancel Scheduled Payment',
      content: `Are you sure you want to cancel the scheduled payment of $${this.scheduledPayment.amount.toFixed(2)} on ${this.scheduledPayment.scheduledDate}?`,
      confirmText: 'Cancel Payment',
      cancelText: 'Keep It',
      danger: true
    });
    dialogRef.subscribe(confirmed => {
      if (confirmed) {
        this.scheduledPayment = null as any;
      }
    });
  }

  getStatusClass(status: BillStatus): string {
    const map: Record<BillStatus, string> = {
      Paid: 'badge--paid',
      Due: 'badge--due',
      Overdue: 'badge--overdue',
      Pending: 'badge--pending',
    };
    return map[status];
  }

  getCategoryIcon(category: string): string {
    const icons: Record<string, string> = {
      Software: 'monitor',
      Dining: 'utensils',
      Entertainment: 'tv',
      Coffee: 'coffee',
      Shopping: 'shopping-bag',
      Travel: 'plane',
    };
    return icons[category] || 'receipt';
  }
}
