import { Component, OnInit, ViewChild } from '@angular/core';
import { NgbActiveModal, NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { FileUpload } from '../shared/shared';

@Component({
    moduleId: module.id,
    selector: 'file-upload',
    templateUrl: 'file-upload.modal.html'
})
export class FileUploadModal {
    public editableItem = new FileUpload();
    public properties: FileUploadModalProperties;
    @ViewChild('fileInput') fileInput;


    constructor(public activeModal: NgbActiveModal) { }

    fileChange($event) {
        if ($event.target.files.length === 0) {
            return;
        }

        this.editableItem.file = this.fileInput.nativeElement.files[0];
    }

    save() {
        //TODO: validation
        this.activeModal.close(this.editableItem);
    }
}

export class FileUploadModalProperties {
    acceptFileTypes: string;
}