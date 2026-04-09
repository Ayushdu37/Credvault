import { Injectable } from '@angular/core';
import { HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';
import {
  NotificationResponse,
  NotificationPreferenceResponse,
  UnreadCountResponse,
  UpdatePreferencesRequest,
} from '../../../core/models/notification.model';
import { PaginatedResponse } from '../../../core/models/api-response.model';

@Injectable({ providedIn: 'root' })
export class NotificationsService {

  constructor(private api: ApiService) { }

  getNotifications(page: number = 1, pageSize: number = 10): Observable<PaginatedResponse<NotificationResponse>> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    return this.api.get<PaginatedResponse<NotificationResponse>>('/api/notifications', params)
      .pipe(
        map(res => res.data!)
      );
  }

  getUnreadCount(): Observable<number> {
    return this.api.get<UnreadCountResponse>('/api/notifications/unread-count')
      .pipe(
        map(res => res.data!.count)
      );
  }

  markAsRead(id: string): Observable<void> {
    return this.api.patch<void>(`/api/notifications/${id}/read`)
      .pipe(
        map(res => res.data!)
      );
  }

  markAllAsRead(): Observable<void> {
    return this.api.patch<void>('/api/notifications/read-all')
      .pipe(
        map(res => res.data!)
      );
  }

  getPreferences(): Observable<NotificationPreferenceResponse> {
    return this.api.get<NotificationPreferenceResponse>('/api/notifications/preferences')
      .pipe(
        map(res => res.data!)
      );
  }

  updatePreferences(payload: UpdatePreferencesRequest): Observable<NotificationPreferenceResponse> {
    return this.api.put<NotificationPreferenceResponse>('/api/notifications/preferences', payload)
      .pipe(
        map(res => res.data!)
      );
  }
}
