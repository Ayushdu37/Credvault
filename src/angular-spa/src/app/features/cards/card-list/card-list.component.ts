import { Component, inject, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { AsyncPipe, CurrencyPipe, DecimalPipe, NgClass, NgFor, NgIf, PercentPipe } from '@angular/common';
import { Router } from '@angular/router';
import { CardsActions } from '../../../store/cards/cards.actions';
import { selectAllCards, selectCardsActionLoading, selectCardsLoading } from '../../../store/cards/cards.selectors';
import { CreditCard, CardNetwork } from '../../../core/models/card.model';
import { CardComponent } from '../../../shared/components/card/card.component';
import { ButtonComponent } from '../../../shared/components/button/button.component';
import { SpinnerComponent } from '../../../shared/components/spinner/spinner.component';
import { EmptyStateComponent } from '../../../shared/components/empty-state/empty-state.component';
import { LucideAngularModule } from 'lucide-angular';
import { ModalService } from '../../../shared/components/modal/modal.service';
import { AddCardModalComponent } from '../add-card-modal/add-card-modal.component';

@Component({
  selector: 'app-card-list',
  standalone: true,
  imports: [
    AsyncPipe, CurrencyPipe, DecimalPipe, NgFor, NgIf, NgClass, PercentPipe,
    CardComponent, ButtonComponent, SpinnerComponent, EmptyStateComponent,
    LucideAngularModule,
  ],
  templateUrl: './card-list.component.html',
  styleUrls: ['./card-list.component.css'],
})
export class CardListComponent implements OnInit {
  private store = inject(Store);
  private router = inject(Router);
  private modalService = inject(ModalService);

  cards$ = this.store.select(selectAllCards);
  loading$ = this.store.select(selectCardsLoading);
  actionLoading$ = this.store.select(selectCardsActionLoading);

  ngOnInit(): void {
    this.store.dispatch(CardsActions.loadCards({ page: 1, pageSize: 10 }));
  }

  viewCard(card: CreditCard): void {
    this.store.dispatch(CardsActions.selectCard({ id: card.id }));
    this.router.navigate(['/cards', card.id]);
  }

  requestNewCard(): void {
    const dialogRef = this.modalService.openCustom<AddCardModalComponent, any>(
      AddCardModalComponent,
      {}
    );

    dialogRef.closed.subscribe((result: any) => {
      if (result?.success) {
        this.store.dispatch(CardsActions.addCard({ payload: result.payload }));
      }
    });
  }

  getUtilization(card: CreditCard): number {
    return card.creditLimit > 0 ? card.currentBalance / card.creditLimit : 0;
  }

  getNetworkColor(network: CardNetwork): string {
    const colors: Record<CardNetwork, string> = {
      Visa: '#1A1F71',
      Mastercard: '#EB001B',
      Amex: '#007BC1',
      Discover: '#FF6000',
    };
    return colors[network] || '#555';
  }

  toggleLock(card: CreditCard): void {
    // TODO: Add lock/unlock action to CardsActions and wire to backend
    const action = card.status === 'Locked' ? 'unlock' : 'lock';
    console.log(`Card ${action} requested for ${card.id}`);
  }
}
