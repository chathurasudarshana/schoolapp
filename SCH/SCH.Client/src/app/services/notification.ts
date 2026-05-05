import { Injectable } from '@angular/core';
import { ToastrService, IndividualConfig } from 'ngx-toastr';

@Injectable({
  providedIn: 'root',
})
export class Notification {
  private readonly defaultConfig: Partial<IndividualConfig> = {
    timeOut: 3000,
    positionClass: 'toast-top-right',
    closeButton: true,
    progressBar: true,
  };

  constructor(private readonly toastr: ToastrService) {}

  success(message: string, title: string = 'Success') {
    this.toastr.success(message, title, this.defaultConfig);
  }

  error(message: string, title: string = 'Error') {
    this.toastr.error(message, title, this.defaultConfig);
  }

  warning(message: string, title: string = 'Warning') {
    this.toastr.warning(message, title, this.defaultConfig);
  }

  info(message: string, title: string = 'Info') {
    this.toastr.info(message, title, this.defaultConfig);
  }
}
