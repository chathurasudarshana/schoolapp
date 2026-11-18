import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { AuthApi } from './auth-api';
import { APP_CONFIG } from '../injection-tokens/app-config.token';

describe('AuthApi', () => {
  let service: AuthApi;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        AuthApi,
        { provide: APP_CONFIG, useValue: { apiUrl: '' } },
      ],
    });
    service = TestBed.inject(AuthApi);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

