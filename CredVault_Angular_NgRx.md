# CredVault

**Angular SPA · Component Guide & NgRx Playbook**

*v2 Architecture · Angular 19 · NgRx 18 · Signals*

| | |
|---|---|
| **Angular version** | 19 — standalone components, no NgModules |
| **State management** | NgRx 18 — Store, Effects, Selectors, Entity |
| **Signals** | Used alongside NgRx for local UI state (loading flags, form state) |
| **Styling** | CSS with BEM — one `.css` per component |
| **HTTP** | HttpClient via `api.service.ts` base — all calls go through Ocelot gateway |
| **Auth** | JWT in-memory signal — never localStorage |
| **Lazy loading** | Every feature module is lazy-loaded via `loadComponent` / `loadChildren` |

---

## 1. Why NgRx — and where NOT to use it

NgRx is the right tool when state is shared across multiple components, needs to survive navigation, or triggers side effects like HTTP calls. It is the wrong tool for purely local, ephemeral UI state that no other component cares about.

| State type | Examples in CredVault | Tool to use |
|---|---|---|
| Global shared state | logged-in user, JWT token, active card list, current bill list | NgRx Store |
| Server-fetched lists | payments history, reward transactions, notification logs | NgRx Store + Effects |
| Cross-feature data | selected card ID used by both billing and payment features | NgRx Store |
| Local UI state | form dirty flag, accordion open/closed, tab index | Angular signal (component-level) |
| Loading / error per request | spinner visible while API call runs | NgRx Store (`loading$` selector) |
| Form input values | text typed in Add Card form before submit | Angular signals + ReactiveFormsModule |

> **NgRx slice ownership rule**
>
> Each feature module owns exactly one NgRx feature slice. Components in that feature only dispatch actions and select from their own slice. Cross-feature reads use selectors from the feature that owns the data — never import another feature's actions.

---

## 2. Angular SPA Folder Structure

The full folder layout of the `angular-spa/` project. Every feature has its own subfolder under `app/features/` and its own NgRx store subfolder.

```
angular-spa/src/app/

core/
  interceptors/
    auth.interceptor.ts       ← attaches Bearer token to every request
    error.interceptor.ts      ← handles 401 (auto-refresh), 5xx (toast)
  guards/
    auth.guard.ts             ← blocks unauthenticated routes
    role.guard.ts             ← blocks by JWT Role claim
    kyc.guard.ts              ← blocks card routes until email verified
  services/
    auth.service.ts           ← login / register / OTP / logout
    api.service.ts            ← base HttpClient wrapper
    token.service.ts          ← JWT lifecycle, expiry, refresh scheduling

shared/
  components/
    button/                   ← reusable button with loading slot
    card/                     ← generic content card wrapper
    spinner/                  ← full-page and inline spinner
    navbar/                   ← top nav with badge and notification bell
    toast/                    ← success / error toast (dispatched from effects)
    empty-state/              ← no-data placeholder for lists
  pipes/
    currency-inr.pipe.ts
    relative-date.pipe.ts

store/                        ← ROOT store (auth slice only)
  auth/
    auth.actions.ts
    auth.reducer.ts
    auth.effects.ts
    auth.selectors.ts

features/
  auth/                       ← login · register · verify · mfa · reset
  dashboard/                  ← summary cards driven by billing + card selectors
  cards/                      ← card list · add · detail
    store/                    ← cards NgRx feature slice
  billing/                    ← bill list · detail · rewards summary
    store/                    ← billing NgRx feature slice
  payments/                   ← pay-bill · payment history
    store/                    ← payments NgRx feature slice
  notifications/              ← notification feed · preferences
    store/                    ← notifications NgRx feature slice
  payment-methods/            ← linked accounts · UPI · wallets
    store/                    ← payment-methods NgRx feature slice
  profile/                    ← view / edit profile

app.routes.ts
app.component.ts
```

---

## 3. NgRx Setup — Installation & Root Config

### 3.1 Install packages

```bash
npm install @ngrx/store @ngrx/effects @ngrx/entity @ngrx/store-devtools
```

### 3.2 Root store wiring in `app.config.ts`

Angular 19 uses standalone bootstrapping. The root store is wired in `app.config.ts`, not a module.

```typescript
// app.config.ts
import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideStore } from '@ngrx/store';
import { provideEffects } from '@ngrx/effects';
import { provideStoreDevtools } from '@ngrx/store-devtools';
import { routes } from './app.routes';
import { authReducer } from './store/auth/auth.reducer';
import { AuthEffects } from './store/auth/auth.effects';
import { authInterceptor } from './core/interceptors/auth.interceptor';
import { errorInterceptor } from './core/interceptors/error.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor, errorInterceptor])),
    provideStore({ auth: authReducer }), // root slice — auth only
    provideEffects([AuthEffects]),
    provideStoreDevtools({ maxAge: 25, logOnly: false }),
  ],
};
```

### 3.3 Feature store lazy registration

Each feature slice is registered when its route lazy-loads. NgRx `provideState()` and `provideEffects()` go inside the route definition.

```typescript
// app.routes.ts
import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: 'cards',
    canActivate: [authGuard],
    loadChildren: () => import('./features/cards/cards.routes').then(m => m.CARDS_ROUTES),
  },
  {
    path: 'billing',
    canActivate: [authGuard],
    loadChildren: () => import('./features/billing/billing.routes').then(m => m.BILLING_ROUTES),
  },
  {
    path: 'payments',
    canActivate: [authGuard],
    loadChildren: () => import('./features/payments/payments.routes').then(m => m.PAYMENT_ROUTES),
  },
  {
    path: 'notifications',
    canActivate: [authGuard],
    loadChildren: () => import('./features/notifications/notifications.routes').then(m => m.NOTIFICATIONS_ROUTES),
  },
  {
    path: 'payment-methods',
    canActivate: [authGuard],
    loadChildren: () => import('./features/payment-methods/payment-methods.routes').then(m => m.PAYMENT_METHODS_ROUTES),
  },
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
];
```

```typescript
// features/cards/cards.routes.ts
import { Routes } from '@angular/router';
import { provideState } from '@ngrx/store';
import { provideEffects } from '@ngrx/effects';
import { cardsReducer } from './store/cards.reducer';
import { CardsEffects } from './store/cards.effects';

export const CARDS_ROUTES: Routes = [
  {
    path: '',
    providers: [
      provideState({ name: 'cards', reducer: cardsReducer }),
      provideEffects([CardsEffects]),
    ],
    children: [
      { path: '', loadComponent: () => import('./card-list/card-list.component') },
      { path: 'add', loadComponent: () => import('./add-card/add-card.component') },
      { path: ':id', loadComponent: () => import('./card-detail/card-detail.component') },
    ],
  },
];
```

---

## 4. Auth Feature — Store, Components & Flow

> **Auth is the only ROOT-level NgRx slice**
>
> Because auth state (user identity, JWT, loading flags) is needed by guards, the navbar, and every HTTP interceptor, it lives in the root store — not a lazy-loaded feature slice. All other features are lazy.

### 4.1 State shape

