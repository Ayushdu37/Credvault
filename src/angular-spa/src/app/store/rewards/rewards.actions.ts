import { createActionGroup, emptyProps, props } from '@ngrx/store';
import { RewardAccount, RewardTransaction, RedeemRewardsRequest } from '../../core/models/billing.model';

export const RewardsActions = createActionGroup({
  source: 'Rewards',
  events: {
    'Load Account': emptyProps(),
    'Load Account Success': props<{ account: RewardAccount }>(),
    'Load Account Failure': props<{ error: string }>(),

    'Load Transactions': props<{ page: number; pageSize: number }>(),
    'Load Transactions Success': props<{ transactions: RewardTransaction[]; totalCount: number }>(),
    'Load Transactions Failure': props<{ error: string }>(),

    'Redeem Points': props<{ payload: RedeemRewardsRequest }>(),
    'Redeem Points Success': props<{ account: RewardAccount }>(),
    'Redeem Points Failure': props<{ error: string }>(),
  },
});
