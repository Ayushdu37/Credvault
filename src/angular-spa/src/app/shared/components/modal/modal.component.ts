import { Component, Inject } from '@angular/core';
import { DIALOG_DATA, DialogRef } from '@angular/cdk/dialog';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';
import { ButtonComponent } from '../button/button.component';

export interface ModalData {
  title: string;
  content: string;
  confirmText?: string;
  cancelText?: string;
  danger?: boolean;
}

@Component({
  selector: 'app-modal',
  standalone: true,
  imports: [CommonModule, LucideAngularModule, ButtonComponent],
  template: `
    <div class="modal-surface">
      <div class="modal-header">
        <h2 class="modal-title">{{ data.title }}</h2>
        <button class="modal-close" (click)="close(false)">
          <lucide-icon name="x" [size]="18"></lucide-icon>
        </button>
      </div>

      <div class="modal-body">
        <p>{{ data.content }}</p>
      </div>

      <div class="modal-footer">
        <app-button variant="secondary" (click)="close(false)">
          {{ data.cancelText || 'Cancel' }}
        </app-button>
        <app-button 
          [variant]="data.danger ? 'danger' : 'primary'"
          (click)="close(true)">
          {{ data.confirmText || 'Confirm' }}
        </app-button>
      </div>
    </div>
  `,
  styleUrls: ['./modal.component.css']
})
export class ModalComponent {
  constructor(
    public dialogRef: DialogRef<boolean>,
    @Inject(DIALOG_DATA) public data: ModalData
  ) {}

  close(result: boolean) {
    this.dialogRef.close(result);
  }
}
