import { Provider } from '@angular/core';
import { APP_CONFIG } from '../injection-tokens/app-config.token';
import { AppConfig } from '../interfaces/app-config';

export function provideEnvironment(config: AppConfig): Provider {
  return {
    provide: APP_CONFIG,
    useValue: config
  };
}
