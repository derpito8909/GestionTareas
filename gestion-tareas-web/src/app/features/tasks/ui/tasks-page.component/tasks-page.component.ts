import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NonNullableFormBuilder, ReactiveFormsModule } from '@angular/forms';

import { TasksFacade } from '../../data-access/task.facade';
import { UsersFacade } from '../../../users/data-access/users.facade';
import { TaskStatus } from '../../domain/task.model';

import { ApiErrorBannerComponent } from '../../../../shared/ui/api-error-banner.component';
import { TaskTableComponent } from '../task-table.component/task-table.component';

@Component({
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ApiErrorBannerComponent, TaskTableComponent],
  templateUrl: './tasks-page.component.html',
})
export class TasksPageComponent {
  readonly tasksFacade = inject(TasksFacade);
  readonly usersFacade = inject(UsersFacade);
  private readonly fb = inject(NonNullableFormBuilder);

  filters = this.fb.group({
    status: ['' as '' | TaskStatus],
    userId: [''],
    priority: [''],
    tag: [''],
    dueDateFrom: [''],
    dueDateTo: [''],
  });

  ngOnInit() {
    this.usersFacade.load();
    this.load();
  }

  load() {
    const f = this.filters.getRawValue();

    this.tasksFacade.load({
      status: (f.status || null),
      userId: f.userId ? Number(f.userId) : null,
      priority: f.priority || null,
      tag: f.tag || null,
      dueDateFrom: f.dueDateFrom || null,
      dueDateTo: f.dueDateTo || null,
    });
  }

  reset() {
    this.filters.reset({
      status: '',
      userId: '',
      priority: '',
      tag: '',
      dueDateFrom: '',
      dueDateTo: '',
    });
    this.load();
  }

  onChangeStatus(taskId: number, status: TaskStatus) {
    this.tasksFacade.changeStatus(taskId, status);
  }

  onAssign(taskId: number, userId: number) {
    this.tasksFacade.assign(taskId, userId);
  }
}
