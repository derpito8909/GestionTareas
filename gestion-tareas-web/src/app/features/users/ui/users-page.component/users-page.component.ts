import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { UsersFacade } from '../../data-access/users.facade';
import { ApiErrorBannerComponent } from '../../../../shared/ui/api-error-banner.component';

@Component({
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ApiErrorBannerComponent],
  templateUrl: './users-page.component.html',
})
export class UsersPageComponent {
  readonly facade = inject(UsersFacade);
  private readonly fb = inject(NonNullableFormBuilder);

  form = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
    email: ['', [Validators.required, Validators.email, Validators.maxLength(255)]],
  });

  ngOnInit() {
    this.facade.load();
  }

  create() {
    const v = this.form.getRawValue();
    this.facade.create(v, () => this.form.reset());
  }
}
