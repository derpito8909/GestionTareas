import { Component, input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiErrorResponse } from '../../core/models/api-error.model';
import { ApiFieldErrorsComponent } from './api-field-errors.component';

@Component({
  standalone: true,
  selector: 'app-api-error-banner',
  imports: [CommonModule, ApiFieldErrorsComponent],
  template: `
    @if (error(); as e) {
      <div style="border:1px solid #b00020; padding:12px; border-radius:8px;">
        <div style="color:#b00020; font-weight:600;">
          {{ e.message }}
        </div>

        @if (e.traceId) {
          <small style="opacity:.75;">TraceId: {{ e.traceId }}</small>
        }

        <app-api-field-errors [errors]="e.errors ?? []" />
      </div>
    }
  `
})
export class ApiErrorBannerComponent {
  error = input<ApiErrorResponse | null>(null);
}
