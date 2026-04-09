import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogRef, DIALOG_DATA } from '@angular/cdk/dialog';
import { LucideAngularModule } from 'lucide-angular';
import { ButtonComponent } from '../../../shared/components/button/button.component';

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

  constructor(
    public dialogRef: DialogRef<any>,
    @Inject(DIALOG_DATA) public data: any,
    private fb: FormBuilder
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

  close() {
    this.dialogRef.close();
  }

  onSubmit() {
    if (this.methodForm.invalid) {
      this.methodForm.markAllAsTouched();
      return;
    }

    this.loading = true;
    
    // TODO: Wire API call [HttpPost] PaymentMethodsController
    setTimeout(() => {
      this.loading = false;
      this.dialogRef.close({
        success: true,
        type: this.methodType,
        details: this.methodForm.value
      });
    }, 1500);
  }
}
