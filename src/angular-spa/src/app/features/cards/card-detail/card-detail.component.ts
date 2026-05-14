import { Component, inject, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { ActivatedRoute, Router } from '@angular/router';
import { AsyncPipe, CurrencyPipe, DecimalPipe, NgClass, NgIf, PercentPipe, NgFor, DatePipe } from '@angular/common';
import { map, switchMap } from 'rxjs';
import { CardsActions } from '../../../store/cards/cards.actions';
import { selectCardById, selectCardsActionLoading, selectCardsLoading } from '../../../store/cards/cards.selectors';
import { CreditCard, CardNetwork } from '../../../core/models/card.model';
import { CardComponent } from '../../../shared/components/card/card.component';
import { ButtonComponent } from '../../../shared/components/button/button.component';
import { SpinnerComponent } from '../../../shared/components/spinner/spinner.component';
import { LucideAngularModule } from 'lucide-angular';
import { ModalService } from '../../../shared/components/modal/modal.service';
import { ManageLimitModalComponent, ManageLimitData } from '../manage-limit-modal/manage-limit-modal.component';
import { BillingService } from '../../billing/services/billing.service';

@Component({
  selector: 'app-card-detail',
  standalone: true,
  imports: [
    AsyncPipe, CurrencyPipe, DecimalPipe, NgIf, NgClass, PercentPipe, NgFor, DatePipe,
    CardComponent, ButtonComponent, SpinnerComponent, LucideAngularModule,
  ],
  templateUrl: './card-detail.component.html',
  styleUrls: ['./card-detail.component.css'],
})
export class CardDetailComponent implements OnInit {
  private store = inject(Store);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private modalService = inject(ModalService);
  private billingService = inject(BillingService);

  loading$ = this.store.select(selectCardsLoading);
  actionLoading$ = this.store.select(selectCardsActionLoading);

  card$ = this.route.paramMap.pipe(
    map(params => params.get('id') || ''),
    switchMap(id => this.store.select(selectCardById(id)))
  );

  /** Billing history for this card — populated from store or API */
  cardBills: { id: string; date: string; dueDate: string; minimumDue: number; amount: number; status: string }[] = [];

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id') || '';
    this.store.dispatch(CardsActions.selectCard({ id }));
    if (id) {
      this.store.dispatch(CardsActions.loadCardById({ id }));
      this.billingService.getBillsByCard(id).subscribe({
        next: (bills) => {
          this.cardBills = bills.map(b => ({
            id: b.id,
            date: b.createdAt || b.billingMonth + '-01',
            dueDate: b.dueDate,
            minimumDue: b.minimumDue,
            amount: b.totalAmount,
            status: b.status.toString() === 'Paid' || b.status === 0 ? 'Paid' : (new Date(b.dueDate) < new Date() ? 'Overdue' : 'Due')
          }));
        },
        error: (err) => console.error('Failed to load bills', err)
      });
    }
  }

  goBack(): void {
    this.router.navigate(['/cards']);
  }

  getUtilization(card: CreditCard): number {
    return card.creditLimit > 0 ? card.currentBalance / card.creditLimit : 0;
  }

  toggleLock(card: CreditCard): void {
    // TODO: Add lock/unlock action to CardsActions and wire to backend
    // For now, show a confirmation dialog as a placeholder
    const action = card.status === 'Locked' ? 'unlock' : 'lock';
    const dialogRef = this.modalService.openConfirm({
      title: `${action === 'lock' ? 'Lock' : 'Unlock'} Card`,
      content: `Are you sure you want to ${action} your card ending in ${card.last4Digits}?`,
      confirmText: action === 'lock' ? 'Lock Card' : 'Unlock Card',
      cancelText: 'Cancel',
      danger: action === 'lock',
    });
    dialogRef.subscribe(confirmed => {
      if (confirmed) {
        // TODO: dispatch CardsActions.toggleLock({ id: card.id })
        console.log(`Card ${action} requested for ${card.id}`);
      }
    });
  }

  manageLimit(card: CreditCard): void {
    const dialogRef = this.modalService.openCustom<ManageLimitModalComponent, ManageLimitData>(
      ManageLimitModalComponent,
      {
        cardId: card.id,
        cardName: card.cardholderName,
        cardLast4: card.last4Digits,
        currentLimit: card.creditLimit
      }
    );

    dialogRef.closed.subscribe((result: any) => {
      if (result?.success) {
        this.store.dispatch(CardsActions.updateCardLimit({
          id: card.id,
          newLimit: result.newLimit,
        }));
      }
    });
  }

  setDefault(card: CreditCard): void {
    this.store.dispatch(CardsActions.setDefaultCard({ id: card.id }));
  }

  verifyCard(card: CreditCard): void {
    const dialogRef = this.modalService.openConfirm({
      title: 'Verify Card',
      content: `Are you sure you want to verify your card ending in ${card.last4Digits}? This will enable full transaction capabilities.`,
      confirmText: 'Verify Now',
      cancelText: 'Cancel',
      danger: false
    });
    dialogRef.subscribe(confirmed => {
      if (confirmed) {
        this.store.dispatch(CardsActions.verifyCard({ id: card.id }));
      }
    });
  }

  deleteCard(card: CreditCard): void {
    const dialogRef = this.modalService.openConfirm({
      title: 'Remove Card',
      content: `Are you sure you want to permanently remove your ${card.network} card ending in ${card.last4Digits}? This action cannot be undone.`,
      confirmText: 'Remove Card',
      cancelText: 'Keep Card',
      danger: true
    });
    dialogRef.subscribe(confirmed => {
      if (confirmed) {
        this.store.dispatch(CardsActions.deleteCard({ id: card.id }));
        this.router.navigate(['/cards']);
      }
    });
  }

  viewBill(billId: string): void {
    this.router.navigate(['/billing', billId]);
  }

  payNow(card: CreditCard): void {
    this.router.navigate(['/payments/pay'], { 
      queryParams: { 
        category: 'credit', 
        cardId: card.id 
      } 
    });
  }
}