```typescript
// store/auth/auth.state.ts
export interface AuthState {
  user: {
    id: string;
    email: string;
    firstName: string;
    lastName: string;
    role: string;
    isEmailVerified: boolean;
  } | null;
  accessToken: string | null; // kept in memory — never written to storage
  loading: boolean;
  error: string | null;
}

export const initialAuthState: AuthState = {
  user: null, accessToken: null, loading: false, error: null,
};
```

### 4.2 Actions

```typescript
// store/auth/auth.actions.ts
import { createActionGroup, emptyProps, props } from '@ngrx/store';

export const AuthActions = createActionGroup({
  source: 'Auth',
  events: {
    // Login
    'Login': props<{ email: string; password: string }>(),
    'Login Success': props<{ user: User; accessToken: string }>(),
    'Login Failure': props<{ error: string }>(),

    // Register
    'Register': props<{ firstName: string; lastName: string; email: string; password: string }>(),
    'Register Success': emptyProps(),
    'Register Failure': props<{ error: string }>(),

    // OTP
    'Send OTP': props<{ purpose: 'Login' | 'Payment' | 'PasswordReset' }>(),
    'Verify OTP': props<{ code: string; purpose: string }>(),
    'Verify OTP Success': props<{ accessToken: string }>(),
    'Verify OTP Failure': props<{ error: string }>(),

    // Misc
    'Logout': emptyProps(),
    'Refresh Token': emptyProps(),
    'Refresh Success': props<{ accessToken: string }>(),
    'Token Expired': emptyProps(),
  },
});
```

### 4.3 Reducer

```typescript
// store/auth/auth.reducer.ts
import { createReducer, on } from '@ngrx/store';
import { AuthActions } from './auth.actions';

export const authReducer = createReducer(
  initialAuthState,
  on(AuthActions.login, s => ({ ...s, loading: true, error: null })),
  on(AuthActions.loginSuccess, (s, { user, accessToken }) =>
    ({ ...s, user, accessToken, loading: false })),
  on(AuthActions.loginFailure, (s, { error }) =>
    ({ ...s, error, loading: false })),
  on(AuthActions.logout, AuthActions.tokenExpired, () => initialAuthState),
  on(AuthActions.refreshSuccess, (s, { accessToken }) =>
    ({ ...s, accessToken })),
  on(AuthActions.verifyOTPSuccess, (s, { accessToken }) =>
    ({ ...s, accessToken, loading: false })),
);
```

### 4.4 Effects

```typescript
// store/auth/auth.effects.ts
import { inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { AuthService } from '../../core/services/auth.service';
import { Router } from '@angular/router';
import { catchError, map, switchMap, tap } from 'rxjs/operators';
import { of } from 'rxjs';

export const loginEffect = createEffect(
  (actions$ = inject(Actions), authService = inject(AuthService), router = inject(Router)) =>
    actions$.pipe(
      ofType(AuthActions.login),
      switchMap(({ email, password }) =>
        authService.login(email, password).pipe(
          map(res => AuthActions.loginSuccess({ user: res.user, accessToken: res.accessToken })),
          catchError(err => of(AuthActions.loginFailure({ error: err.message })))
        )
      )
    ),
  { functional: true }
);

export const loginSuccessRedirect = createEffect(
  (actions$ = inject(Actions), router = inject(Router)) =>
    actions$.pipe(
      ofType(AuthActions.loginSuccess),
      tap(() => router.navigateByUrl('/dashboard'))
    ),
  { functional: true, dispatch: false }
);

export const logoutRedirect = createEffect(
  (actions$ = inject(Actions), router = inject(Router)) =>
    actions$.pipe(
      ofType(AuthActions.logout, AuthActions.tokenExpired),
      tap(() => router.navigateByUrl('/login'))
    ),
  { functional: true, dispatch: false }
);
```

### 4.5 Selectors

```typescript
// store/auth/auth.selectors.ts
import { createFeatureSelector, createSelector } from '@ngrx/store';

const selectAuthState = createFeatureSelector<AuthState>('auth');

export const selectUser = createSelector(selectAuthState, s => s.user);
export const selectAccessToken = createSelector(selectAuthState, s => s.accessToken);
export const selectIsLoggedIn = createSelector(selectAuthState, s => !!s.accessToken);
export const selectAuthLoading = createSelector(selectAuthState, s => s.loading);
export const selectAuthError = createSelector(selectAuthState, s => s.error);
export const selectUserRole = createSelector(selectAuthState, s => s.user?.role ?? null);
export const selectEmailVerified = createSelector(selectAuthState, s => s.user?.isEmailVerified ?? false);
```

### 4.6 Auth components

| Component file | What it does | Dispatches | Selects |
|---|---|---|---|
| `login.component.ts` | Email + password form. Shows inline error. Redirects to `/mfa` if OTP required. | `AuthActions.login` | `selectAuthLoading`, `selectAuthError` |
| `register.component.ts` | Multi-field form. On success shows 'check your email' screen. | `AuthActions.register` | `selectAuthLoading`, `selectAuthError` |
| `verify-email.component.ts` | Token in URL param. Calls identity API directly (no NgRx — one-shot). | None | None |
| `mfa.component.ts` | 6-digit OTP input. Dispatches verifyOTP. On success completes login. | `AuthActions.verifyOTP` | `selectAuthLoading`, `selectAuthError` |
| `reset-password.component.ts` | Two-step: enter email → enter OTP + new password. | `AuthActions.sendOTP`, `AuthActions.verifyOTP` | `selectAuthLoading` |

> **How `auth.interceptor.ts` uses the store**
>
> The interceptor injects `Store` and reads `selectAccessToken` as a one-time value per request using `store.selectSignal(selectAccessToken)()`. It attaches it as `Authorization: Bearer <token>`. On 401 response it dispatches `AuthActions.refreshToken()`. The refresh effect calls the API and dispatches `refreshSuccess` (new token stored) or `tokenExpired` (logout + redirect).

---

## 5. Cards Feature — Store, Components & Flow

| | |
|---|---|
| **Feature slice name** | `'cards'` |
| **Lazy-loaded** | Yes — registered in `cards.routes.ts` |
| **API service** | `cards.service.ts` — calls `/api/cards/*` |
| **NgRx Entity** | Yes — `EntityAdapter<CreditCard>` |
| **Key pattern demonstrated** | Multi-entity list, cross-service balance update via `PaymentCompleted` (read from billing events, card balance is updated server-side — frontend re-fetches) |

### 5.1 State shape

```typescript
// features/cards/store/cards.state.ts
import { EntityState, createEntityAdapter } from '@ngrx/entity';

export interface CreditCard {
  id: string;
  maskedNumber: string;
  cardholderName: string;
  expiryMonth: number;
  expiryYear: number;
  issuerName: string;
  creditLimit: number;
  outstandingBalance: number;
  billingCycleStartDay: number;
  isDefault: boolean;
  isVerified: boolean;
  utilization: number; // (outstandingBalance / creditLimit) * 100
}

export interface CardsState extends EntityState<CreditCard> {
  loading: boolean;
  adding: boolean; // spinner on Add Card button only
  error: string | null;
  selectedId: string | null;
  page: number;
  pageSize: number;
  totalCount: number;
}

export const cardsAdapter = createEntityAdapter<CreditCard>();

export const initialCardsState: CardsState = cardsAdapter.getInitialState({
  loading: false, adding: false, error: null, selectedId: null,
  page: 0, pageSize: 10, totalCount: 0,
});
```

