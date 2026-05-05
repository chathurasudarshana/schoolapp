import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { HTTP_INTERCEPTORS, HttpClient, HttpErrorResponse, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { Router } from '@angular/router';
import { ServerErrorInterceptor } from './server-error.interceptor';

describe('ServerErrorInterceptor', () => {
  let httpClient: HttpClient;
  let httpTestingController: HttpTestingController;
  let router: jasmine.SpyObj<Router>;

  beforeEach(() => {
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptorsFromDi()),
        provideHttpClientTesting(),
        {
          provide: HTTP_INTERCEPTORS,
          useClass: ServerErrorInterceptor,
          multi: true
        },
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

  it('should navigate to servererror page on HTTP 500 error', () => {
    const testUrl = '/api/test';

    httpClient.get(testUrl).subscribe({
      next: () => fail('should have failed with 500 error'),
      error: (error: HttpErrorResponse) => {
        expect(error.status).toBe(500);
        expect(router.navigate).toHaveBeenCalledWith(['/servererror']);
      }
    });

    const req = httpTestingController.expectOne(testUrl);
    req.flush('Server Error', { status: 500, statusText: 'Internal Server Error' });
  });

  it('should not navigate on non-500 errors', () => {
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
