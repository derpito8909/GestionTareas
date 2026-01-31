import { Component, input, output, signal } from '@angular/core';
import { CommonModule } from '@angular/common';

import { TaskResponse, TaskStatus } from '../../domain/task.model';
import { UserResponse } from '../../../users/domain/user.model';

@Component({
  standalone: true,
  selector: 'app-task-table',
  imports: [CommonModule],
  templateUrl: './task-table.component.html',
})
export class TaskTableComponent {
  tasks = input<TaskResponse[]>([]);
  users = input<UserResponse[]>([]);

  changeStatus = output<{ taskId: number; status: TaskStatus }>();
  assign = output<{ taskId: number; userId: number }>();

  private readonly selection = signal<Record<number, string>>({});

  selectedUserIdFor(taskId: number): string {
    return this.selection()[taskId] ?? '';
  }

  onSelectUser(taskId: number, userId: string) {
    this.selection.update(prev => ({ ...prev, [taskId]: userId }));
  }

  emitAssign(taskId: number) {
    const userId = Number(this.selectedUserIdFor(taskId));
    if (!userId) return;

    this.assign.emit({ taskId, userId });

    this.selection.update(prev => ({ ...prev, [taskId]: '' }));
  }
}