### 5.2 Actions

```typescript
export const CardsActions = createActionGroup({
  source: 'Cards',
  events: {
    'Load Cards': props<{ page: number; pageSize: number }>(),
    'Load Cards Success': props<{ cards: CreditCard[]; totalCount: number }>(),
    'Load Cards Failure': props<{ error: string }>(),
    'Add Card': props<{ request: AddCardRequest }>(),
    'Add Card Success': props<{ card: CreditCard }>(),
    'Add Card Failure': props<{ error: string }>(),
    'Remove Card': props<{ cardId: string }>(),
    'Remove Card Success': props<{ cardId: string }>(),
    'Set Default Card': props<{ cardId: string }>(),
    'Set Default Success': props<{ cardId: string }>(),
    'Update Limit': props<{ cardId: string; newLimit: number }>(),
    'Update Limit Success': props<{ cardId: string; newLimit: number }>(),
    'Select Card': props<{ cardId: string }>(),
  },
});
```

### 5.3 Reducer (with EntityAdapter)

```typescript
export const cardsReducer = createReducer(
  initialCardsState,
  on(CardsActions.loadCards, (s, { page, pageSize }) =>
    ({ ...s, loading: true, page, pageSize })),
  on(CardsActions.loadCardsSuccess, (s, { cards, totalCount }) =>
    cardsAdapter.setAll(cards, { ...s, loading: false, totalCount })),
  on(CardsActions.loadCardsFailure, (s, { error }) =>
    ({ ...s, loading: false, error })),
  on(CardsActions.addCard, s => ({ ...s, adding: true })),
  on(CardsActions.addCardSuccess, (s, { card }) =>
    cardsAdapter.addOne(card, { ...s, adding: false })),
  on(CardsActions.addCardFailure, (s, { error }) =>
    ({ ...s, adding: false, error })),
  on(CardsActions.removeCardSuccess, (s, { cardId }) =>
    cardsAdapter.removeOne(cardId, s)),
  on(CardsActions.setDefaultSuccess, (s, { cardId }) => {
    // clear old default, set new
    const updates = Object.values(s.entities).map(c => ({
      id: c!.id, changes: { isDefault: c!.id === cardId }
    }));
    return cardsAdapter.updateMany(updates, s);
  }),
  on(CardsActions.updateLimitSuccess, (s, { cardId, newLimit }) =>
    cardsAdapter.updateOne({ id: cardId, changes: { creditLimit: newLimit } }, s)),
  on(CardsActions.selectCard, (s, { cardId }) =>
    ({ ...s, selectedId: cardId })),
);
```

### 5.4 Selectors

```typescript
const selectCardsState = createFeatureSelector<CardsState>('cards');

const { selectAll, selectEntities } = cardsAdapter.getSelectors(selectCardsState);

export const selectAllCards = selectAll;
export const selectCardsLoading = createSelector(selectCardsState, s => s.loading);
export const selectCardsAdding = createSelector(selectCardsState, s => s.adding);

export const selectSelectedCard = createSelector(
  selectEntities,
  selectCardsState,
  (entities, s) => s.selectedId ? entities[s.selectedId] ?? null : null
);

export const selectDefaultCard = createSelector(
  selectAll, cards => cards.find(c => c.isDefault) ?? null
);

export const selectCardCount = createSelector(selectAll, cards => cards.length);
export const selectCardsPage = createSelector(selectCardsState, s => s.page);
export const selectCardsPageSize = createSelector(selectCardsState, s => s.pageSize);
export const selectCardsTotalCount = createSelector(selectCardsState, s => s.totalCount);
export const selectCardsTotalPages = createSelector(
  selectCardsTotalCount, selectCardsPageSize,
  (total, size) => Math.ceil(total / size)
);
```

### 5.5 Components

| Component | Dispatches | Selects | Notes |
|---|---|---|---|
| `card-list.component.ts` | `CardsActions.loadCards` on init. `CardsActions.selectCard` on click. `CardsActions.setDefaultCard` on toggle. | `selectAllCards`, `selectCardsLoading` | Dispatches `loadCards` in `ngOnInit` if store is empty (check with `selectCardCount === 0`). |
| `add-card.component.ts` | `CardsActions.addCard` on submit. | `selectCardsAdding`, `selectAuthError` — shows spinner on the submit button while `adding === true`. | Reactive form with BIN prefix auto-detection. On `addCardSuccess` effect navigates to `/cards`. |
| `card-detail.component.ts` | `CardsActions.removeCard`, `CardsActions.updateLimit`. | `selectSelectedCard` — populated by `selectCard` dispatch from the list. | Shows utilisation bar as `(outstandingBalance / creditLimit) * 100`. |

---

## 6. Billing Feature — Store, Components & Flow

| | |
|---|---|
| **Feature slice name** | `'billing'` |
| **API service** | `billing.service.ts` — calls `/api/billing/*` |
| **NgRx Entity** | Yes — `EntityAdapter<Bill>` |
| **Key pattern** | Bills + Rewards in one slice (rewards merged into billing-service in v2) |

### 6.1 State shape

```typescript
export interface Bill {
  id: string;
  cardId: string;
  totalAmount: number;
  minimumDue: number;
  amountPaid: number;
  dueDate: string;
  billingMonth: string;
  status: 'Pending' | 'Paid' | 'Overdue' | 'PartiallyPaid';
}

export interface RewardAccount {
  availablePoints: number;
  totalEarned: number;
  tierName: string;
  cashbackPercent: number;
}

export interface BillingState extends EntityState<Bill> {
  loading: boolean;
  selectedBillId: string | null;
  rewardAccount: RewardAccount | null;
  rewardHistory: RewardTransaction[];
  rewardsLoading: boolean;
  error: string | null;
  page: number;
  pageSize: number;
  totalCount: number;
  rewardHistoryPage: number;
  rewardHistoryTotalCount: number;
}
```

### 6.2 Actions

```typescript
export const BillingActions = createActionGroup({
  source: 'Billing',
  events: {
    'Load Bills': emptyProps(),
    'Load Bills Success': props<{ bills: Bill[] }>(),
    'Load Bills Failure': props<{ error: string }>(),
    'Select Bill': props<{ billId: string }>(),
    'Schedule Payment': props<{ billId: string; amount: number; scheduledDate: string }>(),
    'Schedule Success': props<{ billId: string }>(),
    'Load Reward Account': emptyProps(),
    'Load Reward Account Success': props<{ account: RewardAccount }>(),
    'Load Reward History': emptyProps(),
    'Load Reward History Success': props<{ transactions: RewardTransaction[] }>(),
    // Fired by PaymentsActions.completePaymentSuccess to refresh bills
    'Refresh Bill After Payment': props<{ billId: string }>(),
    'Refresh Bill Success': props<{ bill: Bill }>(),
  },
});
```

### 6.3 Cross-feature pattern — payment triggers billing refresh

When a payment completes, the bill's `AmountPaid` and `Status` change server-side. The billing store needs to reflect this without the user manually refreshing. This is handled in billing's Effects — it listens for `PaymentsActions.completePaymentSuccess`.

