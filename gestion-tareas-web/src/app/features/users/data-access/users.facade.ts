import { inject, Injectable, signal } from '@angular/core';
import { UsersApiService } from './users-api.service';
import { UserResponse } from '../domain/user.model';
import { CreateUserRequest } from '../domain/user.model';
import { ApiErrorResponse } from '../../../core/models/api-error.model';
import { finalize } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class UsersFacade {
  private readonly api = inject(UsersApiService);

  readonly users = signal<UserResponse[]>([]);
  readonly loading = signal(false);
  readonly error = signal<ApiErrorResponse | null>(null);

  load() {
    this.loading.set(true);
    this.error.set(null);

    this.api.list()
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: data => this.users.set(data),
        error: e => this.error.set(e),
      });
  }

  create(req: CreateUserRequest, after?: () => void) {
    this.loading.set(true);
    this.error.set(null);

    this.api.create(req)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: _ => after?.(),
        error: e => this.error.set(e),
      });
  }
}
