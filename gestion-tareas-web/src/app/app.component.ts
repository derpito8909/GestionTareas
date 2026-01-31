import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from './shared/ui/navbar.component';

@Component({
  standalone: true,
  selector: 'app-root',
  imports: [NavbarComponent, RouterOutlet],
  template: `
    <div style="max-width: 980px; margin: 0 auto; padding: 0 12px;">
      <app-navbar />
      <router-outlet />
    </div>
  `
})
export class AppComponent {
  title = 'gestion-tareas-web';
}
