import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';

import { TaskQuery } from '../domain/task-query.model';
import {
  AssignTaskRequest,
  ChangeTaskStatusRequest,
  CreateTaskRequest,
  TaskResponse
} from '../domain/task.model';

@Injectable({ providedIn: 'root' })
export class TasksApiService {
  private readonly http = inject(HttpClient);

  create(req: CreateTaskRequest) {
    return this.http.post<TaskResponse>('/api/tasks', req);
  }

  list(query: TaskQuery) {
    let params = new HttpParams();

    if (query.userId) params = params.set('userId', String(query.userId));
    if (query.status) params = params.set('status', query.status);

    if (query.priority) params = params.set('priority', query.priority);
    if (query.tag) params = params.set('tag', query.tag);
    if (query.dueDateFrom) params = params.set('dueDateFrom', query.dueDateFrom);
    if (query.dueDateTo) params = params.set('dueDateTo', query.dueDateTo);

    return this.http.get<TaskResponse[]>('/api/tasks', { params });
  }

  assign(taskId: number, req: AssignTaskRequest) {
    return this.http.put<TaskResponse>(`/api/tasks/${taskId}/assign`, req);
  }

  changeStatus(taskId: number, req: ChangeTaskStatusRequest) {
    return this.http.put<TaskResponse>(`/api/tasks/${taskId}/status`, req);
  }
}
