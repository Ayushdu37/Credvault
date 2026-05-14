import { Injectable } from '@angular/core';
import { HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';
import { 
  PaymentResponse, 
  MakePaymentRequest
} from '../../../core/models/payment.model';
import { PaginatedResponse, ApiResponse } from '../../../core/models/api-response.model';

@Injectable({ providedIn: 'root' })
export class PaymentsService {

  constructor(private api: ApiService) { }

  getPayments(page: number = 1, pageSize: number = 10): Observable<PaginatedResponse<PaymentResponse>> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    return this.api.get<PaginatedResponse<PaymentResponse>>('/api/payments', params)
      .pipe(
        map(res => res.data!)
      );
  }

  getPaymentById(id: string): Observable<PaymentResponse> {
    return this.api.get<PaymentResponse>(`/api/payments/${id}`)
      .pipe(
        map(res => res.data!)
      );
  }

  getPaymentsByBill(billId: string): Observable<PaymentResponse[]> {
    return this.api.get<PaymentResponse[]>(`/api/payments/bill/${billId}`)
      .pipe(
        map(res => res.data!)
      );
  }

  makePayment(payload: MakePaymentRequest): Observable<string> {
    return this.api.post<string>('/api/payments', payload)
      .pipe(
        map(res => res.data!)
      );
  }
}
