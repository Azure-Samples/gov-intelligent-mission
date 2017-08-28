import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { NgbModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { ToasterService, ToasterConfig } from 'angular2-toaster';
import * as _ from 'lodash';

import { MIApiService } from '../services/services';

@Component({
    selector: 'video-analysis-detail',
    moduleId: module.id,
    templateUrl: './video-analysis-detail.component.html'
})
export class VideoAnalysisDetailComponent implements OnInit {
    public busy: Subscription;
    public selectedVideo: any;
    public videoResults: any;
    public toasterConfig: ToasterConfig = new ToasterConfig({
        positionClass: 'toast-bottom-center'
    });

    constructor(
        private router: ActivatedRoute,
        private modal: NgbModal,
        public toastr: ToasterService,
        private miApi: MIApiService) { }

    ngOnInit() {
        this.router.params.subscribe(params => {
            let videoId = params['videoId'];
            this.busy = this.miApi.getVideoCatalogFile(videoId).subscribe(data => {
                console.log('**selected video', data);
                this.selectedVideo = data;
            });
        });
    }

    analyzeVideo() {
        this.busy = this.miApi.analyzeVideo(this.selectedVideo.id).subscribe(data => {
            console.log('***video analysis', data);
            this.videoResults = data;
        })
    }

    checkForAnalysisResults() {
        this.busy = this.miApi.getAnalysisResults(this.selectedVideo.id).subscribe(data => {
            console.log('**previous analysis results', data);
            if (data) {
                this.toastr.pop('success', 'Results Found', 'Previous Analysis results were found for this video.');
                var processingResult = JSON.parse(data.processingResult);
                console.log('**processingResult', processingResult);
                
            } else {
                this.toastr.pop('warning', 'Not Found', 'Previous Analysis results were not found for this video.');
            }
            this.videoResults = data;
        });
    }

    saveResults() {
        this.videoResults.id = this.selectedVideo.id;
        this.busy = this.miApi.saveAnalysisResults(this.videoResults).subscribe(data => {
            console.log('**video results saved');
        });
    }

    viewResults(content) {
        this.modal.open(content);
    }
}