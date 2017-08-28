import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MIApiService } from '../services/services';
import { Subscription } from 'rxjs';
import { NgbModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { ToasterService, ToasterConfig } from 'angular2-toaster';

import { FileUploadModal, FileUploadModalProperties, PersonDetailModal } from '../components';

@Component({
    selector: 'audio-analysis-detail',
    moduleId: module.id,
    templateUrl: './audio-analysis-detail.component.html'
})
export class AudioAnalysisDetailComponent implements OnInit {
    public analysisResult: any = {};
    public busy: Subscription;
    public identifiedSpeaker: any = {};
    public selectedAudio: any = {};
    public speakerIdentified = false;
    public toasterConfig: ToasterConfig = new ToasterConfig({
        positionClass: 'toast-bottom-center'
    });

    constructor(
        private router: ActivatedRoute,
        private modal: NgbModal,
        private miApi: MIApiService,
        public toastr: ToasterService) { }

    ngOnInit() {
        this.router.params.subscribe(params => {
            let audioIdParam = params['audioId'];
            this.busy = this.miApi.getAudioCatalogFile(audioIdParam).subscribe(data => this.selectedAudio = data);
        });
    }

    analyzeAudio() {
        this.busy = this.miApi.executeAudioRecognition(this.selectedAudio.id).subscribe(data => {
            this.analysisResult = data;
            
            if (this.analysisResult.status === 'succeeded') {
                this.speakerIdentified = this.analysisResult.processingResult.identifiedProfileId !== '00000000-0000-0000-0000-000000000000';
                if (this.speakerIdentified) {
                    this.busy = this.miApi.getPersonBySpeakerProfileId(this.analysisResult.processingResult.identifiedProfileId).subscribe(data => {
                        this.identifiedSpeaker = data;
                        this.toastr.pop('success', 'Speech analysis complete', 'Speaker was identified');
                    });
                } else {
                    this.toastr.pop('warning', 'Speaker analysis complete', 'Unable to identify speaker');
                }
            } else {
                this.speakerIdentified = false;
                this.toastr.pop('error', 'Unknown error', 'Unknown error while performing speaker recognition');
            }
        });
    }

    viewDetail() {
        let modalRef = this.modal.open(PersonDetailModal, { size: 'lg' });
        modalRef.componentInstance.person = this.identifiedSpeaker;
    }

    viewResults(content) {
        this.modal.open(content);
    }
}