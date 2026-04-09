import { Component, inject, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { ActivatedRoute, Router } from '@angular/router';
import { AsyncPipe, CurrencyPipe, DecimalPipe, NgClass, NgIf, PercentPipe, NgFor, DatePipe } from '@angular/common';
import { map, switchMap } from 'rxjs';
import { CardsActions } from '../../../store/cards/cards.actions';
import { selectCardById, selectCardsActionLoading, selectCardsLoading } from '../../../store/cards/cards.selectors';
import { CardComponent } from '../../../shared/components/card/card.component';
import { ButtonComponent } from '../../../shared/components/button/button.component';
import { SpinnerComponent } from '../../../shared/components/spinner/spinner.component';
import { LucideAngularModule } from 'lucide-angular';
import { CreditCard } from '../services/cards.service';
import { ModalService } from '../../../shared/components/modal/modal.service';
import { ManageLimitModalComponent, ManageLimitData } from '../manage-limit-modal/manage-limit-modal.component';

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

  loading$ = this.store.select(selectCardsLoading);
  actionLoading$ = this.store.select(selectCardsActionLoading);

  card$ = this.route.paramMap.pipe(
    map(params => params.get('id') || ''),
    switchMap(id => this.store.select(selectCardById(id)))
  );

  ngOnInit(): void {
    this.store.dispatch(CardsActions.loadCards());
  }

  goBack(): void {
    this.router.navigate(['/cards']);
  }

  toggleLock(card: CreditCard): void {
    if (card.status === 'Locked') {
      this.store.dispatch(CardsActions.unlockCard({ id: card.id }));
    } else {
      this.store.dispatch(CardsActions.lockCard({ id: card.id }));
    }
  }

  getUtilization(card: CreditCard): number {
    return card.creditLimit > 0 ? card.currentBalance / card.creditLimit : 0;
  }

  // Mock billing history for this card
  cardBills = [
    { id: 'stmt-001', date: '2026-03-01', amount: 3250.75, minimumDue: 150.00, dueDate: '2026-03-20', status: 'Paid' },
    { id: 'stmt-002', date: '2026-02-01', amount: 2840.50, minimumDue: 125.00, dueDate: '2026-02-20', status: 'Paid' },
    { id: 'stmt-003', date: '2026-01-01', amount: 4120.00, minimumDue: 206.00, dueDate: '2026-01-20', status: 'Paid' },
  ];

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
        this.store.dispatch(CardsActions.loadCards());
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
}
