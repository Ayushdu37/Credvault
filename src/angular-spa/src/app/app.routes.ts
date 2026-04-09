import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () =>
      import('./features/landing/landing.component').then(m => m.LandingComponent),
  },
  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/login/login.component').then(m => m.LoginComponent),
  },
  {
    path: 'register',
    loadComponent: () =>
      import('./features/auth/register/register.component').then(m => m.RegisterComponent),
  },
  {
    path: 'forgot-password',
    loadComponent: () =>
      import('./features/auth/forgot-password/forgot-password.component').then(m => m.ForgotPasswordComponent),
  },
  {
    path: 'reset-password',
    loadComponent: () =>
      import('./features/auth/reset-password/reset-password.component').then(m => m.ResetPasswordComponent),
  },
  {
    path: 'mfa',
    loadComponent: () =>
      import('./features/auth/mfa/mfa.component').then(m => m.MfaComponent),
  },
  {
    path: 'verify-email',
    loadComponent: () =>
      import('./features/auth/verify-email/verify-email.component').then(m => m.VerifyEmailComponent),
  },
  {
    path: '',
    canActivate: [authGuard],
    children: [
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent),
      },
      {
        path: 'cards',
        loadComponent: () =>
          import('./features/cards/card-list/card-list.component').then(m => m.CardListComponent),
      },
      {
        path: 'cards/:id',
        loadComponent: () =>
          import('./features/cards/card-detail/card-detail.component').then(m => m.CardDetailComponent),
      },
      {
        path: 'billing',
        loadComponent: () =>
          import('./features/billing/bills-list/bills-list.component').then(m => m.BillsListComponent),
      },
      {
        path: 'billing/:id',
        loadComponent: () =>
          import('./features/billing/bill-detail/bill-detail.component').then(m => m.BillDetailComponent),
      },
      {
        path: 'payments',
        loadComponent: () =>
          import('./features/payments/payment-history/payment-history.component').then(m => m.PaymentHistoryComponent),
      },
      {
        path: 'payments/pay',
        loadComponent: () =>
          import('./features/payments/pay-bill/pay-bill.component').then(m => m.PayBillComponent),
      },
      {
        path: 'payments/:id',
        loadComponent: () =>
          import('./features/payments/payment-detail/payment-detail.component').then(m => m.PaymentDetailComponent),
      },
      {
        path: 'notifications',
        loadComponent: () =>
          import('./features/notifications/notifications-list/notifications-list.component').then(m => m.NotificationsListComponent),
      },
      {
        path: 'payment-methods',
        loadComponent: () =>
          import('./features/payment-methods/payment-methods/payment-methods.component').then(m => m.PaymentMethodsComponent),
      },
      {
        path: 'profile',
        loadComponent: () =>
          import('./features/profile/profile/profile.component').then(m => m.ProfileComponent),
      },
      {
        path: 'rewards',
        loadComponent: () =>
          import('./features/rewards/rewards/rewards.component').then(m => m.RewardsComponent),
      },
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
    ],
  },
  { path: '**', redirectTo: 'login' },
];
