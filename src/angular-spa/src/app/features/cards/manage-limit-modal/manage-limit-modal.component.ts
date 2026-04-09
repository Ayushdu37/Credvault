import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { DialogRef, DIALOG_DATA } from '@angular/cdk/dialog';
import { LucideAngularModule } from 'lucide-angular';
import { ButtonComponent } from '../../../shared/components/button/button.component';

export interface ManageLimitData {
  cardId: string;
  cardName: string;
  cardLast4: string;
  currentLimit: number;
}

@Component({
  selector: 'app-manage-limit-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, LucideAngularModule, ButtonComponent],
  templateUrl: './manage-limit-modal.component.html',
  styleUrls: ['../../payment-methods/add-method-modal/add-method-modal.component.css', './manage-limit-modal.component.css']
})
export class ManageLimitModalComponent implements OnInit {
  limitForm: FormGroup;
  loading = false;

  constructor(
    public dialogRef: DialogRef<any>,
    @Inject(DIALOG_DATA) public data: ManageLimitData,
    private fb: FormBuilder
  ) {
    this.limitForm = this.fb.group({
      newLimit: ['', [Validators.required, Validators.min(500), Validators.max(500000)]]
    });
  }

  ngOnInit() {
    this.limitForm.patchValue({ newLimit: this.data.currentLimit });
  }

  get newLimitControl() { return this.limitForm.get('newLimit'); }

  close() {
    this.dialogRef.close();
  }

  onSubmit() {
    if (this.limitForm.invalid) {
      this.limitForm.markAllAsTouched();
      return;
    }

    this.loading = true;
    
    // TODO: Wire API call [HttpPut("{id:guid}/limit")]
    setTimeout(() => {
      this.loading = false;
      this.dialogRef.close({
        success: true,
        newLimit: this.limitForm.value.newLimit
      });
    }, 1200);
  }
}
