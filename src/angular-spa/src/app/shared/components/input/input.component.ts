import { Component, forwardRef, Input } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, FormsModule } from '@angular/forms';
import { LucideAngularModule } from 'lucide-angular';

@Component({
  selector: 'app-input',
  standalone: true,
  imports: [FormsModule, LucideAngularModule],
  template: `
    <div class="input-group">
      @if (label) {
        <label [for]="inputId" class="input-label">{{ label }}</label>
      }
      <div class="input-wrapper" [class.input-wrapper--error]="error">
        @if (icon) {
          <div class="input-icon">
            <lucide-icon [name]="icon" [size]="18" [strokeWidth]="2"></lucide-icon>
          </div>
        }
        <input
          [id]="inputId"
          [type]="type"
          [placeholder]="placeholder"
          [disabled]="disabled"
          [(ngModel)]="value"
          (blur)="onTouched()"
          class="input-field"
          [class.input-field--with-icon]="icon" />
      </div>
      @if (error) {
        <span class="input-error">{{ error }}</span>
      }
    </div>
  `,
  styleUrl: './input.component.css',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => InputComponent),
      multi: true,
    },
  ],
})
export class InputComponent implements ControlValueAccessor {
  @Input() label = '';
  @Input() type = 'text';
  @Input() placeholder = '';
  @Input() icon = '';
  @Input() error = '';
  @Input() inputId = `input-${Math.random().toString(36).substring(2, 9)}`;

  private innerValue: string = '';
  disabled = false;

  onChange: any = () => {};
  onTouched: any = () => {};

  get value(): string {
    return this.innerValue;
  }

  set value(v: string) {
    if (v !== this.innerValue) {
      this.innerValue = v;
      this.onChange(v);
    }
  }

  writeValue(value: any): void {
    if (value !== undefined) {
      this.innerValue = value;
    }
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }
}
