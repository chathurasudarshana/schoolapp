import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { App } from './app/app';
import { provideEnvironment } from './app/providers/app-config.provider';

fetch('/config.json')
  .then((response) => {
    if (!response.ok) {
      throw new Error('Failed to load config.json');
    }
    return response.json();
  })
  .then((config) => {
    return bootstrapApplication(App, {
      ...appConfig,
      providers: [...(appConfig.providers || []), provideEnvironment(config)],
    });
  })
  .catch((err) => console.error('Bootstrap failed:', err));
