import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogModule, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'sch-confirm-dialog',
  imports: [CommonModule, MatDialogModule, MatButtonModule],
  templateUrl: './confirm-dialog.html',
  styleUrl: './confirm-dialog.scss',
})
export class ConfirmDialog {
  protected readonly message: string;
  protected readonly cancelText: string;
  protected readonly confirmText: string;

  constructor(
    @Inject(MAT_DIALOG_DATA)
    private readonly data: {
      message?: string;
      cancelText?: string;
      confirmText?: string;
    }
  ) {
    this.message = data?.message ?? 'Are you sure?';
    this.cancelText = data?.cancelText ?? 'No';
    this.confirmText = data?.confirmText ?? 'Yes';
  }
}
