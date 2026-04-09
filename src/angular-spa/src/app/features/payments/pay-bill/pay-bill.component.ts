import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { Store } from '@ngrx/store';
import { ActivatedRoute, Router } from '@angular/router';
import { AsyncPipe, NgIf, NgFor, NgClass } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PaymentsActions } from '../../../store/payments/payments.actions';
import {
  selectPaymentsSubmitting,
  selectPaymentSuccessMessage,
  selectPaymentsError,
} from '../../../store/payments/payments.selectors';
import { CardComponent } from '../../../shared/components/card/card.component';
import { ButtonComponent } from '../../../shared/components/button/button.component';
import { SpinnerComponent } from '../../../shared/components/spinner/spinner.component';
import { LucideAngularModule } from 'lucide-angular';
import { Subject, takeUntil, map } from 'rxjs';

@Component({
  selector: 'app-pay-bill',
  standalone: true,
  imports: [
    AsyncPipe, NgIf, NgFor, NgClass,
    ReactiveFormsModule,
    CardComponent, ButtonComponent, SpinnerComponent, LucideAngularModule,
  ],
  templateUrl: './pay-bill.component.html',
  styleUrls: ['./pay-bill.component.css'],
})
export class PayBillComponent implements OnInit, OnDestroy {
  private store = inject(Store);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  private destroy$ = new Subject<void>();

  submitting$ = this.store.select(selectPaymentsSubmitting);
  successMessage$ = this.store.select(selectPaymentSuccessMessage);
  error$ = this.store.select(selectPaymentsError);

  /** Reference number shown after successful payment submission */
  referenceNumber$ = this.successMessage$.pipe(
    map(msg => msg ? `REF-${Date.now().toString(36).toUpperCase()}` : null)
  );

  billId = '';
  cardId = '';
  submitted = false;

  /** Available payment methods */
  paymentMethods: string[] = ['Bank Account', 'Debit Card', 'UPI'];

  form: FormGroup = this.fb.group({
    amount: [null, [Validators.required, Validators.min(1)]],
    method: ['Bank Account'],
  });

  ngOnInit(): void {
    this.billId = this.route.snapshot.queryParamMap.get('billId') || '';
    this.cardId = this.route.snapshot.queryParamMap.get('cardId') || '';
    this.store.dispatch(PaymentsActions.clearPaymentResult());

    this.successMessage$
      .pipe(takeUntil(this.destroy$))
      .subscribe(msg => { if (msg) this.submitted = true; });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  get amountControl() { return this.form.get('amount'); }
  get methodControl() { return this.form.get('method'); }

  getMethodIcon(method: string): string {
    const icons: Record<string, string> = {
      'Bank Account': 'landmark',
      'Debit Card': 'credit-card',
      'UPI': 'smartphone',
    };
    return icons[method] || 'wallet';
  }

  onSubmit(): void {
    if (this.form.invalid || !this.billId || !this.cardId) {
      this.form.markAllAsTouched();
      return;
    }
    this.store.dispatch(PaymentsActions.submitPayment({
      payload: {
        billId: this.billId,
        cardId: this.cardId,
        amount: this.amountControl?.value,
        paymentMethodId: '', // TODO: wire to payment method selector
      },
    }));
  }

  goToHistory(): void {
    this.store.dispatch(PaymentsActions.clearPaymentResult());
    this.router.navigate(['/payments']);
  }

  goBack(): void {
    this.router.navigate(['/billing']);
  }
}
