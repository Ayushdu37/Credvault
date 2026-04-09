import { createActionGroup, emptyProps, props } from '@ngrx/store';
import { RewardAccount, RewardTier, RewardTransaction } from '../../features/rewards/services/rewards.service';

export const RewardsActions = createActionGroup({
  source: 'Rewards',
  events: {
    'Load Account': emptyProps(),
    'Load Account Success': props<{ account: RewardAccount }>(),
    'Load Account Failure': props<{ error: string }>(),

    'Load Tiers': emptyProps(),
    'Load Tiers Success': props<{ tiers: RewardTier[] }>(),

    'Load Transactions': emptyProps(),
    'Load Transactions Success': props<{ transactions: RewardTransaction[] }>(),
    'Load Transactions Failure': props<{ error: string }>(),

    'Redeem Points': props<{ points: number }>(),
    'Redeem Points Success': props<{ account: RewardAccount }>(),
    'Redeem Points Failure': props<{ error: string }>(),
  },
});
