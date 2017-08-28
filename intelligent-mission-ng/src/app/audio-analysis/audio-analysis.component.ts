import { Component, Inject, OnInit } from '@angular/core';
import { MIApiService } from '../services/services';
import { Subscription } from 'rxjs';
import { NgbModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { ToasterService, ToasterConfig } from 'angular2-toaster';

import { FileUploadModal, FileUploadModalProperties } from '../components';
import { FileUpload, IdentityInfo } from '../shared/shared';
import { IdentityInfoService } from '../services/services';

@Component({
    selector: 'audio-analysis',
    moduleId: module.id,
    templateUrl: './audio-analysis.component.html'
})
export class AudioAnalysisComponent implements OnInit {
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
        this.busy = this.miApi.getAudioCatalogFiles().subscribe(data => this.files = data);
    }

    addFile() {
        let modalRef = this.modal.open(FileUploadModal);
        modalRef.componentInstance.properties = <FileUploadModalProperties>{ acceptFileTypes: '.wav' };
        modalRef.result.then(result => {
            this.busy = this.miApi.addAudioCatalogFile(result).subscribe(savedItem => {
                this.files.push(savedItem);
                this.toastr.pop('success', 'Complete', 'New audio file added successfully');
            });
        }, reason => reason);
    }
}