import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { HttpClient, HttpErrorResponse, provideHttpClient, withInterceptors } from '@angular/common/http';
import { Router } from '@angular/router';
import { unauthorizedInterceptor } from './unauthorized.interceptor';

describe('UnauthorizedInterceptor', () => {
  let httpClient: HttpClient;
  let httpTestingController: HttpTestingController;
  let router: jasmine.SpyObj<Router>;

  beforeEach(() => {
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptors([unauthorizedInterceptor])),
        provideHttpClientTesting(),
        { provide: Router, useValue: routerSpy }
      ]
    });

    httpClient = TestBed.inject(HttpClient);
    httpTestingController = TestBed.inject(HttpTestingController);
    router = TestBed.inject(Router) as jasmine.SpyObj<Router>;
  });

  afterEach(() => {
    httpTestingController.verify();
  });

  it('should navigate to login page on HTTP 401 error', () => {
    const testUrl = '/api/test';

    httpClient.get(testUrl).subscribe({
      next: () => fail('should have failed with 401 error'),
      error: (error: HttpErrorResponse) => {
        expect(error.status).toBe(401);
        expect(router.navigate).toHaveBeenCalledWith(['/login']);
      }
    });

    const req = httpTestingController.expectOne(testUrl);
    req.flush('Unauthorized', { status: 401, statusText: 'Unauthorized' });
  });

  it('should not navigate on non-401 errors', () => {
    const testUrl = '/api/test';

    httpClient.get(testUrl).subscribe({
      next: () => fail('should have failed with 404 error'),
      error: (error: HttpErrorResponse) => {
        expect(error.status).toBe(404);
        expect(router.navigate).not.toHaveBeenCalled();
      }
    });

    const req = httpTestingController.expectOne(testUrl);
    req.flush('Not Found', { status: 404, statusText: 'Not Found' });
  });

  it('should pass through successful requests', () => {
    const testUrl = '/api/test';
    const testData = { message: 'success' };

    httpClient.get(testUrl).subscribe(data => {
      expect(data).toEqual(testData);
      expect(router.navigate).not.toHaveBeenCalled();
    });

    const req = httpTestingController.expectOne(testUrl);
    req.flush(testData);
  });
});
