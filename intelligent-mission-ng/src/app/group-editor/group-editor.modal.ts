import { Component, OnInit } from '@angular/core';
import { NgbActiveModal, NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { PersonGroup } from '../shared/shared';

@Component({
    moduleId: module.id,
    selector: 'group-editor',
    templateUrl: 'group-editor.modal.html'
})
export class GroupEditorModal {
    public editableItem = new PersonGroup();

    constructor(public activeModal: NgbActiveModal) { }

    save() {
        //TODO: validation
        this.activeModal.close(this.editableItem);
    }
}