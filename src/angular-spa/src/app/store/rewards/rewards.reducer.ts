import { createReducer, on } from '@ngrx/store';
import { RewardAccount, RewardTransaction } from '../../core/models/billing.model';
import { RewardsActions } from './rewards.actions';

export interface RewardsState {
  account: RewardAccount | null;
  transactions: RewardTransaction[];
  loading: boolean;
  redeeming: boolean;
  error: string | null;
  page: number;
  pageSize: number;
  totalCount: number;
}

export const initialState: RewardsState = {
  account: null,
  transactions: [],
  loading: false,
  redeeming: false,
  error: null,
  page: 1,
  pageSize: 10,
  totalCount: 0,
};

export const rewardsReducer = createReducer(
  initialState,
  on(RewardsActions.loadAccount, (state) => ({ ...state, loading: true, error: null })),
  on(RewardsActions.loadAccountSuccess, (state, { account }) => ({ ...state, account, loading: false })),
  on(RewardsActions.loadAccountFailure, (state, { error }) => ({ ...state, loading: false, error })),

  on(RewardsActions.loadTransactions, (state, { page, pageSize }) =>
    ({ ...state, page, pageSize })
  ),
  on(RewardsActions.loadTransactionsSuccess, (state, { transactions, totalCount }) =>
    ({ ...state, transactions, totalCount })
  ),
  on(RewardsActions.loadTransactionsFailure, (state, { error }) => ({ ...state, error })),

  on(RewardsActions.redeemPoints, (state) => ({ ...state, redeeming: true })),
  on(RewardsActions.redeemPointsSuccess, (state, { account }) => ({ ...state, account, redeeming: false })),
  on(RewardsActions.redeemPointsFailure, (state, { error }) => ({ ...state, redeeming: false, error })),
);
