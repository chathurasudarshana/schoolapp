import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { APP_CONFIG } from '../../injection-tokens/app-config.token';

import { ImageApi } from './image-api';

describe('ImageApi', () => {
  let service: ImageApi;
  let httpMock: HttpTestingController;
  const apiUrl = 'http://test-api';

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        ImageApi,
        { provide: APP_CONFIG, useValue: { apiUrl } },
      ],
    });
    service = TestBed.inject(ImageApi);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('uploadStudentProfile POSTs form data', () => {
    const file = new File([new Blob(['a'])], 'a.jpg', { type: 'image/jpeg' });
    service.uploadStudentProfile(file).subscribe();
    const req = httpMock.expectOne(`${apiUrl}/Image/uploadStudentProfile`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body instanceof FormData).toBeTrue();
    req.flush({ filename: 'a.jpg' });
  });

  it('deleteStudentProfile DELETEs by filename', () => {
    service.deleteStudentProfile('a.jpg').subscribe();
    const req = httpMock.expectOne(`${apiUrl}/Image/deleteStudentProfile/a.jpg`);
    expect(req.request.method).toBe('DELETE');
    req.flush({});
  });
});
