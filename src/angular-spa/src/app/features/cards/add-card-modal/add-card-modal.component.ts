import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogRef, DIALOG_DATA } from '@angular/cdk/dialog';
import { LucideAngularModule } from 'lucide-angular';
import { ButtonComponent } from '../../../shared/components/button/button.component';

@Component({
  selector: 'app-add-card-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, LucideAngularModule, ButtonComponent],
  templateUrl: './add-card-modal.component.html',
  styleUrls: ['../../payment-methods/add-method-modal/add-method-modal.component.css', './add-card-modal.component.css']
})
export class AddCardModalComponent {
  cardForm: FormGroup;
  loading = false;

  issuers = ['Visa', 'Mastercard', 'Amex', 'Discover'];

  constructor(
    public dialogRef: DialogRef<any>,
    @Inject(DIALOG_DATA) public data: any,
    private fb: FormBuilder
  ) {
    this.cardForm = this.fb.group({
      cardNumber: ['', [Validators.required, Validators.pattern(/^\d{16}$/)]],
      cardHolderName: ['', [Validators.required, Validators.minLength(2)]],
      expiryMonth: ['', [Validators.required, Validators.min(1), Validators.max(12)]],
      expiryYear: ['', [Validators.required, Validators.min(2025), Validators.max(2040)]],
      issuer: ['', Validators.required],
      creditLimit: ['', [Validators.required, Validators.min(1000)]],
      billingCycleStartDay: [1, [Validators.required, Validators.min(1), Validators.max(28)]],
      nickname: ['']
    });
  }

  close() {
    this.dialogRef.close();
  }

  onSubmit() {
    if (this.cardForm.invalid) {
      this.cardForm.markAllAsTouched();
      return;
    }

    this.loading = true;
    setTimeout(() => {
      this.loading = false;
      this.dialogRef.close({
        success: true,
        payload: this.cardForm.value
      });
    }, 1500);
  }
}
