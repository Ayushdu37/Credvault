import { Component, inject, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { toSignal } from '@angular/core/rxjs-interop';
import { DatePipe, DecimalPipe, NgClass } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';
import { CardComponent } from '../../../shared/components/card/card.component';
import { SpinnerComponent } from '../../../shared/components/spinner/spinner.component';
import { RewardsActions } from '../../../store/rewards/rewards.actions';
import {
  selectRewardAccount,
  selectRewardTiers,
  selectRewardTransactions,
  selectRewardsLoading,
} from '../../../store/rewards/rewards.selectors';
import { ButtonComponent } from '../../../shared/components/button/button.component';
import { ModalService } from '../../../shared/components/modal/modal.service';
import { RedeemPointsModalComponent, RedeemPointsData } from '../redeem-points-modal/redeem-points-modal.component';

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
  tiers = toSignal(this.store.select(selectRewardTiers), { initialValue: [] });
  transactions = toSignal(this.store.select(selectRewardTransactions), { initialValue: [] });
  loading = toSignal(this.store.select(selectRewardsLoading), { initialValue: false });

  ngOnInit(): void {
    this.store.dispatch(RewardsActions.loadAccount());
    this.store.dispatch(RewardsActions.loadTiers());
    this.store.dispatch(RewardsActions.loadTransactions());
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
        // Mock backend parity syncing:
        this.store.dispatch(RewardsActions.loadAccount());
      }
    });
  }

  getTierProgress(): number {
    const acct = this.account();
    const allTiers = this.tiers();
    if (!acct || allTiers.length === 0) return 0;

    const currentTierIdx = allTiers.findIndex(t => t.name === acct.tierName);
    const nextTier = allTiers[currentTierIdx + 1];
    if (!nextTier) return 100; // Already at max tier

    const currentMin = allTiers[currentTierIdx]?.minPoints || 0;
    const range = nextTier.minPoints - currentMin;
    const progress = acct.totalEarned - currentMin;
    return Math.min(100, Math.round((progress / range) * 100));
  }

  getNextTier(): string {
    const acct = this.account();
    const allTiers = this.tiers();
    if (!acct || allTiers.length === 0) return '';

    const currentTierIdx = allTiers.findIndex(t => t.name === acct.tierName);
    const nextTier = allTiers[currentTierIdx + 1];
    return nextTier ? nextTier.name : 'Max Tier';
  }

  getPointsToNextTier(): number {
    const acct = this.account();
    const allTiers = this.tiers();
    if (!acct || allTiers.length === 0) return 0;

    const currentTierIdx = allTiers.findIndex(t => t.name === acct.tierName);
    const nextTier = allTiers[currentTierIdx + 1];
    if (!nextTier) return 0;
    return Math.max(0, nextTier.minPoints - acct.totalEarned);
  }

  getTierColor(tierName: string): string {
    const colors: Record<string, string> = {
      Bronze: '#cd7f32',
      Silver: '#c0c0c0',
      Gold: '#eab308',
      Platinum: '#a78bfa',
    };
    return colors[tierName] || '#94a3b8';
  }
}
