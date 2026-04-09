import { Component, Input } from '@angular/core';
import { LucideAngularModule } from 'lucide-angular';

@Component({
  selector: 'app-empty-state',
  standalone: true,
  imports: [LucideAngularModule],
  template: `
    <div class="empty-state">
      <div class="empty-state__icon">
        <lucide-icon [name]="getLucideIcon(icon)" [strokeWidth]="1.5" [size]="48"></lucide-icon>
      </div>
      <h3 class="empty-state__title">{{ title }}</h3>
      @if (message) {
        <p class="empty-state__message">{{ message }}</p>
      }
      <div class="empty-state__action">
        <ng-content />
      </div>
    </div>
  `,
  styleUrl: './empty-state.component.css',
})
export class EmptyStateComponent {
  @Input() title = 'Nothing here yet';
  @Input() message = '';
  @Input() icon: 'cards' | 'bills' | 'notifications' | 'wallet' | 'payments' | 'default' = 'default';

  getLucideIcon(iconState: string): string {
    switch (iconState) {
      case 'cards': return 'credit-card';
      case 'bills': return 'receipt';
      case 'notifications': return 'bell';
      case 'wallet': return 'wallet';
      case 'payments': return 'arrow-right-left';
      default: return 'inbox';
    }
  }
}
