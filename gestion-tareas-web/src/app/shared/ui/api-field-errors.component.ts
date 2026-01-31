import { Component, input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiFieldError } from '../../core/models/api-error.model';

@Component({
  standalone: true,
  selector: 'app-api-field-errors',
  imports: [CommonModule],
  template: `
    @if (errors().length > 0) {
      <ul style="margin-top:10px;">
        @for (err of errors(); track err.field + err.code) {
          <li style="color:#b00020;">
            <b>{{ err.field }}</b>: {{ err.message }}
          </li>
        }
      </ul>
    }
  `
})
export class ApiFieldErrorsComponent {
  errors = input<ApiFieldError[]>([]);
}
