import { createReducer, on } from '@ngrx/store';
import { RewardAccount, RewardTier, RewardTransaction } from '../../features/rewards/services/rewards.service';
import { RewardsActions } from './rewards.actions';

export interface RewardsState {
  account: RewardAccount | null;
  tiers: RewardTier[];
  transactions: RewardTransaction[];
  loading: boolean;
  redeeming: boolean;
  error: string | null;
}

export const initialState: RewardsState = {
  account: null,
  tiers: [],
  transactions: [],
  loading: false,
  redeeming: false,
  error: null,
};

export const rewardsReducer = createReducer(
  initialState,
  on(RewardsActions.loadAccount, (state) => ({ ...state, loading: true, error: null })),
  on(RewardsActions.loadAccountSuccess, (state, { account }) => ({ ...state, account, loading: false })),
  on(RewardsActions.loadAccountFailure, (state, { error }) => ({ ...state, loading: false, error })),

  on(RewardsActions.loadTiersSuccess, (state, { tiers }) => ({ ...state, tiers })),

  on(RewardsActions.loadTransactionsSuccess, (state, { transactions }) => ({ ...state, transactions })),
  on(RewardsActions.loadTransactionsFailure, (state, { error }) => ({ ...state, error })),

  on(RewardsActions.redeemPoints, (state) => ({ ...state, redeeming: true })),
  on(RewardsActions.redeemPointsSuccess, (state, { account }) => ({ ...state, account, redeeming: false })),
  on(RewardsActions.redeemPointsFailure, (state, { error }) => ({ ...state, redeeming: false, error })),
);