```typescript
// features/billing/store/billing.effects.ts
export const refreshBillAfterPayment = createEffect(
  (actions$ = inject(Actions), billingService = inject(BillingService)) =>
    actions$.pipe(
      ofType(PaymentsActions.completePaymentSuccess), // listens to payments feature
      switchMap(({ billId }) =>
        billingService.getBillById(billId).pipe(
          map(bill => BillingActions.refreshBillSuccess({ bill })),
          catchError(() => EMPTY)
        )
      )
    ),
  { functional: true }
);
```

> **Why this is the correct pattern**
>
> The payments feature owns payment actions. The billing feature owns bill state. Billing Effects cross-listens to `PaymentsActions.completePaymentSuccess` to know when to re-fetch a bill. This is a one-way dependency — payments never imports billing actions. The data ownership stays clean.

### 6.4 Components

| Component | Dispatches | Selects | Notes |
|---|---|---|---|
| `bills-list.component.ts` | `BillingActions.loadBills` on init. `BillingActions.selectBill` on row click. | `selectAllBills`, `selectBillsLoading` | Groups bills by status. Overdue bills shown with red badge. Paid bills greyed out. |
| `bill-detail.component.ts` | `BillingActions.schedulePay` on form submit. | `selectSelectedBill` | Shows total, minimum due, amount paid, due date. Has Pay Now button (navigates to `/payments/pay/:billId`). Has Schedule Payment accordion. |
| `rewards-summary.component.ts` | `BillingActions.loadRewardAccount`, `loadRewardHistory` on init. | `selectRewardAccount`, `selectRewardHistory`, `selectRewardsLoading` | Shows tier badge (Silver/Gold/Platinum), available points, tier progress bar, and last 10 transactions. |

---

## 7. Payments Feature — Store, Components & Saga Tracking

| | |
|---|---|
| **Feature slice name** | `'payments'` |
| **API service** | `payments.service.ts` — calls `/api/payments/*` |
| **Key pattern** | Tracks the server-side Saga state in the UI — polls or uses SSE to show progress |

> **Tracking the Payment Saga in the frontend**
>
> The Saga on the server goes through states: `Initiated → RiskCheckPassed → Processing → Completed | Failed`. The frontend needs to show the user where their payment is at each stage. The approach used in CredVault v2 is polling: after initiating a payment, the frontend polls `GET /api/payments/{id}` every 1.5 seconds until `Status` is `Completed` or `Failed`. The NgRx effect manages the polling lifecycle.

### 7.1 State shape

```typescript
export interface Payment {
  id: string; cardId: string; billId: string;
  amount: number; paymentType: string;
  status: 'Initiated' | 'Completed' | 'Failed' | 'Reversed';
  failureReason: string | null;
  createdAt: string;
}

export interface PaymentsState {
  history: Payment[];
  historyLoading: boolean;
  activePayment: Payment | null; // the in-progress payment being tracked
  sagaStatus: 'idle' | 'initiated' | 'risk_check' | 'processing' | 'completed' | 'failed';
  sagaError: string | null;
  initiating: boolean;  // spinner on Pay button
  otpRequired: boolean; // show OTP prompt mid-saga
  historyPage: number;
  historyPageSize: number;
  historyTotalCount: number;
}
```

### 7.2 Actions

```typescript
export const PaymentsActions = createActionGroup({
  source: 'Payments',
  events: {
    'Initiate Payment': props<{ billId: string; cardId: string; amount: number; type: string }>(),
    'Initiate Payment Success': props<{ payment: Payment }>(),
    'Initiate Payment Failure': props<{ error: string }>(),
    'Poll Saga Status': props<{ paymentId: string }>(),
    'Poll Saga Tick': props<{ payment: Payment }>(),
    'Saga OTP Required': emptyProps(),
    'Complete Payment Success': props<{ payment: Payment; billId: string }>(),
    'Complete Payment Failed': props<{ reason: string }>(),
    'Stop Polling': emptyProps(),
    'Load History': emptyProps(),
    'Load History Success': props<{ payments: Payment[] }>(),
  },
});
```

### 7.3 Saga polling effect

```typescript
// features/payments/store/payments.effects.ts
import { interval, switchMap, takeUntil, map, filter } from 'rxjs';

export const pollSagaEffect = createEffect(
  (actions$ = inject(Actions), paymentsService = inject(PaymentsService)) =>
    actions$.pipe(
      ofType(PaymentsActions.pollSagaStatus),
      switchMap(({ paymentId }) =>
        interval(1500).pipe(
          switchMap(() => paymentsService.getPaymentById(paymentId)),
          map(payment => {
            if (payment.status === 'Completed')
              return PaymentsActions.completePaymentSuccess({ payment, billId: payment.billId });
            if (payment.status === 'Failed')
              return PaymentsActions.completePaymentFailed({ reason: payment.failureReason ?? 'Unknown' });
            return PaymentsActions.pollSagaTick({ payment });
          }),
          takeUntil(actions$.pipe(
            ofType(
              PaymentsActions.stopPolling,
              PaymentsActions.completePaymentSuccess,
              PaymentsActions.completePaymentFailed
            )
          ))
        )
      )
    ),
  { functional: true }
);
```

### 7.4 Reducer

```typescript
export const paymentsReducer = createReducer(
  initialPaymentsState,
  on(PaymentsActions.initiatePayment, s =>
    ({ ...s, initiating: true, sagaStatus: 'idle', sagaError: null })),
  on(PaymentsActions.initiatePaymentSuccess, (s, { payment }) =>
    ({ ...s, initiating: false, activePayment: payment, sagaStatus: 'initiated' })),
  on(PaymentsActions.pollSagaTick, (s, { payment }) => ({
    ...s,
    activePayment: payment,
    sagaStatus: mapStatusToSagaStatus(payment.status),
  })),
  on(PaymentsActions.sagaOTPRequired, s =>
    ({ ...s, otpRequired: true, sagaStatus: 'risk_check' })),
  on(PaymentsActions.completePaymentSuccess, (s, { payment }) =>
    ({ ...s, activePayment: payment, sagaStatus: 'completed', otpRequired: false })),
  on(PaymentsActions.completePaymentFailed, (s, { reason }) =>
    ({ ...s, sagaStatus: 'failed', sagaError: reason, otpRequired: false })),
);
```

### 7.5 Components

| Component | Dispatches | Selects | Notes |
|---|---|---|---|
| `pay-bill.component.ts` | `PaymentsActions.initiatePayment`. On `sagaStatus === 'risk_check'` + `otpRequired`, show OTP dialog which dispatches `AuthActions.verifyOTP`. | `selectSagaStatus`, `selectInitiating`, `selectSagaError`, `selectActivePayment`, `selectOtpRequired` | Shows a 4-step progress indicator: Submitted → Risk Check → Processing → Done. Each step highlights as `sagaStatus` progresses. This is the centrepiece UI component. |
| `payment-history.component.ts` | `PaymentsActions.loadHistory` on init. | `selectPaymentHistory`, `selectHistoryLoading` | Paginated table. Status badges coloured by value. Failed rows show failure reason on expand. |

---

## 8. Dashboard — Cross-Feature Selectors

The dashboard component aggregates data from multiple stores. It does not have its own NgRx slice — it reads from the cards and billing feature selectors directly. This works because both features are eagerly loaded from the root once the user is authenticated.

