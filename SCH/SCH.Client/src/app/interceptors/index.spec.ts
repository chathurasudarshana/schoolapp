import { httpInterceptors, serverErrorInterceptor, unauthorizedInterceptor } from './index';

describe('Interceptors Index', () => {
  it('should export httpInterceptors array', () => {
    expect(httpInterceptors).toBeDefined();
    expect(Array.isArray(httpInterceptors)).toBe(true);
    expect(httpInterceptors.length).toBeGreaterThan(0);
  });

  it('should include serverErrorInterceptor in the array', () => {
    expect(httpInterceptors).toContain(serverErrorInterceptor);
  });

  it('should include unauthorizedInterceptor in the array', () => {
    expect(httpInterceptors).toContain(unauthorizedInterceptor);
  });

  it('should export interceptors as functions', () => {
    expect(typeof serverErrorInterceptor).toBe('function');
    expect(typeof unauthorizedInterceptor).toBe('function');
  });
});

