import { Component, inject, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { toSignal } from '@angular/core/rxjs-interop';
import { DatePipe, NgClass } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';
import { CardComponent } from '../../../shared/components/card/card.component';

import { SpinnerComponent } from '../../../shared/components/spinner/spinner.component';
import { EmptyStateComponent } from '../../../shared/components/empty-state/empty-state.component';
import { PaymentMethodsActions } from '../../../store/payment-methods/payment-methods.actions';
import {
  selectAllPaymentMethods,
  selectPaymentMethodsLoading,
} from '../../../store/payment-methods/payment-methods.selectors';
import { ModalService } from '../../../shared/components/modal/modal.service';
import { AddMethodModalComponent } from '../add-method-modal/add-method-modal.component';
import { ButtonComponent } from '../../../shared/components/button/button.component';

@Component({
  selector: 'app-payment-methods',
  standalone: true,
  imports: [
    DatePipe,
    NgClass,
    LucideAngularModule,
    CardComponent,
    ButtonComponent,

    SpinnerComponent,
    EmptyStateComponent,
  ],
  templateUrl: './payment-methods.component.html',
  styleUrl: './payment-methods.component.css',
})
export class PaymentMethodsComponent implements OnInit {
  private store = inject(Store);

  methods = toSignal(this.store.select(selectAllPaymentMethods), { initialValue: [] });
  loading = toSignal(this.store.select(selectPaymentMethodsLoading), { initialValue: false });

  ngOnInit(): void {
    this.store.dispatch(PaymentMethodsActions.loadMethods());
  }

  private modalService = inject(ModalService);

  openAddModal(): void {
    const dialogRef = this.modalService.openCustom(AddMethodModalComponent);

    dialogRef.closed.subscribe((result: any) => {
      if (result?.success) {
        // Here we would typically dispatch an action like:
        // this.store.dispatch(PaymentMethodsActions.addMethod({ payload: result.details }));
        // Since we are mocking parity for UI right now, we can just trigger a reload or show toast
        this.store.dispatch(PaymentMethodsActions.loadMethods());
      }
    });
  }

  deleteMethod(id: string): void {
    this.store.dispatch(PaymentMethodsActions.deleteMethod({ id }));
  }

  setDefault(id: string): void {
    this.store.dispatch(PaymentMethodsActions.setDefault({ id }));
  }

  getTypeLabel(type: string): string {
    const map: Record<string, string> = {
      bank_account: 'Bank Account',
      debit_card: 'Debit Card',
      upi: 'UPI',
    };
    return map[type] || type;
  }

  getTypeClass(type: string): string {
    const map: Record<string, string> = {
      bank_account: 'method-bank',
      debit_card: 'method-card',
      upi: 'method-upi',
    };
    return map[type] || 'method-bank';
  }
}
