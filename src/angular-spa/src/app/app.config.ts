import { ApplicationConfig, isDevMode, importProvidersFrom, LOCALE_ID, DEFAULT_CURRENCY_CODE } from '@angular/core';
import { registerLocaleData } from '@angular/common';
import localeIn from '@angular/common/locales/en-IN';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';

registerLocaleData(localeIn);
import { provideStore, provideState } from '@ngrx/store';
import { provideEffects } from '@ngrx/effects';
import { provideStoreDevtools } from '@ngrx/store-devtools';
import { routes } from './app.routes';
import { authReducer } from './store/auth/auth.reducer';
import * as authEffects from './store/auth/auth.effects';
import { dashboardReducer } from './store/dashboard/dashboard.reducer';
import * as dashboardEffects from './store/dashboard/dashboard.effects';
import { cardsReducer } from './store/cards/cards.reducer';
import * as cardsEffects from './store/cards/cards.effects';
import { billingReducer } from './store/billing/billing.reducer';
import * as billingEffects from './store/billing/billing.effects';
import { paymentsReducer } from './store/payments/payments.reducer';
import * as paymentsEffects from './store/payments/payments.effects';
import { notificationsReducer } from './store/notifications/notifications.reducer';
import * as notificationsEffects from './store/notifications/notifications.effects';
import { paymentMethodsReducer } from './store/payment-methods/payment-methods.reducer';
import * as paymentMethodsEffects from './store/payment-methods/payment-methods.effects';
import { rewardsReducer } from './store/rewards/rewards.reducer';
import * as rewardsEffects from './store/rewards/rewards.effects';
import { authInterceptor } from './core/interceptors/auth.interceptor';
import { errorInterceptor } from './core/interceptors/error.interceptor';
import {
  LucideAngularModule,
  LayoutDashboard, CreditCard, Receipt, ArrowRightLeft, Wallet,
  Bell, ChevronLeft, Menu, User, LogOut, Inbox,
  Lock, LockOpen, ArrowRight, ArrowLeft,
  FileText, AlertCircle, Monitor, Tv, Coffee, ShoppingBag, Plane,
  Landmark, Smartphone, Send, CheckCircle, ShieldCheck, Clock,
  Info, TrendingUp, ShieldAlert, Calendar, Crown, Mail, Check, Trash2,
  Star, History, Percent, Box, Shield, Loader2, Sun, Moon, Zap, Droplet, Home, Copy,
  HandMetal, Download,
} from 'lucide-angular';

export const appConfig: ApplicationConfig = {
  providers: [
    importProvidersFrom(LucideAngularModule.pick({
      LayoutDashboard, CreditCard, Receipt, ArrowRightLeft, Wallet,
      Bell, ChevronLeft, Menu, User, LogOut, Inbox,
      Lock, LockOpen, ArrowRight, ArrowLeft,
      FileText, AlertCircle, Monitor, Tv, Coffee, ShoppingBag, Plane,
      Landmark, Smartphone, Send, CheckCircle, ShieldCheck, Clock,
      Info, TrendingUp, ShieldAlert, Calendar, Crown, Mail, Check, Trash2,
      Star, History, Percent, Box, Shield, Loader2, Sun, Moon, Zap, Droplet, Home, Copy,
      HandMetal, Download,
    })),
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor, errorInterceptor])),
    provideStore({ auth: authReducer }),
    provideState('dashboard', dashboardReducer),
    provideState('cards', cardsReducer),
    provideState('billing', billingReducer),
    provideState('payments', paymentsReducer),
    provideState('notifications', notificationsReducer),
    provideState('paymentMethods', paymentMethodsReducer),
    provideState('rewards', rewardsReducer),
    provideEffects(authEffects, dashboardEffects, cardsEffects, billingEffects, paymentsEffects, notificationsEffects, paymentMethodsEffects, rewardsEffects),
    provideStoreDevtools({ maxAge: 25, logOnly: isDevMode() }),
    { provide: LOCALE_ID, useValue: 'en-IN' },
    { provide: DEFAULT_CURRENCY_CODE, useValue: 'INR' },
  ],
};
