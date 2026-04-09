import { createReducer, on } from '@ngrx/store';
import { DashboardActions } from './dashboard.actions';
import { DashboardSummary } from '../../core/models/dashboard.model';

export interface DashboardState {
  summary: DashboardSummary | null;
  loading: boolean;
  error: string | null;
}

export const initialState: DashboardState = {
  summary: null,
  loading: false,
  error: null,
};

export const dashboardReducer = createReducer(
  initialState,
  on(DashboardActions.loadSummary, (state) => ({
    ...state,
    loading: true,
    error: null,
  })),
  on(DashboardActions.loadSummarySuccess, (state, { summary }) => ({
    ...state,
    loading: false,
    summary,
  })),
  on(DashboardActions.loadSummaryFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error,
  }))
);
