import { Component } from '@angular/core';
import { ICellRendererAngularComp } from 'ag-grid-angular';
import { ICellRendererParams } from 'ag-grid-community';
import { SecureImage } from '../../../../pipes/secure-image';
import { CommonModule } from '@angular/common';

/**
 * AG Grid cell renderer component for student profile photos
 * Uses SecureImage pipe to load images with JWT authentication
 */
@Component({
  selector: 'student-photo-cell',
  imports: [SecureImage, CommonModule],
  templateUrl: './student-photo-cell.html',
  styleUrl: './student-photo-cell.scss'
})
export class StudentPhotoCell implements ICellRendererAngularComp {
  protected imageName: string | null = null;

  agInit(params: ICellRendererParams): void {
    this.imageName = params.data?.image || null;
  }

  refresh(params: ICellRendererParams): boolean {
    this.imageName = params.data?.image || null;
    return true;
  }
}

