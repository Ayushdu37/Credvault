import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogRef, DIALOG_DATA } from '@angular/cdk/dialog';
import { LucideAngularModule } from 'lucide-angular';
import { ButtonComponent } from '../../../shared/components/button/button.component';
import { Store } from '@ngrx/store';
import { PaymentMethodsActions } from '../../../store/payment-methods/payment-methods.actions';
import { AddPaymentMethodRequest, PaymentMethodType } from '../../../core/models/payment.model';

@Component({
  selector: 'app-add-method-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, LucideAngularModule, ButtonComponent],
  templateUrl: './add-method-modal.component.html',
  styleUrls: ['./add-method-modal.component.css']
})
export class AddMethodModalComponent {
  methodForm: FormGroup;
  loading = false;
  methodType: 'Card' | 'Bank' = 'Card';
  cardSubType: 'Debit' | 'Credit' = 'Debit';

  constructor(
    public dialogRef: DialogRef<any>,
    @Inject(DIALOG_DATA) public data: any,
    private fb: FormBuilder,
    private store: Store
  ) {
    this.methodForm = this.fb.group({
      cardName: ['', [Validators.required]],
      cardNumber: ['', [Validators.required, Validators.pattern('^[0-9]{16}$')]],
      expiry: ['', [Validators.required, Validators.pattern('^(0[1-9]|1[0-2])\/[0-9]{2}$')]],
      cvv: ['', [Validators.required, Validators.pattern('^[0-9]{3,4}$')]]
    });
  }

  setMethodType(type: 'Card' | 'Bank') {
    this.methodType = type;
    if (type === 'Bank') {
      this.methodForm = this.fb.group({
        accountName: ['', [Validators.required]],
        routingNumber: ['', [Validators.required, Validators.pattern('^[0-9]{9}$')]],
        accountNumber: ['', [Validators.required, Validators.pattern('^[0-9]{8,12}$')]]
      });
    } else {
      this.methodForm = this.fb.group({
        cardName: ['', [Validators.required]],
        cardNumber: ['', [Validators.required, Validators.pattern('^[0-9]{16}$')]],
        expiry: ['', [Validators.required, Validators.pattern('^(0[1-9]|1[0-2])\/[0-9]{2}$')]],
        cvv: ['', [Validators.required, Validators.pattern('^[0-9]{3,4}$')]]
      });
    }
  }

  getCardNetwork(number: string): string {
    if (!number) return 'Card';
    if (number.startsWith('4')) return 'Visa';
    if (number.startsWith('5')) return 'Mastercard';
    if (number.startsWith('3')) return 'Amex';
    if (number.startsWith('6')) return 'Discover';
    return 'Card';
  }

  close() {
    this.dialogRef.close();
  }

  onSubmit() {
    if (this.methodForm.invalid) {
      this.methodForm.markAllAsTouched();
      return;
    }

    const formVal = this.methodForm.value;
    let payload: AddPaymentMethodRequest;

    if (this.methodType === 'Card') {
      const last4 = formVal.cardNumber.slice(-4);
      const network = this.getCardNetwork(formVal.cardNumber);
      const isCredit = this.cardSubType === 'Credit';
      
      payload = {
        methodType: isCredit ? PaymentMethodType.CreditCard : PaymentMethodType.DebitCard,
        displayName: formVal.cardName,
        details: `${network} ${this.cardSubType} •••• ${last4}`
      };
    } else {
      const last4 = formVal.accountNumber.slice(-4);
      payload = {
        methodType: PaymentMethodType.BankTransfer,
        displayName: formVal.accountName,
        details: `Bank Account •••• ${last4}`
      };
    }

    this.loading = true;
    this.store.dispatch(PaymentMethodsActions.addMethod({ payload }));
    
    setTimeout(() => {
      this.loading = false;
      this.dialogRef.close({ success: true });
    }, 1000);
  }
}
