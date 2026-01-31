import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';

import { appRoutes } from './app.routes';
import { API_BASE_URL } from './core/config/app.tokens';
import { apiBaseUrlInterceptor } from './core/http/api-base-url.interceptor';
import { errorInterceptor } from './core/http/error.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(appRoutes),

    { provide: API_BASE_URL, useValue: '' },

    provideHttpClient(
      withInterceptors([apiBaseUrlInterceptor, errorInterceptor])
    ),
  ],
};
