import { createActionGroup, emptyProps, props } from '@ngrx/store';
import { DashboardSummary } from '../../core/models/dashboard.model';

export const DashboardActions = createActionGroup({
  source: 'Dashboard',
  events: {
    'Load Summary': emptyProps(),
    'Load Summary Success': props<{ summary: DashboardSummary }>(),
    'Load Summary Failure': props<{ error: string }>(),
  }
});
