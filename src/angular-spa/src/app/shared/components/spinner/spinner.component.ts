import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-spinner',
  standalone: true,
  template: `
    <div class="spinner-container" [class.spinner-container--inline]="inline">
      <div class="spinner"
           [class.spinner--sm]="size === 'sm'"
           [class.spinner--lg]="size === 'lg'">
      </div>
      @if (text) {
        <span class="spinner-text">{{ text }}</span>
      }
    </div>
  `,
  styleUrl: './spinner.component.css',
})
export class SpinnerComponent {
  @Input() size: 'sm' | 'md' | 'lg' = 'md';
  @Input() text = '';
  @Input() inline = false;
}
