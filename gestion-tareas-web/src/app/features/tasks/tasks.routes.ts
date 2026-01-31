import { Routes } from '@angular/router';
import { TasksPageComponent } from './ui/tasks-page.component/tasks-page.component';
import { TaskCreateComponent } from './ui/task-create.component/task-create.component';

export const TASKS_ROUTES: Routes = [
  { path: '', component: TasksPageComponent },
  { path: 'create', component: TaskCreateComponent },
];
