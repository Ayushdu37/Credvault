import { Component, inject, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { ActivatedRoute, Router } from '@angular/router';
import { AsyncPipe, CurrencyPipe, DatePipe, NgClass, NgFor, NgIf } from '@angular/common';
import { map, switchMap } from 'rxjs';
import { BillingActions } from '../../../store/billing/billing.actions';
import { selectBillById, selectBillingLoading } from '../../../store/billing/billing.selectors';
import { BillingStatement, BillStatusLabel, PaymentScheduleResponse } from '../../../core/models/billing.model';
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
        cardId: 'card-1',
        cardLast4: bill.cardLast4
      }
    );

    dialogRef.closed.subscribe((result: any) => {
      if (result?.success) {
        this.store.dispatch(BillingActions.loadBillDetail({ id: bill.id }));
      }
    });
  }

  scheduledPayment: PaymentScheduleResponse | null = null;

  billPayments: any[] = [];

  cancelScheduledPayment(): void {
    if (!this.scheduledPayment) return;
    const dialogRef = this.modalService.openConfirm({
      title: 'Cancel Scheduled Payment',
      content: `Are you sure you want to cancel the scheduled payment?`,
      confirmText: 'Cancel Payment',
      cancelText: 'Keep It',
      danger: true
    });
    dialogRef.subscribe(confirmed => {
      if (confirmed) {
        this.store.dispatch(BillingActions.cancelScheduledPayment({ scheduleId: this.scheduledPayment!.id }));
        this.scheduledPayment = null;
      }
    });
  }

  getStatusClass(status: BillStatusLabel): string {
    const map: Record<BillStatusLabel, string> = {
      'Paid': 'badge--paid',
      'Pending': 'badge--pending',
      'Overdue': 'badge--overdue',
      'Due': 'badge--partially-paid',
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
