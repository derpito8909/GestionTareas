export type TaskStatus = 'Pending' | 'InProgress' | 'Done';

export interface CreateTaskRequest {
  title: string;
  description?: string | null;
  assignedUserId: number;
  additionalInfoJson?: string | null;
}

export interface AssignTaskRequest {
  userId: number;
}

export interface ChangeTaskStatusRequest {
  status: TaskStatus;
}

export interface TaskResponse {
  id: number;
  title: string;
  description?: string | null;
  status: TaskStatus;
  createdAt: string;
  assignedUserId: number;
  assignedUserName: string;
  additionalInfoJson?: string | null;
}
