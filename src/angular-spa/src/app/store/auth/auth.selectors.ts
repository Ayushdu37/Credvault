import { createFeatureSelector, createSelector } from '@ngrx/store';
import { AuthState } from './auth.state';

export const selectAuthState = createFeatureSelector<AuthState>('auth');

export const selectUser = createSelector(selectAuthState, (s) => s.user);

export const selectAccessToken = createSelector(
  selectAuthState,
  (s) => s.accessToken
);

export const selectRefreshToken = createSelector(
  selectAuthState,
  (s) => s.refreshToken
);

export const selectIsLoggedIn = createSelector(
  selectAuthState,
  (s) => !!s.accessToken && !!s.user
);

export const selectAuthLoading = createSelector(
  selectAuthState,
  (s) => s.loading
);

export const selectAuthError = createSelector(
  selectAuthState,
  (s) => s.error
);

export const selectUserRole = createSelector(
  selectAuthState,
  (s) => s.user?.role ?? null
);

export const selectEmailVerified = createSelector(
  selectAuthState,
  (s) => s.user?.isEmailVerified ?? false
);
