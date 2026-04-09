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
  selectLastReferenceNumber,
} from '../../../store/payments/payments.selectors';
import { PaymentMethod } from '../services/payments.service';
import { CardComponent } from '../../../shared/components/card/card.component';
import { ButtonComponent } from '../../../shared/components/button/button.component';
import { SpinnerComponent } from '../../../shared/components/spinner/spinner.component';
import { LucideAngularModule } from 'lucide-angular';
import { Subject, takeUntil } from 'rxjs';

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
  referenceNumber$ = this.store.select(selectLastReferenceNumber);

  paymentMethods: PaymentMethod[] = ['Bank Account', 'Debit Card', 'UPI'];
  billId = '';
  submitted = false;

  form: FormGroup = this.fb.group({
    amount: [null, [Validators.required, Validators.min(1)]],
    method: ['Bank Account', Validators.required],
  });

  ngOnInit(): void {
    this.billId = this.route.snapshot.queryParamMap.get('billId') || '';
    this.store.dispatch(PaymentsActions.clearPaymentResult());

    // Watch for success to mark form as submitted
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

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.store.dispatch(PaymentsActions.submitPayment({
      payload: {
        billId: this.billId,
        amount: this.amountControl?.value,
        method: this.methodControl?.value,
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

  getMethodIcon(method: string): string {
    const icons: Record<string, string> = {
      'Bank Account': 'landmark',
      'Debit Card':   'credit-card',
      'UPI':          'smartphone',
    };
    return icons[method] || 'wallet';
  }
}
