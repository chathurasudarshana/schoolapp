import { HTTP_INTERCEPTORS_PROVIDERS, ServerErrorInterceptor } from './index';
import { HTTP_INTERCEPTORS } from '@angular/common/http';

describe('Interceptors Index', () => {
  it('should export HTTP_INTERCEPTORS_PROVIDERS array', () => {
    expect(HTTP_INTERCEPTORS_PROVIDERS).toBeDefined();
    expect(Array.isArray(HTTP_INTERCEPTORS_PROVIDERS)).toBe(true);
    expect(HTTP_INTERCEPTORS_PROVIDERS.length).toBeGreaterThan(0);
  });

  it('should include ServerErrorInterceptor in providers', () => {
    const serverErrorProvider = HTTP_INTERCEPTORS_PROVIDERS.find(
      provider => provider.useClass === ServerErrorInterceptor
    );
    
    expect(serverErrorProvider).toBeDefined();
    expect(serverErrorProvider?.provide).toBe(HTTP_INTERCEPTORS);
    expect(serverErrorProvider?.multi).toBe(true);
  });

  it('should export ServerErrorInterceptor', () => {
    expect(ServerErrorInterceptor).toBeDefined();
    expect(typeof ServerErrorInterceptor).toBe('function');
  });
});
