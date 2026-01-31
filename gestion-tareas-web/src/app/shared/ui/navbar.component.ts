import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-navbar',
  imports: [RouterLink, RouterLinkActive],
  template: `
    <nav style="display:flex; gap:12px; padding:12px 0; border-bottom:1px solid #ddd; margin-bottom:16px;">
      <a routerLink="/tasks" routerLinkActive="active" [routerLinkActiveOptions]="{ exact: true }">Tareas</a>
      <a routerLink="/tasks/create" routerLinkActive="active">Crear tarea</a>
      <a routerLink="/users" routerLinkActive="active">Usuarios</a>
    </nav>
  `,
  styles: [`
    a { text-decoration: none; padding: 6px 10px; border-radius: 8px; }
    a.active { font-weight: 700; border: 1px solid #333; }
  `]
})
export class NavbarComponent {}
