import { Component, Inject, OnInit } from '@angular/core';
import { MIApiService } from '../services/services';
import { Subscription } from 'rxjs';
import { NgbModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { ToasterService, ToasterConfig } from 'angular2-toaster';
import * as _ from 'lodash';

import { ConfirmModalComponent, ConfirmModalProperties, FileUploadModal, FileUploadModalProperties } from '../components';
import { IdentityInfoService } from '../services/services';
import { FileUpload, IdentityInfo } from '../shared/shared';

@Component({
    selector: 'image-analysis',
    moduleId: module.id,
    templateUrl: './image-analysis.component.html'
})
export class ImageAnalysisComponent implements OnInit {
    public identityInfo: IdentityInfo;
    public busy: Subscription;
    public files: FileUpload[] = [];
    public toasterConfig: ToasterConfig = new ToasterConfig({
        positionClass: 'toast-bottom-center'
    });

    constructor(
        private modal: NgbModal,
        private miApi: MIApiService,
        public toastr: ToasterService,
        identityService: IdentityInfoService) { 
        this.identityInfo = identityService.info;
    }

    ngOnInit() {
        this.busy = this.miApi.getImageCatalogFiles().subscribe(data => this.files = data);
    }

    addFile() {
        let modalRef = this.modal.open(FileUploadModal);
        modalRef.componentInstance.properties = <FileUploadModalProperties>{ acceptFileTypes: '.jpeg,.jpg,.png,.gif' };
        modalRef.result.then(result => {
            this.busy = this.miApi.addImageCatalogFile(result).subscribe(savedItem => {
                this.files.push(savedItem);
                this.toastr.pop('success', 'Complete', 'New image file added successfully');
            });
        }, reason => reason);
    }

    deleteImage(id) {
        let modalRef = this.modal.open(ConfirmModalComponent);
        modalRef.componentInstance.properties = <ConfirmModalProperties>{ title: 'Delete Image?', message: 'Are you sure you want to delete this image?', buttons: ['Yes (Delete)', 'No'] };
        modalRef.result.then(result => 
            this.busy = this.miApi.deleteImageCatalogFile(id).subscribe(() => 
                _.remove(this.files, f => f.id === id)), reason => reason);
    }
}