import { Injectable } from '@angular/core';
import { HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';
import { BillResponse, PaymentScheduleResponse, SchedulePaymentRequest } from '../../../core/models/billing.model';
import { PaginatedResponse } from '../../../core/models/api-response.model';

@Injectable({ providedIn: 'root' })
export class BillingService {

  constructor(private api: ApiService) { }

  getBills(page: number = 1, pageSize: number = 10): Observable<PaginatedResponse<BillResponse>> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    return this.api.get<PaginatedResponse<BillResponse>>('/api/bills', params)
      .pipe(
        map(res => res.data!)
      );
  }

  getBillById(id: string): Observable<BillResponse> {
    return this.api.get<BillResponse>(`/api/bills/${id}`)
      .pipe(
        map(res => res.data!)
      );
  }

  getBillsByCard(cardId: string): Observable<BillResponse[]> {
    return this.api.get<BillResponse[]>(`/api/bills/card/${cardId}`)
      .pipe(
        map(res => res.data!)
      );
  }

  schedulePayment(billId: string, payload: SchedulePaymentRequest): Observable<PaymentScheduleResponse> {
    return this.api.post<PaymentScheduleResponse>(`/api/bills/${billId}/schedule`, payload)
      .pipe(
        map(res => res.data!)
      );
  }

  cancelScheduledPayment(scheduleId: string): Observable<void> {
    return this.api.delete<void>(`/api/bills/schedule/${scheduleId}`)
      .pipe(
        map(res => res.data!)
      );
  }
}
