import { Injectable } from '@angular/core';
import { HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';
import { CardResponse, CardSummaryResponse, AddCardRequest, UpdateCardLimitRequest } from '../../../core/models/card.model';
import { PaginatedResponse } from '../../../core/models/api-response.model';

@Injectable({ providedIn: 'root' })
export class CardsService {

  constructor(private api: ApiService) { }

  getCards(page: number = 1, pageSize: number = 10): Observable<PaginatedResponse<CardResponse>> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    return this.api.get<PaginatedResponse<CardResponse>>('/api/cards', params)
      .pipe(
        map(res => res.data!)
      );
  }

  getCardById(id: string): Observable<CardResponse> {
    return this.api.get<CardResponse>(`/api/cards/${id}`)
      .pipe(
        map(res => res.data!)
      );
  }

  getCardUtilization(): Observable<CardSummaryResponse> {
    return this.api.get<CardSummaryResponse>('/api/cards/utilization')
      .pipe(
        map(res => res.data!)
      );
  }

  addCard(payload: AddCardRequest): Observable<CardResponse> {
    return this.api.post<CardResponse>('/api/cards', payload)
      .pipe(
        map(res => res.data!)
      );
  }

  setDefaultCard(id: string): Observable<void> {
    return this.api.put<void>(`/api/cards/${id}/default`, {})
      .pipe(
        map(res => res.data!)
      );
  }

  verifyCard(id: string): Observable<void> {
    return this.api.put<void>(`/api/cards/${id}/verify`, {})
      .pipe(
        map(res => res.data!)
      );
  }

  updateCardLimit(id: string, payload: UpdateCardLimitRequest): Observable<void> {
    return this.api.put<void>(`/api/cards/${id}/limit`, payload)
      .pipe(
        map(res => res.data!)
      );
  }

  deleteCard(id: string): Observable<void> {
    return this.api.delete<void>(`/api/cards/${id}`)
      .pipe(
        map(res => res.data!)
      );
  }
}
