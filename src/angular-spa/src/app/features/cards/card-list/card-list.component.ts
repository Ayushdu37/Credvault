import { Component, inject, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { AsyncPipe, CurrencyPipe, DecimalPipe, NgClass, NgFor, NgIf, PercentPipe } from '@angular/common';
import { Router } from '@angular/router';
import { CardsActions } from '../../../store/cards/cards.actions';
import { selectAllCards, selectCardsActionLoading, selectCardsLoading } from '../../../store/cards/cards.selectors';
import { CreditCard } from '../services/cards.service';
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
    this.store.dispatch(CardsActions.loadCards());
  }

  viewCard(card: CreditCard): void {
    this.router.navigate(['/cards', card.id]);
  }

  toggleLock(card: CreditCard): void {
    if (card.status === 'Locked') {
      this.store.dispatch(CardsActions.unlockCard({ id: card.id }));
    } else {
      this.store.dispatch(CardsActions.lockCard({ id: card.id }));
    }
  }

  requestNewCard(): void {
    const dialogRef = this.modalService.openCustom<AddCardModalComponent, any>(
      AddCardModalComponent,
      {}
    );

    dialogRef.closed.subscribe((result: any) => {
      if (result?.success) {
        this.store.dispatch(CardsActions.addCard({ payload: result.payload }));
        this.store.dispatch(CardsActions.loadCards());
      }
    });
  }

  getUtilization(card: CreditCard): number {
    return card.creditLimit > 0 ? card.currentBalance / card.creditLimit : 0;
  }

  getNetworkColor(network: string): string {
    const colors: Record<string, string> = {
      Visa: '#1A1F71',
      Mastercard: '#EB001B',
      Amex: '#007BC1',
      Discover: '#FF6000',
    };
    return colors[network] || '#555';
  }
}
