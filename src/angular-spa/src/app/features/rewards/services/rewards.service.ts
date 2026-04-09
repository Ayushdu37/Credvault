import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { delay } from 'rxjs/operators';

export interface RewardTier {
  id: string;
  name: string;
  minPoints: number;
  cashbackPercent: number;
}

export interface RewardAccount {
  id: string;
  userId: string;
  tierId: string;
  tierName: string;
  availablePoints: number;
  totalEarned: number;
  cashbackPercent: number;
}

export interface RewardTransaction {
  id: string;
  rewardAccountId: string;
  paymentId: string | null;
  type: 'Earned' | 'Redeemed';
  points: number;
  description: string;
  createdAt: string;
}

@Injectable({ providedIn: 'root' })
export class RewardsService {

  private tiers: RewardTier[] = [
    { id: 'tier-001', name: 'Bronze',   minPoints: 0,     cashbackPercent: 0.5 },
    { id: 'tier-002', name: 'Silver',   minPoints: 5000,  cashbackPercent: 1.0 },
    { id: 'tier-003', name: 'Gold',     minPoints: 15000, cashbackPercent: 1.5 },
    { id: 'tier-004', name: 'Platinum', minPoints: 50000, cashbackPercent: 2.5 },
  ];

  private account: RewardAccount = {
    id: 'ra-001',
    userId: 'user-001',
    tierId: 'tier-003',
    tierName: 'Gold',
    availablePoints: 18750,
    totalEarned: 24300,
    cashbackPercent: 1.5,
  };

  private transactions: RewardTransaction[] = [
    {
      id: 'rt-001', rewardAccountId: 'ra-001', paymentId: 'pay-001',
      type: 'Earned', points: 250,
      description: 'Bill payment — Platinum Credit Card',
      createdAt: '2025-03-15T10:30:00Z',
    },
    {
      id: 'rt-002', rewardAccountId: 'ra-001', paymentId: null,
      type: 'Redeemed', points: -500,
      description: 'Statement credit — ₹500 applied',
      createdAt: '2025-03-12T14:00:00Z',
    },
    {
      id: 'rt-003', rewardAccountId: 'ra-001', paymentId: 'pay-002',
      type: 'Earned', points: 180,
      description: 'Bill payment — Gold Rewards Card',
      createdAt: '2025-03-10T09:00:00Z',
    },
    {
      id: 'rt-004', rewardAccountId: 'ra-001', paymentId: 'pay-003',
      type: 'Earned', points: 420,
      description: 'Auto-pay — Platinum Credit Card',
      createdAt: '2025-03-05T12:00:00Z',
    },
    {
      id: 'rt-005', rewardAccountId: 'ra-001', paymentId: null,
      type: 'Redeemed', points: -1000,
      description: 'Gift card — Amazon ₹1,000',
      createdAt: '2025-02-28T16:30:00Z',
    },
    {
      id: 'rt-006', rewardAccountId: 'ra-001', paymentId: 'pay-004',
      type: 'Earned', points: 310,
      description: 'Bill payment — Gold Rewards Card',
      createdAt: '2025-02-25T11:15:00Z',
    },
    {
      id: 'rt-007', rewardAccountId: 'ra-001', paymentId: null,
      type: 'Redeemed', points: -2000,
      description: 'Statement credit — ₹2,000 applied',
      createdAt: '2025-02-20T10:00:00Z',
    },
    {
      id: 'rt-008', rewardAccountId: 'ra-001', paymentId: 'pay-005',
      type: 'Earned', points: 550,
      description: 'Auto-pay — Platinum Credit Card',
      createdAt: '2025-02-15T08:30:00Z',
    },
  ];

  getRewardAccount(): Observable<RewardAccount> {
    return of(this.account).pipe(delay(400));
  }

  getTiers(): Observable<RewardTier[]> {
    return of(this.tiers).pipe(delay(300));
  }

  getTransactions(): Observable<RewardTransaction[]> {
    return of(this.transactions).pipe(delay(400));
  }

  redeemPoints(points: number): Observable<RewardAccount> {
    this.account = {
      ...this.account,
      availablePoints: this.account.availablePoints - points,
    };
    this.transactions.unshift({
      id: `rt-${Date.now()}`,
      rewardAccountId: this.account.id,
      paymentId: null,
      type: 'Redeemed',
      points: -points,
      description: `Statement credit — ₹${points.toLocaleString()} redeemed`,
      createdAt: new Date().toISOString(),
    });
    return of(this.account).pipe(delay(500));
  }
}
