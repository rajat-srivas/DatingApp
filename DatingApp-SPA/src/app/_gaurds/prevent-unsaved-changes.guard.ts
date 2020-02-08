import { Injectable, Component } from '@angular/core';
import { CanDeactivate } from '@angular/router';
import { MemberEditComponent } from '../member-edit/member-edit.component';

@Injectable()
export class PreventUnsavedChanges implements CanDeactivate<MemberEditComponent> {
    canDeactivate(component: MemberEditComponent) {
        console.log('inside the gaurd');
        console.log(component.editForm.dirty);
        if (component.editForm.dirty) {
                return confirm('Are you sure you want to continue? Any unsaved changes will be lost');
            }
        return true;
    }
}
