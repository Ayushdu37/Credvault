import { createActionGroup, emptyProps, props } from '@ngrx/store';
import { Notification, NotificationPreferenceResponse, UpdatePreferencesRequest } from '../../core/models/notification.model';

export const NotificationsActions = createActionGroup({
  source: 'Notifications',
  events: {
    'Load Notifications': props<{ page: number; pageSize: number }>(),
    'Load Notifications Success': props<{ notifications: Notification[]; totalCount: number }>(),
    'Load Notifications Failure': props<{ error: string }>(),

    'Mark As Read': props<{ id: string }>(),
    'Mark As Read Success': props<{ id: string }>(),

    'Mark All As Read': emptyProps(),
    'Mark All As Read Success': emptyProps(),

    'Load Unread Count': emptyProps(),
    'Load Unread Count Success': props<{ count: number }>(),

    'Load Preferences': emptyProps(),
    'Load Preferences Success': props<{ preferences: NotificationPreferenceResponse }>(),

    'Update Preferences': props<{ payload: UpdatePreferencesRequest }>(),
    'Update Preferences Success': props<{ preferences: NotificationPreferenceResponse }>(),
  },
});