> **Dashboard data sources**
>
> - Cards summary → `selectAllCards`, `selectDefaultCard` from the cards store
> - Bills summary → `selectAllBills`, filtered to Pending/Overdue, from billing store
> - Rewards summary → `selectRewardAccount` from billing store
> - Total outstanding → derived selector combining all cards

### 8.1 Dashboard derived selectors

```typescript
// features/dashboard/dashboard.selectors.ts
// Combines selectors from cards + billing feature stores

export const selectTotalOutstanding = createSelector(
  selectAllCards,
  cards => cards.reduce((sum, c) => sum + c.outstandingBalance, 0)
);

export const selectTotalCreditLimit = createSelector(
  selectAllCards,
  cards => cards.reduce((sum, c) => sum + c.creditLimit, 0)
);

export const selectOverallUtilization = createSelector(
  selectTotalOutstanding,
  selectTotalCreditLimit,
  (outstanding, limit) => limit > 0 ? (outstanding / limit) * 100 : 0
);

export const selectUpcomingBills = createSelector(
  selectAllBills,
  bills => bills
    .filter(b => b.status === 'Pending' || b.status === 'Overdue')
    .sort((a, b) => new Date(a.dueDate).getTime() - new Date(b.dueDate).getTime())
    .slice(0, 3) // show next 3 only on dashboard
);
```

### 8.2 Dashboard component

```typescript
// features/dashboard/dashboard.component.ts
@Component({
  standalone: true,
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  imports: [AsyncPipe, CardComponent, CurrencyInrPipe, RelativeDatePipe],
})
export class DashboardComponent {
  private store = inject(Store);

  // Dispatch both feature loads on init
  constructor() {
    this.store.dispatch(CardsActions.loadCards());
    this.store.dispatch(BillingActions.loadBills());
    this.store.dispatch(BillingActions.loadRewardAccount());
  }

  cards$ = this.store.select(selectAllCards);
  totalOutstanding = this.store.selectSignal(selectTotalOutstanding);
  utilization = this.store.selectSignal(selectOverallUtilization);
  upcomingBills$ = this.store.select(selectUpcomingBills);
  rewardAccount = this.store.selectSignal(selectRewardAccount);
  user = this.store.selectSignal(selectUser);
}
```

---

## 9. NgRx Patterns Quick Reference

### 9.1 When to use `selectSignal` vs `select`

```typescript
// selectSignal → use in template or computed() — Angular signal, no async pipe needed
loading = this.store.selectSignal(selectCardsLoading);
// Template: @if (loading()) { <app-spinner /> }

// select → use with async pipe when you need RxJS operators (filter, combineLatest)
cards$ = this.store.select(selectAllCards);
// Template: @for (card of cards$ | async; track card.id) { ... }
```

### 9.2 Dispatching on component init safely

Never dispatch in a constructor if the effect depends on router params. Use `ngOnInit` or inject `ActivatedRoute` and dispatch after reading params.

```typescript
// CORRECT — dispatch only when store is empty, avoid redundant API calls
ngOnInit() {
  this.store.select(selectCardCount).pipe(
    take(1),
    filter(count => count === 0)
  ).subscribe(() => this.store.dispatch(CardsActions.loadCards()));
}

// CORRECT — dispatch with route param
ngOnInit() {
  const id = this.route.snapshot.paramMap.get('id')!;
  this.store.dispatch(CardsActions.selectCard({ cardId: id }));
}
```

### 9.3 Error handling pattern — effect → reducer → template

```typescript
// Effect catches error and dispatches failure action
catchError(err => of(CardsActions.addCardFailure({ error: err.error?.message ?? 'Failed' })))

// Reducer stores it
on(CardsActions.addCardFailure, (s, { error }) => ({ ...s, adding: false, error })),

// Selector exposes it
export const selectCardsError = createSelector(selectCardsState, s => s.error);

// Component reads it as a signal — shows inline alert
error = this.store.selectSignal(selectCardsError);

// Template: @if (error()) { <div class='alert'>{{ error() }}</div> }
```

### 9.4 Toast notifications from effects

Effects can trigger toasts without a dedicated toast NgRx slice. Inject a `ToastService` (a simple signal-based service) directly in the effect.

```typescript
// core/services/toast.service.ts
export class ToastService {
  messages = signal<{ type: 'success' | 'error'; text: string }[]>([]);

  show(type: 'success' | 'error', text: string) {
    this.messages.update(m => [...m, { type, text }]);
    setTimeout(() => this.messages.update(m => m.slice(1)), 3500);
  }
}

// In an effect (functional style):
export const addCardSuccessToast = createEffect(
  (actions$ = inject(Actions), toast = inject(ToastService)) =>
    actions$.pipe(
      ofType(CardsActions.addCardSuccess),
      tap(() => toast.show('success', 'Card added successfully')),
    ),
  { functional: true, dispatch: false }
);
```

---

## 10. Complete Component Build Checklist

Every component in the SPA in build order. Tick off as you complete each one. Start with core infrastructure (interceptors, guards, store) before building any feature components.

### Phase 1 — Core infrastructure (Day 11)

| File | Type | Depends on |
|---|---|---|
| `auth.service.ts` | Service | HttpClient, TokenService |
| `api.service.ts` | Service | HttpClient |
| `token.service.ts` | Service | Store (`selectAccessToken`) |
| `auth.interceptor.ts` | Interceptor | Store, TokenService |
| `error.interceptor.ts` | Interceptor | Store, ToastService |
| `auth.guard.ts` | Guard | Store (`selectIsLoggedIn`) |
| `role.guard.ts` | Guard | Store (`selectUserRole`) |
| `kyc.guard.ts` | Guard | Store (`selectEmailVerified`) |
| `auth.actions / reducer / effects / selectors` | NgRx root slice | AuthService |
| `toast.service.ts` | Service | Angular signal |

### Phase 2 — Shared components (Day 11)

| Component | Inputs / Outputs | Used by |
|---|---|---|
| `button.component.ts` | `@Input loading: boolean;` `@Input variant: 'primary'\|'ghost'` | All feature forms |
| `card.component.ts` | `@Input title: string;` content: `ng-content` | Dashboard, lists, detail pages |
| `spinner.component.ts` | `@Input fullPage: boolean` | All loading states |
| `navbar.component.ts` | Reads `selectUser`, `selectCardCount` signals | `app.component.ts` (always visible) |
| `toast.component.ts` | Reads `ToastService.messages` signal | `app.component.ts` (always visible) |
| `empty-state.component.ts` | `@Input message: string;` `@Input icon: string` | All list components |
| `currency-inr.pipe.ts` | Pure pipe — number → string | Dashboard, billing, payments |
| `relative-date.pipe.ts` | Pure pipe — ISO string → `'2 days ago'` | Billing, notifications |

### Phase 3 — Auth feature (Day 11)

| Component | Route | Key behaviour |
|---|---|---|
| `login.component.ts` | `/login` | Reactive form. Dispatches `AuthActions.login`. On `loginSuccess` effect navigates to `/dashboard`. On MFA needed: navigates to `/mfa`. |
| `register.component.ts` | `/register` | Reactive form with password confirmation. Dispatches `AuthActions.register`. On success shows 'check your email' view. |
| `verify-email.component.ts` | `/verify-email` | Reads token param from URL. Calls identity API directly (no store — one-shot side effect). Shows success/failure. |
| `mfa.component.ts` | `/mfa` | 6-digit OTP input. Dispatches `AuthActions.verifyOTP`. On success effect completes login and navigates to `/dashboard`. |
| `reset-password.component.ts` | `/reset-password` | Step 1: enter email → dispatches `sendOTP`. Step 2: enter OTP + new password → dispatches `verifyOTP` then calls reset API. |

