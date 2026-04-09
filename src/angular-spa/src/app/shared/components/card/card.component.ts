import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-card',
  standalone: true,
  template: `
    <div class="card" [class.card--hoverable]="hoverable" [class.card--compact]="compact">
      @if (title || headerTpl) {
        <div class="card__header">
          @if (title) {
            <h3 class="card__title">{{ title }}</h3>
          }
          @if (subtitle) {
            <p class="card__subtitle">{{ subtitle }}</p>
          }
          <ng-content select="[card-header-action]" />
        </div>
      }
      <div class="card__body" [class.card__body--no-padding]="noPadding">
        <ng-content />
      </div>
      <ng-content select="[card-footer]" />
    </div>
  `,
  styleUrl: './card.component.css',
})
export class CardComponent {
  @Input() title = '';
  @Input() subtitle = '';
  @Input() hoverable = false;
  @Input() compact = false;
  @Input() noPadding = false;
  headerTpl: any;
}
