import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { Store } from '@ngrx/store';
import { ActivatedRoute, Router } from '@angular/router';
import { AsyncPipe, NgIf, NgFor, NgClass, DecimalPipe, CurrencyPipe } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators, FormsModule } from '@angular/forms';
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
import { Subject, takeUntil, map, take } from 'rxjs';
import { PAYMENT_CATEGORIES, PaymentCategory, QUICK_AMOUNTS } from '../payment-categories';
import { CardsService } from '../../cards/services/cards.service';
import { CardResponse, CreditCard, mapCardResponseToCreditCard } from '../../../core/models/card.model';
import { selectUser } from '../../../store/auth/auth.selectors';
import { AuthService } from '../../../core/services/auth.service';
import { OTPPurpose } from '../../../core/models/enums.model';
import { UserProfile } from '../../../core/models/auth.model';

@Component({
  selector: 'app-pay-bill',
  standalone: true,
  imports: [
    AsyncPipe, NgIf, NgFor, NgClass, DecimalPipe, CurrencyPipe,
    ReactiveFormsModule, FormsModule,
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
  private cardsService = inject(CardsService);
  private authService = inject(AuthService);
  private destroy$ = new Subject<void>();

  user$ = this.store.select(selectUser);
  submitting$ = this.store.select(selectPaymentsSubmitting);
  successMessage$ = this.store.select(selectPaymentSuccessMessage);
  error$ = this.store.select(selectPaymentsError);

  referenceNumber$ = this.successMessage$.pipe(
    map(msg => msg ? `REF-${Date.now().toString(36).toUpperCase()}` : null)
  );

  billId = '';
  cardId = '';
  submitted = false;

  // OTP Modal State
  showOtpModal = false;
  otpCode = '';
  otpError = '';
  sendingOtp = false;
  verifyingOtp = false;
  currentUser: UserProfile | null = null;
  pendingPayload: any = null;

  categories: PaymentCategory[] = PAYMENT_CATEGORIES;
  quickAmounts: number[] = QUICK_AMOUNTS;
  selectedCategory: PaymentCategory | null = null;

  userCards: CreditCard[] = [];
  cardsLoading = false;
  selectedCard: CreditCard | null = null;
  selectedPayFromCard: CreditCard | null = null;

  paymentMethods: string[] = ['Credit Card'];

  form: FormGroup = this.fb.group({
    recipient: ['', Validators.required],
    amount: [null, [Validators.required, Validators.min(1)]],
    method: ['Credit Card'],
    selectedCardId: [''],
    operator: [''],
    payFromCardId: ['', Validators.required],
  });

  get recipientControl() { return this.form.get('recipient'); }
  get amountControl() { return this.form.get('amount'); }
  get methodControl() { return this.form.get('method'); }
  get selectedCardIdControl() { return this.form.get('selectedCardId'); }
  get operatorControl() { return this.form.get('operator'); }
  get payFromCardIdControl() { return this.form.get('payFromCardId'); }

  ngOnInit(): void {
    this.billId = this.route.snapshot.queryParamMap.get('billId') || '';
    this.cardId = this.route.snapshot.queryParamMap.get('cardId') || '';
    this.store.dispatch(PaymentsActions.clearPaymentResult());

    this.successMessage$
      .pipe(takeUntil(this.destroy$))
      .subscribe(msg => { if (msg) this.submitted = true; });

    this.loadUserCards();

    this.user$.pipe(takeUntil(this.destroy$)).subscribe(user => {
      this.currentUser = user;
    });

    // Handle category from query params
    const categoryId = this.route.snapshot.queryParamMap.get('category');
    if (categoryId) {
      const cat = this.categories.find(c => c.id === categoryId);
      if (cat) {
        this.selectCategory(cat);
      }
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadUserCards(): void {
    this.cardsLoading = true;
    this.cardsService.getCards(1, 50).pipe(takeUntil(this.destroy$)).subscribe({
      next: (res) => {
        this.userCards = res.items.map(mapCardResponseToCreditCard);
        this.cardsLoading = false;
        if (this.cardId) {
          const preselected = this.userCards.find(c => c.id === this.cardId);
          if (preselected) {
            this.selectedCard = preselected;
            this.selectedCardIdControl?.setValue(preselected.id);
          }
        }
      },
      error: () => {
        this.cardsLoading = false;
      }
    });
  }

  selectCategory(category: PaymentCategory): void {
    this.selectedCategory = category;
    this.selectedCard = null;
    this.selectedPayFromCard = null;
    this.recipientControl?.setValue('');
    this.selectedCardIdControl?.setValue('');
    this.operatorControl?.setValue('');
    this.payFromCardIdControl?.setValue('');

    if (category.usesCardDropdown) {
      this.recipientControl?.clearValidators();
      this.selectedCardIdControl?.setValidators([Validators.required]);
    } else {
      this.recipientControl?.setValidators([Validators.required]);
      this.selectedCardIdControl?.clearValidators();
    }
    this.recipientControl?.updateValueAndValidity();
    this.selectedCardIdControl?.updateValueAndValidity();

    // Dynamic payment methods based on category
    if (category.id === 'credit') {
      this.paymentMethods = ['UPI', 'Bank Account', 'Debit Card'];
      this.methodControl?.setValue('UPI');
    } else {
      this.paymentMethods = ['Credit Card'];
      this.methodControl?.setValue('Credit Card');
    }
  }

  onCardSelected(cardId: string): void {
    this.selectedCardIdControl?.setValue(cardId);
    this.selectedCard = this.userCards.find(c => c.id === cardId) || null;
  }

  getMethodIcon(method: string): string {
    const icons: Record<string, string> = {
      'Bank Account': 'landmark',
      'Debit Card': 'credit-card',
      'UPI': 'smartphone',
      'Credit Card': 'credit-card',
    };
    return icons[method] || 'wallet';
  }

  onPayFromCardSelected(cardId: string): void {
    this.payFromCardIdControl?.setValue(cardId);
    this.selectedPayFromCard = this.userCards.find(c => c.id === cardId) || null;
  }

  isCardMethod(): boolean {
    const method = this.methodControl?.value;
    return method === 'Debit Card' || method === 'Credit Card';
  }

  maskCardNumber(last4: string): string {
    return `•••• •••• •••• ${last4}`;
  }

  setQuickAmount(amount: number): void {
    this.amountControl?.setValue(amount);
  }

  onSubmit(): void {
    if (this.form.invalid || !this.selectedCategory) {
      this.form.markAllAsTouched();
      return;
    }

    const methodMap: Record<string, number> = {
      'UPI': 0,
      'Bank Account': 1,
      'Debit Card': 2,
      'Credit Card': 4,
    };
    const methodStr = this.methodControl?.value || 'UPI';

    let resolvedCardId = this.cardId || '00000000-0000-0000-0000-000000000000';
    if (this.selectedCategory.usesCardDropdown && this.selectedCard) {
      resolvedCardId = this.selectedCard.id;
    }
    if (this.isCardMethod() && this.selectedPayFromCard) {
      resolvedCardId = this.selectedPayFromCard.id;
    }

    const payload = {
      billId: this.billId || crypto.randomUUID(),
      cardId: resolvedCardId,
      amount: this.amountControl?.value,
      paymentMethod: methodMap[methodStr] ?? 0,
      transactionReference: `REF-${Date.now().toString(36).toUpperCase()}`
    };

    this.initiateOtp(payload);
  }

  private initiateOtp(payload: any): void {
    if (!this.currentUser?.email) {
      alert('User email not found. Please log in again.');
      return;
    }

    this.sendingOtp = true;
    this.otpError = '';
    this.pendingPayload = payload;

    this.authService.sendOtp({
      email: this.currentUser.email,
      purpose: 'PaymentVerification'
    }).subscribe({
      next: () => {
        this.sendingOtp = false;
        this.showOtpModal = true;
      },
      error: (err) => {
        this.sendingOtp = false;
        this.otpError = 'Failed to send OTP. Please try again.';
        console.error('OTP Send Error:', err);
      }
    });
  }

  verifyOtpAndPay(): void {
    if (!this.otpCode || this.otpCode.length < 6) {
      this.otpError = 'Please enter a valid 6-digit OTP.';
      return;
    }

    if (!this.currentUser?.email) return;

    this.verifyingOtp = true;
    this.otpError = '';

    this.authService.verifyOtp({
      email: this.currentUser.email,
      otpCode: this.otpCode,
      purpose: 'PaymentVerification'
    }).subscribe({
      next: () => {
        this.verifyingOtp = false;
        this.showOtpModal = false;
        this.store.dispatch(PaymentsActions.submitPayment({ payload: this.pendingPayload }));
      },
      error: (err) => {
        this.verifyingOtp = false;
        this.otpError = 'Invalid or expired OTP. Please check your email.';
      }
    });
  }

  cancelOtp(): void {
    this.showOtpModal = false;
    this.otpCode = '';
    this.otpError = '';
    this.pendingPayload = null;
  }

  goToHistory(): void {
    this.store.dispatch(PaymentsActions.clearPaymentResult());
    this.router.navigate(['/payments']);
  }

  goBack(): void {
    this.router.navigate(['/billing']);
  }
}