### Phase 4 — Cards feature (Day 12)

| Component | Route | Key behaviour |
|---|---|---|
| `card-list.component.ts` | `/cards` | Dispatches `loadCards` on init if store empty. Renders card tiles with issuer logo, masked number, utilisation ring, and outstanding balance. Tap → navigate to `/cards/:id`. |
| `add-card.component.ts` | `/cards/add` | Multi-step form: card details → verify BIN → confirm. Dispatches `addCard`. Spinner on submit button while `adding === true`. |
| `card-detail.component.ts` | `/cards/:id` | Dispatches `selectCard` with route param. Shows full card info, utilisation bar, set-default toggle, update-limit input, and remove card button with confirmation dialog. |

### Phase 5 — Billing feature (Day 12)

| Component | Route | Key behaviour |
|---|---|---|
| `bills-list.component.ts` | `/billing` | Dispatches `loadBills`. Groups by status: Overdue (red) at top, Pending (amber), Paid (green) at bottom. Each row shows card, month, amount, due date, and status badge. |
| `bill-detail.component.ts` | `/billing/:id` | Dispatches `selectBill`. Shows bill breakdown. Two CTAs: Pay Now (navigate to `/payments/pay/:billId`) and Schedule Payment (inline accordion form). |
| `rewards-summary.component.ts` | `/billing/rewards` | Dispatches `loadRewardAccount` + `loadRewardHistory`. Shows tier badge with progress bar to next tier, available points, and last 20 transactions table. |

### Phase 6 — Payments feature (Day 12)

| Component | Route | Key behaviour |
|---|---|---|
| `pay-bill.component.ts` | `/payments/pay/:billId` | Reads bill from billing store via `billId`. Lets user choose Full / Partial amount and select card. On submit dispatches `initiatePayment`. Shows 4-step Saga progress UI. If `sagaStatus` becomes `risk_check` and `otpRequired`, shows OTP modal overlay. On `completed`: shows success screen with receipt. On `failed`: shows error reason with retry option. |
| `payment-history.component.ts` | `/payments/history` | Dispatches `loadHistory`. Paginated table of all payments. Columns: date, card, bill month, amount, type, status. Failed rows expand to show failure reason. Completed rows link to the original bill. |

### Phase 7 — Dashboard & Profile (Day 12)

| Component | Route | Key behaviour |
|---|---|---|
| `dashboard.component.ts` | `/dashboard` | Dispatches `loadCards`, `loadBills`, `loadRewardAccount` on init. Renders: total outstanding widget, overall utilisation donut, next 3 upcoming bills, reward tier badge, and quick-action buttons (Add Card, Pay Bill). |
| `profile.component.ts` | `/profile` | Reads `selectUser` signal. Displays name and email (read-only). Has Change Password flow: dispatches `sendOTP` (`Purpose=PasswordReset`) then `verifyOTP` then calls reset API. |

### Phase 8 — Notifications feature (Day 13)

| Component | Route | Key behaviour |
|---|---|---|
| `notification-list.component.ts` | `/notifications` | Dispatches `loadNotifications` on init with pagination. Renders notification cards grouped by date (Today, Yesterday, Earlier). Each card shows icon, title, message, and relative timestamp. Unread items have left accent border in `#4F46E5`. |
| `notification-preferences.component.ts` | `/notifications/preferences` | Dispatches `loadPreferences` on init. Toggle switches for each notification category (Payments, Billing, Security, Rewards). On change dispatches `updatePreferences`. |

### Phase 9 — Payment Methods feature (Day 13)

| Component | Route | Key behaviour |
|---|---|---|
| `payment-methods-list.component.ts` | `/payment-methods` | Dispatches `loadPaymentMethods` on init. Renders cards for each linked method (bank account, UPI, wallet) with masked account number, type icon, and default badge. |
| `add-payment-method.component.ts` | `/payment-methods/add` | Multi-step form: select type → enter details → verify. Dispatches `addPaymentMethod`. On success navigates back to list. |

---

## 11. Pagination Pattern — Reusable Across All Features

Every list in CredVault uses the same pagination pattern. The Cards feature (Section 5) is the canonical implementation. This section documents the reusable contract.

### 11.1 Pagination interface

```typescript
// shared/models/pagination.model.ts
export interface PaginatedRequest {
  page: number;    // 0-indexed page number
  pageSize: number; // items per page (default 10)
}

export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}
```

### 11.2 API service pattern

```typescript
// Every feature API service follows this pattern
getCards(page: number, pageSize: number): Observable<PaginatedResponse<CreditCard>> {
  return this.http.get<PaginatedResponse<CreditCard>>(
    `${this.baseUrl}/api/cards?page=${page}&pageSize=${pageSize}`
  );
}
```

### 11.3 State shape contract

Every feature that has a paginated list must include these fields in its state:

```typescript
interface PaginatedFeatureState {
  page: number;          // current page (0-indexed)
  pageSize: number;      // items per page
  totalCount: number;    // total items from API
}
```

### 11.4 Reducer pattern

```typescript
// Load action stores page/pageSize from the dispatch
on(Actions.load, (s, { page, pageSize }) =>
  ({ ...s, loading: true, page, pageSize })),

// Success action stores totalCount from API response
on(Actions.loadSuccess, (s, { items, totalCount }) =>
  adapter.setAll(items, { ...s, loading: false, totalCount })),
```

### 11.5 Selector pattern

```typescript
export const selectCurrentPage = createSelector(selectState, s => s.page);
export const selectPageSize = createSelector(selectState, s => s.pageSize);
export const selectTotalCount = createSelector(selectState, s => s.totalCount);
export const selectTotalPages = createSelector(
  selectTotalCount, selectPageSize,
  (total, size) => Math.ceil(total / size)
);
export const selectHasNextPage = createSelector(
  selectCurrentPage, selectTotalPages,
  (page, total) => page < total - 1
);
export const selectHasPreviousPage = createSelector(
  selectCurrentPage, page => page > 0
);
```

### 11.6 Component dispatch pattern

```typescript
// Component
pageSize = 10;
currentPage = this.store.selectSignal(selectCurrentPage);
totalPages = this.store.selectSignal(selectTotalPages);
hasNext = this.store.selectSignal(selectHasNextPage);
hasPrev = this.store.selectSignal(selectHasPreviousPage);

ngOnInit() {
  this.store.dispatch(Actions.load({ page: 0, pageSize: this.pageSize }));
}

onPageChange(page: number) {
  this.store.dispatch(Actions.load({ page, pageSize: this.pageSize }));
}

onNextPage() {
  this.onPageChange(this.currentPage() + 1);
}

onPreviousPage() {
  this.onPageChange(this.currentPage() - 1);
}
```

### 11.7 Shared paginator component

