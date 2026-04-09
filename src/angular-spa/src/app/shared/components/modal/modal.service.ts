import { Injectable, inject } from '@angular/core';
import { Dialog, DialogRef } from '@angular/cdk/dialog';
import { ModalComponent, ModalData } from './modal.component';
import { Observable } from 'rxjs';
import { ComponentType } from '@angular/cdk/portal';

@Injectable({
  providedIn: 'root'
})
export class ModalService {
  private dialog = inject(Dialog);

  /**
   * Opens a simple confirmation/alert modal.
   * @param data The title, content, buttons configuration.
   * @returns An observable resolving to true (confirmed) or false/undefined (cancelled/closed).
   */
  openConfirm(data: ModalData): Observable<boolean | undefined> {
    const dialogRef = this.dialog.open<boolean>(ModalComponent, {
      data,
      minWidth: '320px',
      maxWidth: '440px',
      panelClass: 'modal-cdk-panel',
      backdropClass: 'modal-cdk-backdrop',
      disableClose: false
    });

    return dialogRef.closed;
  }

  /**
   * Opens a custom component within the modal system.
   * @param component The Angular component to render inside the dialog.
   * @param data Any data to pass to the component.
   */
  openCustom<T, D = any, R = any>(component: ComponentType<T>, data?: D) {
    return this.dialog.open<R, D, T>(component, {
      data,
      panelClass: 'modal-cdk-panel',
      backdropClass: 'modal-cdk-backdrop',
      disableClose: false
    });
  }
}
