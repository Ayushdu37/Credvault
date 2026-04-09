import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-button',
  standalone: true,
  template: `
    <button
      class="btn"
      [class.btn--primary]="variant === 'primary'"
      [class.btn--secondary]="variant === 'secondary'"
      [class.btn--danger]="variant === 'danger'"
      [class.btn--ghost]="variant === 'ghost'"
      [class.btn--sm]="size === 'sm'"
      [class.btn--lg]="size === 'lg'"
      [class.btn--loading]="loading"
      [class.btn--full]="fullWidth"
      [disabled]="disabled || loading"
      [type]="type"
      [attr.id]="btnId">
      @if (loading) {
        <span class="btn__spinner"></span>
      }
      <ng-content />
    </button>
  `,
  styleUrl: './button.component.css',
})
export class ButtonComponent {
  @Input() variant: 'primary' | 'secondary' | 'danger' | 'ghost' = 'primary';
  @Input() size: 'sm' | 'md' | 'lg' = 'md';
  @Input() type: 'button' | 'submit' | 'reset' = 'button';
  @Input() loading = false;
  @Input() disabled = false;
  @Input() fullWidth = false;
  @Input() btnId: string | null = null;
}
