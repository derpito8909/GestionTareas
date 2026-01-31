import { TaskStatus } from './task.model';

export interface TaskQuery {
  userId?: number | null;
  status?: TaskStatus | null;


  priority?: string | null;
  tag?: string | null;
  dueDateFrom?: string | null;
  dueDateTo?: string | null;
}
