import { Component, inject, OnInit, signal } from '@angular/core';
import { Store } from '@ngrx/store';
import { toSignal } from '@angular/core/rxjs-interop';
import { DatePipe, DecimalPipe, NgClass } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';
import { CardComponent } from '../../../shared/components/card/card.component';
import { SpinnerComponent } from '../../../shared/components/spinner/spinner.component';
import { RewardsActions } from '../../../store/rewards/rewards.actions';
import {
  selectRewardAccount,
  selectRewardTransactions,
  selectRewardsLoading,
} from '../../../store/rewards/rewards.selectors';
import { ButtonComponent } from '../../../shared/components/button/button.component';
import { ModalService } from '../../../shared/components/modal/modal.service';
import { RedeemPointsModalComponent, RedeemPointsData } from '../redeem-points-modal/redeem-points-modal.component';

interface TierInfo {
  id: string;
  name: string;
  minPoints: number;
  cashbackPercent: number;
}

@Component({
  selector: 'app-rewards',
  standalone: true,
  imports: [
    DatePipe,
    DecimalPipe,
    NgClass,
    LucideAngularModule,
    CardComponent,
    SpinnerComponent,
    ButtonComponent,
  ],
  templateUrl: './rewards.component.html',
  styleUrl: './rewards.component.css',
})
export class RewardsComponent implements OnInit {
  private store = inject(Store);

  account = toSignal(this.store.select(selectRewardAccount));
  transactions = toSignal(this.store.select(selectRewardTransactions), { initialValue: [] });
  loading = toSignal(this.store.select(selectRewardsLoading), { initialValue: false });

  /** Tier definitions used in the template tier markers */
  tiers = signal<TierInfo[]>([
    { id: 'silver', name: 'Silver', minPoints: 0, cashbackPercent: 1 },
    { id: 'gold', name: 'Gold', minPoints: 1000, cashbackPercent: 2 },
    { id: 'platinum', name: 'Platinum', minPoints: 5000, cashbackPercent: 5 },
  ]);

  ngOnInit(): void {
    this.store.dispatch(RewardsActions.loadAccount());
    this.store.dispatch(RewardsActions.loadTransactions({ page: 1, pageSize: 10 }));
  }

  private modalService = inject(ModalService);

  openRedeemModal(): void {
    const acct = this.account();
    if (!acct) return;

    const dialogRef = this.modalService.openCustom<RedeemPointsModalComponent, RedeemPointsData>(
      RedeemPointsModalComponent,
      { availablePoints: acct.availablePoints }
    );

    dialogRef.closed.subscribe((result: any) => {
      if (result?.success) {
        this.store.dispatch(RewardsActions.loadAccount());
      }
    });
  }

  getTierProgress(): number {
    const acct = this.account();
    if (!acct) return 0;

    const tierList = this.tiers();
    const currentTierIdx = tierList.findIndex(t => t.name === acct.tierName);
    const nextTier = tierList[currentTierIdx + 1];
    if (!nextTier) return 100;

    const currentMin = tierList[currentTierIdx]?.minPoints || 0;
    const range = nextTier.minPoints - currentMin;
    const progress = acct.totalEarned - currentMin;
    return Math.min(100, Math.round((progress / range) * 100));
  }

  getNextTier(): string {
    const acct = this.account();
    if (!acct) return '';

    const tierNames = this.tiers().map(t => t.name);
    const currentTierIdx = tierNames.findIndex(t => t === acct.tierName);
    const nextTier = tierNames[currentTierIdx + 1];
    return nextTier || 'Max Tier';
  }

  getPointsToNextTier(): number {
    const acct = this.account();
    if (!acct) return 0;

    const tierList = this.tiers();
    const tierNames = tierList.map(t => t.name);
    const currentTierIdx = tierNames.findIndex(t => t === acct.tierName);
    const nextTier = tierList[currentTierIdx + 1];
    if (!nextTier) return 0;
    return Math.max(0, nextTier.minPoints - acct.totalEarned);
  }

  getTierColor(tierName: string): string {
    const colors: Record<string, string> = {
      Silver: '#c0c0c0',
      Gold: '#eab308',
      Platinum: '#a78bfa',
    };
    return colors[tierName] || '#94a3b8';
  }
}
