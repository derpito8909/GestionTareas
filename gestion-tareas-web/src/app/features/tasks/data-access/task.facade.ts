import { inject, Injectable, signal } from '@angular/core';
import { TasksApiService } from './task-api.service';
import { TaskQuery } from '../domain/task-query.model';
import { TaskResponse, TaskStatus, CreateTaskRequest } from '../domain/task.model';
import { ApiErrorResponse } from '../../../core/models/api-error.model';
import { finalize } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class TasksFacade {
  private readonly api = inject(TasksApiService);

  readonly tasks = signal<TaskResponse[]>([]);
  readonly loading = signal(false);
  readonly error = signal<ApiErrorResponse | null>(null);

  load(query: TaskQuery) {
    this.loading.set(true);
    this.error.set(null);

    this.api.list(query)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: data => this.tasks.set(data),
        error: e => this.error.set(e),
      });
  }

  create(req: CreateTaskRequest, after?: () => void) {
    this.loading.set(true);
    this.error.set(null);

    this.api.create(req)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: _ => after?.(),
        error: e => this.error.set(e),
      });
  }

  changeStatus(taskId: number, status: TaskStatus) {
    this.loading.set(true);
    this.error.set(null);

    this.api.changeStatus(taskId, { status })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: updated => {
          this.tasks.set(this.tasks().map(t => (t.id === updated.id ? updated : t)));
        },
        error: e => this.error.set(e),
      });
  }

  assign(taskId: number, userId: number) {
    this.loading.set(true);
    this.error.set(null);

    this.api.assign(taskId, { userId })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: updated => {
          this.tasks.set(this.tasks().map(t => (t.id === updated.id ? updated : t)));
        },
        error: e => this.error.set(e),
      });
  }
}
