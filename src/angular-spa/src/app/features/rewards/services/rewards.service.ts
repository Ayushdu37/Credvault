import { Injectable } from '@angular/core';
import { HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';
import {
  RewardAccountResponse,
  RewardTransactionResponse,
  RedeemRewardsRequest,
} from '../../../core/models/billing.model';
import { PaginatedResponse } from '../../../core/models/api-response.model';

@Injectable({ providedIn: 'root' })
export class RewardsService {

  constructor(private api: ApiService) { }

  getRewardAccount(): Observable<RewardAccountResponse> {
    return this.api.get<RewardAccountResponse>('/api/rewards')
      .pipe(
        map(res => res.data!)
      );
  }

  getTransactions(page: number = 1, pageSize: number = 10): Observable<PaginatedResponse<RewardTransactionResponse>> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    return this.api.get<PaginatedResponse<RewardTransactionResponse>>('/api/rewards/transactions', params)
      .pipe(
        map(res => res.data!)
      );
  }

  redeemPoints(payload: RedeemRewardsRequest): Observable<RewardAccountResponse> {
    return this.api.post<RewardAccountResponse>('/api/rewards/redeem', payload)
      .pipe(
        map(res => res.data!)
      );
  }
}
