import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { NgbModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import * as _ from 'lodash';

import { PersonDetailModal } from '../components';
import { MIApiService } from '../services/services';


@Component({
    selector: 'image-analysis-detail',
    moduleId: module.id,
    templateUrl: './image-analysis-detail.component.html'
})
export class ImageAnalysisDetailComponent implements OnInit {
    public busy: Subscription;
    public detectedFaces: any;// = [];
    public detectedObjects: any;// = {};
    public multiplier: number;
    public selectedFace: any;
    public selectedImage: any;
    @ViewChild('mainImg') mainImg;


    constructor(
        private router: ActivatedRoute,
        private modal: NgbModal,
        private miApi: MIApiService) {}

    ngOnInit() {
        this.router.params.subscribe(params => {
            let imageId = params['imageId'];
            this.busy = this.miApi.getImageCatalogFile(imageId).subscribe(data => this.selectedImage = data);
        });
    }

    analyzeFaces() {
        this.busy = this.miApi.identifyFaces(this.selectedImage.id).subscribe(data => this.detectedFaces = data);
    }
    

    analyzeObjects() {
        this.busy = this.miApi.identifyObjects(this.selectedImage.id).subscribe(data => this.detectedObjects = data);
    }

    imageLoaded($event) {
        let img = this.mainImg.nativeElement;
        this.multiplier = img.clientWidth / img.naturalWidth;
    }

    faceClicked(face) {
        console.log('**selected face', face);
        this.selectedFace = face;
    }

    formatPct(value) {
        return _.round(value * 100) + '%';
    }

    viewDetail() {
        let modalRef = this.modal.open(PersonDetailModal, { size: 'lg' });
        modalRef.componentInstance.person = this.selectedFace.identifiedPerson.person;
    }

    viewResults(content) {
        this.modal.open(content);
    }
}