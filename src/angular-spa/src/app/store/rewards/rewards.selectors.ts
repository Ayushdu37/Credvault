import { createFeatureSelector, createSelector } from '@ngrx/store';
import { RewardsState } from './rewards.reducer';

export const selectRewardsState = createFeatureSelector<RewardsState>('rewards');

export const selectRewardAccount = createSelector(selectRewardsState, (s) => s.account);
export const selectRewardTiers = createSelector(selectRewardsState, (s) => s.tiers);
export const selectRewardTransactions = createSelector(selectRewardsState, (s) => s.transactions);
export const selectRewardsLoading = createSelector(selectRewardsState, (s) => s.loading);
export const selectRewardsRedeeming = createSelector(selectRewardsState, (s) => s.redeeming);
export const selectRewardsError = createSelector(selectRewardsState, (s) => s.error);
