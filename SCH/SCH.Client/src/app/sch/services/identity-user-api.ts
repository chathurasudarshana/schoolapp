import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { AppConfig } from '../../interfaces/app-config';
import { APP_CONFIG } from '../../injection-tokens/app-config.token';
import { Observable } from 'rxjs';
import { UserLookup } from '../interfaces/user-lookup';

@Injectable()
export class IdentityUserApi {

  private readonly apiUrl: string;

  constructor(
    @Inject(APP_CONFIG) private readonly appConfig: AppConfig,
    private readonly http: HttpClient
  ) {
    this.apiUrl = this.appConfig.apiUrl;
  }

  /**
   * Returns Basic-role users not yet linked to a Student or Teacher record.
   * Only accessible to Admin users (enforced on backend).
   */
  public getBasicOnlyUsers(): Observable<UserLookup[]> {
    return this.http.get<UserLookup[]>(
      `${this.apiUrl}/identityusers/basic-only`
    );
  }
}
