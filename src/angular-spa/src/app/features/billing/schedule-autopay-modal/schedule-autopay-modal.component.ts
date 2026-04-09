import { Component, Inject, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogRef, DIALOG_DATA } from '@angular/cdk/dialog';
import { LucideAngularModule } from 'lucide-angular';
import { ButtonComponent } from '../../../shared/components/button/button.component';
import { Store } from '@ngrx/store';
import { selectAllPaymentMethods } from '../../../store/payment-methods/payment-methods.selectors';
import { PaymentMethodsActions } from '../../../store/payment-methods/payment-methods.actions';

export interface ScheduleAutopayData {
  cardId: string;
  cardLast4: string;
}

@Component({
  selector: 'app-schedule-autopay-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, LucideAngularModule, ButtonComponent],
  templateUrl: './schedule-autopay-modal.component.html',
  styleUrls: ['../../payment-methods/add-method-modal/add-method-modal.component.css', './schedule-autopay-modal.component.css']
})
export class ScheduleAutopayModalComponent implements OnInit {
  private store = inject(Store);

  autopayForm: FormGroup;
  loading = false;
  paymentMethods$ = this.store.select(selectAllPaymentMethods);

  constructor(
    public dialogRef: DialogRef<any>,
    @Inject(DIALOG_DATA) public data: ScheduleAutopayData,
    private fb: FormBuilder
  ) {
    this.autopayForm = this.fb.group({
      paymentMethodId: ['', Validators.required],
      amountPreference: ['full', Validators.required],
      customAmount: [null],
      paymentDate: [1, [Validators.required, Validators.min(1), Validators.max(28)]]
    });
  }

  ngOnInit() {
    this.store.dispatch(PaymentMethodsActions.loadMethods({ page: 1, pageSize: 10 }));

    // Validator logic for custom amount
    this.autopayForm.get('amountPreference')?.valueChanges.subscribe(val => {
      const customCtrl = this.autopayForm.get('customAmount');
      if (val === 'custom') {
        customCtrl?.setValidators([Validators.required, Validators.min(10)]);
      } else {
        customCtrl?.clearValidators();
      }
      customCtrl?.updateValueAndValidity();
    });
  }

  get amountPreferenceControl() { return this.autopayForm.get('amountPreference'); }

  close() {
    this.dialogRef.close();
  }

  onSubmit() {
    if (this.autopayForm.invalid) {
      this.autopayForm.markAllAsTouched();
      return;
    }

    this.loading = true;

    // Simulate API call to save autopay preference
    setTimeout(() => {
      this.loading = false;
      this.dialogRef.close({
        success: true,
        autopayConfig: this.autopayForm.value
      });
    }, 1500);
  }
}
