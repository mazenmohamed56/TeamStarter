import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { apiBaseUrlInterceptor } from './core/api';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    // TODO (Week 3): add an auth interceptor that attaches `Authorization: Bearer <token>`
    provideHttpClient(withInterceptors([apiBaseUrlInterceptor])),
  ],
};
