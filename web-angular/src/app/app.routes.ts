import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'tasks' },
  { path: 'tasks', loadComponent: () => import('./features/tasks/tasks.page').then((m) => m.TasksPage) },
  // TODO (Week 3): 'login' route + canActivate guard for authenticated routes
];
