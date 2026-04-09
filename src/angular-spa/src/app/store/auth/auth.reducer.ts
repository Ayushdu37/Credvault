import { createReducer, on } from '@ngrx/store';
import { AuthActions } from './auth.actions';
import { AuthState, initialAuthState } from './auth.state';

export const authReducer = createReducer(
  initialAuthState,

  // --- Login ---
  on(AuthActions.login, (state) => ({
    ...state,
    loading: true,
    error: null,
  })),
  on(AuthActions.loginSuccess, (state, { auth }) => ({
    ...state,
    accessToken: auth.accessToken,
    refreshToken: auth.refreshToken,
    loading: false,
    error: null,
  })),
  on(AuthActions.loginFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error,
  })),

  // --- Register ---
  on(AuthActions.register, (state) => ({
    ...state,
    loading: true,
    error: null,
  })),
  on(AuthActions.registerSuccess, (state) => ({
    ...state,
    loading: false,
  })),
  on(AuthActions.registerFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error,
  })),

  // --- Profile ---
  on(AuthActions.loadProfileSuccess, (state, { user }) => ({
    ...state,
    user,
  })),
  on(AuthActions.loadProfileFailure, (state, { error }) => ({
    ...state,
    error,
  })),

  // --- Token Refresh ---
  on(AuthActions.refreshTokenSuccess, (state, { auth }) => ({
    ...state,
    accessToken: auth.accessToken,
    refreshToken: auth.refreshToken,
  })),

  // --- Logout / Token Expired ---
  on(AuthActions.logout, AuthActions.tokenExpired, () => initialAuthState),

  // --- OTP / Verify / Reset ---
  on(
    AuthActions.sendOTP,
    AuthActions.verifyOTP,
    AuthActions.verifyEmail,
    AuthActions.resetPassword,
    (state) => ({ ...state, loading: true, error: null })
  ),
  on(
    AuthActions.sendOTPSuccess,
    AuthActions.verifyOTPSuccess,
    AuthActions.verifyEmailSuccess,
    AuthActions.resetPasswordSuccess,
    (state) => ({ ...state, loading: false })
  ),
  on(
    AuthActions.sendOTPFailure,
    AuthActions.verifyOTPFailure,
    AuthActions.verifyEmailFailure,
    AuthActions.resetPasswordFailure,
    (state, { error }) => ({ ...state, loading: false, error })
  ),

  // --- Clear Error ---
  on(AuthActions.clearError, (state) => ({ ...state, error: null }))
);
