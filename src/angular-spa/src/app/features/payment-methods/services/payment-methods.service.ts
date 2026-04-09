import { Injectable } from '@angular/core';
import { HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';
import { PaymentMethodResponse, AddPaymentMethodRequest } from '../../../core/models/payment.model';
import { PaginatedResponse } from '../../../core/models/api-response.model';

@Injectable({ providedIn: 'root' })
export class PaymentMethodsService {

  constructor(private api: ApiService) { }

  getPaymentMethods(page: number = 1, pageSize: number = 10): Observable<PaginatedResponse<PaymentMethodResponse>> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    return this.api.get<PaginatedResponse<PaymentMethodResponse>>('/api/payment-methods', params)
      .pipe(
        map(res => res.data!)
      );
  }

  addPaymentMethod(payload: AddPaymentMethodRequest): Observable<PaymentMethodResponse> {
    return this.api.post<PaymentMethodResponse>('/api/payment-methods', payload)
      .pipe(
        map(res => res.data!)
      );
  }

  deletePaymentMethod(id: string): Observable<void> {
    return this.api.delete<void>(`/api/payment-methods/${id}`)
      .pipe(
        map(res => res.data!)
      );
  }
}