```typescript
// shared/components/paginator/paginator.component.ts
@Component({
  standalone: true,
  selector: 'app-paginator',
  template: `
    <nav class="paginator">
      <button [disabled]="!hasPrev()" (click)="prev.emit()">← Previous</button>
      <span class="paginator__info">Page {{ page() + 1 }} of {{ totalPages() }}</span>
      <button [disabled]="!hasNext()" (click)="next.emit()">Next →</button>
    </nav>
  `,
})
export class PaginatorComponent {
  page = input.required<number>();
  totalPages = input.required<number>();
  hasNext = input.required<boolean>();
  hasPrev = input.required<boolean>();
  next = output<void>();
  prev = output<void>();
}
```

### 11.8 Features using pagination

| Feature | List component | Load action | Paginated fields |
|---|---|---|---|
| Cards | `card-list` | `CardsActions.loadCards` | `page`, `pageSize`, `totalCount` |
| Billing | `bills-list` | `BillingActions.loadBills` | `page`, `pageSize`, `totalCount` |
| Billing → Rewards | `rewards-summary` | `BillingActions.loadRewardHistory` | `rewardHistoryPage`, `rewardHistoryTotalCount` |
| Payments | `payment-history` | `PaymentsActions.loadHistory` | `historyPage`, `historyPageSize`, `historyTotalCount` |
| Notifications | `notification-list` | `NotificationsActions.loadNotifications` | `page`, `pageSize`, `totalCount` |
| Payment Methods | `payment-methods-list` | `PaymentMethodsActions.loadPaymentMethods` | `page`, `pageSize`, `totalCount` |

---

## 12. Notifications Feature — Store, Components & Flow

| | |
|---|---|
| **Feature slice name** | `'notifications'` |
| **Lazy-loaded** | Yes — registered in `notifications.routes.ts` |
| **API service** | `notifications.service.ts` — calls `/api/notifications/*` |
| **Key pattern** | Unread count badge in navbar, mark-read on view, preference management |

### 12.1 State shape

```typescript
// features/notifications/store/notifications.state.ts
export interface Notification {
  id: string;
  userId: string;
  title: string;
  message: string;
  type: 'Payment' | 'Billing' | 'Security' | 'Reward' | 'System';
  isRead: boolean;
  createdAt: string;
}

export interface NotificationPreferences {
  paymentAlerts: boolean;
  billingReminders: boolean;
  securityAlerts: boolean;
  rewardUpdates: boolean;
  emailNotifications: boolean;
}

export interface NotificationsState {
  notifications: Notification[];
  loading: boolean;
  error: string | null;
  unreadCount: number;
  preferences: NotificationPreferences | null;
  preferencesLoading: boolean;
  page: number;
  pageSize: number;
  totalCount: number;
}

export const initialNotificationsState: NotificationsState = {
  notifications: [], loading: false, error: null,
  unreadCount: 0, preferences: null, preferencesLoading: false,
  page: 0, pageSize: 20, totalCount: 0,
};
```

### 12.2 Actions

```typescript
export const NotificationsActions = createActionGroup({
  source: 'Notifications',
  events: {
    'Load Notifications': props<{ page: number; pageSize: number }>(),
    'Load Notifications Success': props<{ notifications: Notification[]; totalCount: number }>(),
    'Load Notifications Failure': props<{ error: string }>(),
    'Mark As Read': props<{ notificationId: string }>(),
    'Mark As Read Success': props<{ notificationId: string }>(),
    'Mark All As Read': emptyProps(),
    'Mark All As Read Success': emptyProps(),
    'Load Unread Count': emptyProps(),
    'Load Unread Count Success': props<{ count: number }>(),
    'Load Preferences': emptyProps(),
    'Load Preferences Success': props<{ preferences: NotificationPreferences }>(),
    'Update Preferences': props<{ preferences: Partial<NotificationPreferences> }>(),
    'Update Preferences Success': props<{ preferences: NotificationPreferences }>(),
  },
});
```

### 12.3 Reducer

```typescript
export const notificationsReducer = createReducer(
  initialNotificationsState,
  on(NotificationsActions.loadNotifications, (s, { page, pageSize }) =>
    ({ ...s, loading: true, page, pageSize })),
  on(NotificationsActions.loadNotificationsSuccess, (s, { notifications, totalCount }) =>
    ({ ...s, notifications, loading: false, totalCount })),
  on(NotificationsActions.loadNotificationsFailure, (s, { error }) =>
    ({ ...s, loading: false, error })),
  on(NotificationsActions.markAsReadSuccess, (s, { notificationId }) => ({
    ...s,
    notifications: s.notifications.map(n =>
      n.id === notificationId ? { ...n, isRead: true } : n),
    unreadCount: Math.max(0, s.unreadCount - 1),
  })),
  on(NotificationsActions.markAllAsReadSuccess, s => ({
    ...s,
    notifications: s.notifications.map(n => ({ ...n, isRead: true })),
    unreadCount: 0,
  })),
  on(NotificationsActions.loadUnreadCountSuccess, (s, { count }) =>
    ({ ...s, unreadCount: count })),
  on(NotificationsActions.loadPreferences, s =>
    ({ ...s, preferencesLoading: true })),
  on(NotificationsActions.loadPreferencesSuccess, (s, { preferences }) =>
    ({ ...s, preferences, preferencesLoading: false })),
  on(NotificationsActions.updatePreferencesSuccess, (s, { preferences }) =>
    ({ ...s, preferences })),
);
```

### 12.4 Selectors

```typescript
const selectNotificationsState = createFeatureSelector<NotificationsState>('notifications');

export const selectAllNotifications = createSelector(selectNotificationsState, s => s.notifications);
export const selectNotificationsLoading = createSelector(selectNotificationsState, s => s.loading);
export const selectUnreadCount = createSelector(selectNotificationsState, s => s.unreadCount);
export const selectNotificationPreferences = createSelector(selectNotificationsState, s => s.preferences);
export const selectNotificationsPage = createSelector(selectNotificationsState, s => s.page);
export const selectNotificationsTotalCount = createSelector(selectNotificationsState, s => s.totalCount);
export const selectNotificationsTotalPages = createSelector(
  selectNotificationsTotalCount,
  createSelector(selectNotificationsState, s => s.pageSize),
  (total, size) => Math.ceil(total / size)
);

// Grouped by date for the UI
export const selectNotificationsGroupedByDate = createSelector(
  selectAllNotifications,
  notifications => {
    const today = new Date().toDateString();
    const yesterday = new Date(Date.now() - 86400000).toDateString();
    return {
      today: notifications.filter(n => new Date(n.createdAt).toDateString() === today),
      yesterday: notifications.filter(n => new Date(n.createdAt).toDateString() === yesterday),
      earlier: notifications.filter(n => {
        const d = new Date(n.createdAt).toDateString();
        return d !== today && d !== yesterday;
      }),
    };
  }
);
```

### 12.5 Effects

