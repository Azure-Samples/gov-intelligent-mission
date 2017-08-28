import { Component, Inject, OnInit } from '@angular/core';
import { MIApiService } from '../services/services';
import { Subscription } from 'rxjs';
import { NgbModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { ToasterService, ToasterConfig } from 'angular2-toaster';
import * as _ from 'lodash';

import { ConfirmModalComponent, ConfirmModalProperties, FileUploadModal, FileUploadModalProperties } from '../components';
import { FileUpload, IdentityInfo } from '../shared/shared';
import { IdentityInfoService } from '../services/services';

@Component({
    selector: 'video-analysis',
    moduleId: module.id,
    templateUrl: './video-analysis.component.html'
})
export class VideoAnalysisComponent implements OnInit {
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
        this.busy = this.miApi.getVideoCatalogFiles().subscribe(data => this.files = data);
    }

    addFile() {
        let modalRef = this.modal.open(FileUploadModal);
        modalRef.componentInstance.properties = <FileUploadModalProperties>{ acceptFileTypes: '.mp4' };
        modalRef.result.then(result => {
            this.busy = this.miApi.addVideoCatalogFile(result).subscribe(savedItem => {
                this.files.push(savedItem);
                this.toastr.pop('success', 'Complete', 'New audio file added successfully');
            });
        }, reason => reason);
    }

    deleteVideo(id) {
        let modalRef = this.modal.open(ConfirmModalComponent);
        modalRef.componentInstance.properties = <ConfirmModalProperties>{ title: 'Delete Video?', message: 'Are you sure you want to delete this video?', buttons: ['Yes (Delete)', 'No'] };
        modalRef.result.then(result =>
            this.busy = this.miApi.deleteVideoCatalogFile(id).subscribe(() =>
                _.remove(this.files, f => f.id === id)), reason => reason);
    }
}