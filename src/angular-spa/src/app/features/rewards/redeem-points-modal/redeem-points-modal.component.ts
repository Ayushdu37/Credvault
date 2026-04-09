import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule, DecimalPipe } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogRef, DIALOG_DATA } from '@angular/cdk/dialog';
import { LucideAngularModule } from 'lucide-angular';
import { ButtonComponent } from '../../../shared/components/button/button.component';

export interface RedeemPointsData {
  availablePoints: number;
}

@Component({
  selector: 'app-redeem-points-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, LucideAngularModule, ButtonComponent, DecimalPipe],
  templateUrl: './redeem-points-modal.component.html',
  styleUrls: ['../../payment-methods/add-method-modal/add-method-modal.component.css', './redeem-points-modal.component.css']
})
export class RedeemPointsModalComponent implements OnInit {
  redeemForm: FormGroup;
  loading = false;
  redemptionMethod: 'Cashback' | 'GiftCard' = 'Cashback';

  constructor(
    public dialogRef: DialogRef<any>,
    @Inject(DIALOG_DATA) public data: RedeemPointsData,
    private fb: FormBuilder
  ) {
    this.redeemForm = this.fb.group({
      points: ['', [Validators.required, Validators.min(100), Validators.max(this.data.availablePoints)]]
    });
  }

  ngOnInit() {}

  get pointsControl() { return this.redeemForm.get('points'); }

  setMethod(method: 'Cashback' | 'GiftCard') {
    this.redemptionMethod = method;
  }

  get convertedValue(): number {
    const points = this.pointsControl?.value || 0;
    // Mock conversion rate: 100 points = $1.00
    return this.pointsControl?.valid ? points / 100 : 0;
  }

  close() {
    this.dialogRef.close();
  }

  onSubmit() {
    if (this.redeemForm.invalid) {
      this.redeemForm.markAllAsTouched();
      return;
    }

    this.loading = true;
    
    // Simulate API call to redeem points
    setTimeout(() => {
      this.loading = false;
      this.dialogRef.close({
        success: true,
        pointsRedeemed: this.redeemForm.value.points,
        method: this.redemptionMethod
      });
    }, 1500);
  }
}
