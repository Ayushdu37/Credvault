import { createActionGroup, emptyProps, props } from '@ngrx/store';
import { UserProfile, AuthResponse } from '../../core/models/auth.model';

export const AuthActions = createActionGroup({
  source: 'Auth',
  events: {
    // Login
    'Login': props<{ email: string; password: string }>(),
    'Login Success': props<{ auth: AuthResponse }>(),
    'Login Failure': props<{ error: string }>(),

    // Register
    'Register': props<{
      email: string;
      password: string;
      fullName: string;
      phoneNumber: string;
    }>(),
    'Register Success': props<{ email: string }>(),
    'Register Failure': props<{ error: string }>(),

    // Profile (loaded after login success)
    'Load Profile': emptyProps(),
    'Load Profile Success': props<{ user: UserProfile }>(),
    'Load Profile Failure': props<{ error: string }>(),

    // OTP
    'Send OTP': props<{ email: string; purpose: string }>(),
    'Send OTP Success': emptyProps(),
    'Send OTP Failure': props<{ error: string }>(),

    'Verify OTP': props<{ email: string; otpCode: string; purpose: string }>(),
    'Verify OTP Success': emptyProps(),
    'Verify OTP Failure': props<{ error: string }>(),

    // Verify Email
    'Verify Email': props<{ email: string; otpCode: string }>(),
    'Verify Email Success': emptyProps(),
    'Verify Email Failure': props<{ error: string }>(),

    // Reset Password
    'Reset Password': props<{
      email: string;
      otpCode: string;
      newPassword: string;
    }>(),
    'Reset Password Success': emptyProps(),
    'Reset Password Failure': props<{ error: string }>(),

    // Token Refresh
    'Refresh Token': emptyProps(),
    'Refresh Token Success': props<{ auth: AuthResponse }>(),
    'Token Expired': emptyProps(),

    // Logout
    'Logout': emptyProps(),

    // Clear Error (for UI reset)
    'Clear Error': emptyProps(),
  },
});
