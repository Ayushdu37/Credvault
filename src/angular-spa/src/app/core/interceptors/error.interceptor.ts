import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { Store } from '@ngrx/store';
import { Router } from '@angular/router';
import { ToastService } from '../services/toast.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const store = inject(Store);
  const router = inject(Router);
  const toast = inject(ToastService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      switch (error.status) {
        case 401:
          // Token expired or invalid — redirect to login
          // (In Phase 3 you'll dispatch AuthActions.tokenExpired here)
          router.navigateByUrl('/login');
          break;

        case 403:
          toast.show('You don\'t have permission to perform this action.', 'error');
          break;

        case 0:
          toast.show('Network error — is the backend running?', 'error');
          break;

        default:
          if (error.status >= 500) {
            toast.show('Server error. Please try again later.', 'error');
          }
          break;
      }

      return throwError(() => error);
    })
  );
};
