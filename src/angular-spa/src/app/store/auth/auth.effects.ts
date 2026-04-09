import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { Store } from '@ngrx/store';
import { of } from 'rxjs';
import { catchError, exhaustMap, map, switchMap, tap } from 'rxjs/operators';
import { AuthService } from '../../core/services/auth.service';
import { TokenService } from '../../core/services/token.service';
import { ToastService } from '../../core/services/toast.service';
import { AuthActions } from './auth.actions';
import { selectRefreshToken } from './auth.selectors';

// ─── Login ───────────────────────────────────────────────
export const login$ = createEffect(
  (
    actions$ = inject(Actions),
    authService = inject(AuthService),
    tokenService = inject(TokenService)
  ) =>
    actions$.pipe(
      ofType(AuthActions.login),
      exhaustMap(({ email, password }) =>
        authService.login({ email, password }).pipe(
          map((res) => {
            if (!res.success || !res.data) {
              return AuthActions.loginFailure({
                error: res.message || 'Login failed',
              });
            }
            // Store tokens in memory
            tokenService.setTokens(
              res.data.accessToken,
              res.data.refreshToken,
              res.data.expiresAt
            );
            return AuthActions.loginSuccess({ auth: res.data });
          }),
          catchError((err) =>
            of(
              AuthActions.loginFailure({
                error: err.error?.message || 'Network error',
              })
            )
          )
        )
      )
    ),
  { functional: true }
);

// ─── After Login Success → Load Profile ──────────────────
export const loadProfileAfterLogin$ = createEffect(
  (actions$ = inject(Actions)) =>
    actions$.pipe(
      ofType(AuthActions.loginSuccess),
      map(() => AuthActions.loadProfile())
    ),
  { functional: true }
);

// ─── Load Profile ────────────────────────────────────────
export const loadProfile$ = createEffect(
  (actions$ = inject(Actions), authService = inject(AuthService)) =>
    actions$.pipe(
      ofType(AuthActions.loadProfile),
      switchMap(() =>
        authService.getProfile().pipe(
          map((res) => {
            if (!res.success || !res.data) {
              return AuthActions.loadProfileFailure({
                error: res.message || 'Failed to load profile',
              });
            }
            return AuthActions.loadProfileSuccess({ user: res.data });
          }),
          catchError((err) =>
            of(
              AuthActions.loadProfileFailure({
                error: err.error?.message || 'Network error',
              })
            )
          )
        )
      )
    ),
  { functional: true }
);

// ─── After Profile Loaded → Navigate to Dashboard ───────
export const redirectAfterProfile$ = createEffect(
  (actions$ = inject(Actions), router = inject(Router)) =>
    actions$.pipe(
      ofType(AuthActions.loadProfileSuccess),
      tap(() => router.navigateByUrl('/dashboard'))
    ),
  { functional: true, dispatch: false }
);

// ─── Register ────────────────────────────────────────────
export const register$ = createEffect(
  (
    actions$ = inject(Actions),
    authService = inject(AuthService),
    toast = inject(ToastService)
  ) =>
    actions$.pipe(
      ofType(AuthActions.register),
      exhaustMap(({ email, password, fullName, phoneNumber }) =>
        authService
          .register({ email, password, fullName, phoneNumber })
          .pipe(
            map((res) => {
              if (!res.success) {
                return AuthActions.registerFailure({
                  error: res.message || 'Registration failed',
                });
              }
              toast.show(
                'Registration successful! Check your email to verify.',
                'success'
              );
              return AuthActions.registerSuccess();
            }),
            catchError((err) =>
              of(
                AuthActions.registerFailure({
                  error: err.error?.message || 'Network error',
                })
              )
            )
          )
      )
    ),
  { functional: true }
);

// ─── After Register Success → Redirect to Login ─────────
export const redirectAfterRegister$ = createEffect(
  (actions$ = inject(Actions), router = inject(Router)) =>
    actions$.pipe(
      ofType(AuthActions.registerSuccess),
      tap(() => router.navigateByUrl('/login'))
    ),
  { functional: true, dispatch: false }
);

// ─── Refresh Token ───────────────────────────────────────
export const refreshToken$ = createEffect(
  (
    actions$ = inject(Actions),
    authService = inject(AuthService),
    tokenService = inject(TokenService),
    store = inject(Store)
  ) =>
    actions$.pipe(
      ofType(AuthActions.refreshToken),
      switchMap(() => {
        const rt = tokenService.refresh();
        if (!rt) return of(AuthActions.tokenExpired());

        return authService.refreshToken(rt).pipe(
          map((res) => {
            if (!res.success || !res.data) {
              return AuthActions.tokenExpired();
            }
            tokenService.setTokens(
              res.data.accessToken,
              res.data.refreshToken,
              res.data.expiresAt
            );
            return AuthActions.refreshTokenSuccess({ auth: res.data });
          }),
          catchError(() => of(AuthActions.tokenExpired()))
        );
      })
    ),
  { functional: true }
);

// ─── Logout ──────────────────────────────────────────────
export const logout$ = createEffect(
  (
    actions$ = inject(Actions),
    tokenService = inject(TokenService),
    router = inject(Router)
  ) =>
    actions$.pipe(
      ofType(AuthActions.logout, AuthActions.tokenExpired),
      tap(() => {
        tokenService.clearTokens();
        router.navigateByUrl('/login');
      })
    ),
  { functional: true, dispatch: false }
);