```typescript
export const loadNotificationsEffect = createEffect(
  (actions$ = inject(Actions), notifService = inject(NotificationsService)) =>
    actions$.pipe(
      ofType(NotificationsActions.loadNotifications),
      switchMap(({ page, pageSize }) =>
        notifService.getNotifications(page, pageSize).pipe(
          map(res => NotificationsActions.loadNotificationsSuccess({
            notifications: res.items, totalCount: res.totalCount
          })),
          catchError(err => of(NotificationsActions.loadNotificationsFailure({ error: err.message })))
        )
      )
    ),
  { functional: true }
);

export const markAsReadEffect = createEffect(
  (actions$ = inject(Actions), notifService = inject(NotificationsService)) =>
    actions$.pipe(
      ofType(NotificationsActions.markAsRead),
      switchMap(({ notificationId }) =>
        notifService.markAsRead(notificationId).pipe(
          map(() => NotificationsActions.markAsReadSuccess({ notificationId })),
          catchError(() => EMPTY)
        )
      )
    ),
  { functional: true }
);

export const markAllAsReadEffect = createEffect(
  (actions$ = inject(Actions), notifService = inject(NotificationsService)) =>
    actions$.pipe(
      ofType(NotificationsActions.markAllAsRead),
      switchMap(() =>
        notifService.markAllAsRead().pipe(
          map(() => NotificationsActions.markAllAsReadSuccess()),
          catchError(() => EMPTY)
        )
      )
    ),
  { functional: true }
);
```

> **Navbar unread badge**
>
> The `navbar.component.ts` dispatches `NotificationsActions.loadUnreadCount` on init and reads `selectUnreadCount` as a signal to display the badge. This works because the navbar is always mounted inside the app shell — the notifications slice must be eagerly loaded or the unread count selector must be a root-level selector. Recommended approach: register the `unreadCount` state in the root store alongside auth, and keep the full notifications list as a lazy feature slice.

### 12.6 Routes

```typescript
// features/notifications/notifications.routes.ts
export const NOTIFICATIONS_ROUTES: Routes = [
  {
    path: '',
    providers: [
      provideState({ name: 'notifications', reducer: notificationsReducer }),
      provideEffects([NotificationsEffects]),
    ],
    children: [
      { path: '', loadComponent: () => import('./notification-list/notification-list.component') },
      { path: 'preferences', loadComponent: () => import('./notification-preferences/notification-preferences.component') },
    ],
  },
];
```

---

## 13. Payment Methods Feature — Store, Components & Flow

| | |
|---|---|
| **Feature slice name** | `'paymentMethods'` |
| **Lazy-loaded** | Yes — registered in `payment-methods.routes.ts` |
| **API service** | `payment-methods.service.ts` — calls `/api/payment-methods/*` |
| **NgRx Entity** | Yes — `EntityAdapter<PaymentMethod>` |
| **Key pattern** | Entity adapter for list, default method selection, cross-feature usage by payments |

### 13.1 State shape

```typescript
// features/payment-methods/store/payment-methods.state.ts
import { EntityState, createEntityAdapter } from '@ngrx/entity';

export interface PaymentMethod {
  id: string;
  userId: string;
  type: 'BankAccount' | 'UPI' | 'Wallet' | 'DebitCard';
  displayName: string;    // "HDFC ••••4521"
  maskedAccount: string;
  isDefault: boolean;
  isVerified: boolean;
  addedAt: string;
}

export interface PaymentMethodsState extends EntityState<PaymentMethod> {
  loading: boolean;
  adding: boolean;
  error: string | null;
  page: number;
  pageSize: number;
  totalCount: number;
}

export const paymentMethodsAdapter = createEntityAdapter<PaymentMethod>();

export const initialPaymentMethodsState: PaymentMethodsState = paymentMethodsAdapter.getInitialState({
  loading: false, adding: false, error: null,
  page: 0, pageSize: 10, totalCount: 0,
});
```

### 13.2 Actions

```typescript
export const PaymentMethodsActions = createActionGroup({
  source: 'PaymentMethods',
  events: {
    'Load Payment Methods': props<{ page: number; pageSize: number }>(),
    'Load Payment Methods Success': props<{ methods: PaymentMethod[]; totalCount: number }>(),
    'Load Payment Methods Failure': props<{ error: string }>(),
    'Add Payment Method': props<{ request: AddPaymentMethodRequest }>(),
    'Add Payment Method Success': props<{ method: PaymentMethod }>(),
    'Add Payment Method Failure': props<{ error: string }>(),
    'Remove Payment Method': props<{ methodId: string }>(),
    'Remove Payment Method Success': props<{ methodId: string }>(),
    'Set Default Method': props<{ methodId: string }>(),
    'Set Default Method Success': props<{ methodId: string }>(),
  },
});
```

### 13.3 Reducer

```typescript
export const paymentMethodsReducer = createReducer(
  initialPaymentMethodsState,
  on(PaymentMethodsActions.loadPaymentMethods, (s, { page, pageSize }) =>
    ({ ...s, loading: true, page, pageSize })),
  on(PaymentMethodsActions.loadPaymentMethodsSuccess, (s, { methods, totalCount }) =>
    paymentMethodsAdapter.setAll(methods, { ...s, loading: false, totalCount })),
  on(PaymentMethodsActions.loadPaymentMethodsFailure, (s, { error }) =>
    ({ ...s, loading: false, error })),
  on(PaymentMethodsActions.addPaymentMethod, s => ({ ...s, adding: true })),
  on(PaymentMethodsActions.addPaymentMethodSuccess, (s, { method }) =>
    paymentMethodsAdapter.addOne(method, { ...s, adding: false })),
  on(PaymentMethodsActions.addPaymentMethodFailure, (s, { error }) =>
    ({ ...s, adding: false, error })),
  on(PaymentMethodsActions.removePaymentMethodSuccess, (s, { methodId }) =>
    paymentMethodsAdapter.removeOne(methodId, s)),
  on(PaymentMethodsActions.setDefaultMethodSuccess, (s, { methodId }) => {
    const updates = Object.values(s.entities).map(m => ({
      id: m!.id, changes: { isDefault: m!.id === methodId }
    }));
    return paymentMethodsAdapter.updateMany(updates, s);
  }),
);
```

### 13.4 Selectors

```typescript
const selectPaymentMethodsState = createFeatureSelector<PaymentMethodsState>('paymentMethods');
const { selectAll, selectEntities } = paymentMethodsAdapter.getSelectors(selectPaymentMethodsState);

export const selectAllPaymentMethods = selectAll;
export const selectPaymentMethodsLoading = createSelector(selectPaymentMethodsState, s => s.loading);
export const selectPaymentMethodsAdding = createSelector(selectPaymentMethodsState, s => s.adding);
export const selectDefaultPaymentMethod = createSelector(selectAll, methods => methods.find(m => m.isDefault) ?? null);
export const selectPaymentMethodsPage = createSelector(selectPaymentMethodsState, s => s.page);
export const selectPaymentMethodsTotalCount = createSelector(selectPaymentMethodsState, s => s.totalCount);
export const selectPaymentMethodsTotalPages = createSelector(
  selectPaymentMethodsTotalCount,
  createSelector(selectPaymentMethodsState, s => s.pageSize),
  (total, size) => Math.ceil(total / size)
);
```

### 13.5 Routes

```typescript
// features/payment-methods/payment-methods.routes.ts
export const PAYMENT_METHODS_ROUTES: Routes = [
  {
    path: '',
    providers: [
      provideState({ name: 'paymentMethods', reducer: paymentMethodsReducer }),
      provideEffects([PaymentMethodsEffects]),
    ],
    children: [
      { path: '', loadComponent: () => import('./payment-methods-list/payment-methods-list.component') },
      { path: 'add', loadComponent: () => import('./add-payment-method/add-payment-method.component') },
    ],
  },
];
```
