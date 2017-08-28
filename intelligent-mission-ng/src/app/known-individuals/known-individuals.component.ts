import { Component, Inject, OnInit, ViewChild, ViewContainerRef } from '@angular/core';
import { MIApiService } from '../services/services';
import { Subscription } from 'rxjs';
import { NgbModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { ToasterService, ToasterConfig } from 'angular2-toaster';
import * as _ from 'lodash';

import { ConfirmModalComponent, ConfirmModalProperties, FileUploadModal, FileUploadModalProperties, GroupEditorModal, PersonEditorModal } from '../components';
import { IdentityInfo, PersonFace } from '../shared/shared';
import { IdentityInfoService } from '../services/services';

@Component({
    selector: 'known-individuals',
    moduleId: module.id,
    templateUrl: './known-individuals.component.html'
})
export class KnownIndividualsComponent implements OnInit {
    public busy: Subscription;
    public identityInfo: IdentityInfo
    public personFaces = [];
    public personGroups = [];
    public personList = [];
    public selectedGroupId = '';
    public selectedPerson: any;
    public toasterConfig: ToasterConfig =
        new ToasterConfig({
            showCloseButton: true,
            positionClass: 'toast-bottom-center'
        });


    @ViewChild('fileInput') fileInput;
    
    constructor(
        private modal: NgbModal, 
        private miApi: MIApiService,
        public toastr: ToasterService,
        //@Inject('IdentityInfo') public identityInfo: IdentityInfo
        public identityService: IdentityInfoService) {
            this.identityInfo = this.identityService.info;
    }


    ngOnInit() {
        this.busy = this.miApi.getPersonGroups().subscribe(result => this.personGroups = result);
    }

    addGroup() {
        if (!this.checkIsAdmin()) {
            return;
        }

        this.modal.open(GroupEditorModal).result.then(result => {
            this.busy = this.miApi.createPersonGroup(result).subscribe(savedPersonGroup => {
                this.personGroups.push(savedPersonGroup);
                this.selectedGroupId = savedPersonGroup.personGroupId;
            });
        }, reason => reason);
    }

    addPerson() {
        if (!this.checkIsAdmin()) {
            return;
        }

        this.modal.open(PersonEditorModal).result.then(result => {
            result.facePersonGroupId = this.selectedGroupId;
            this.busy = this.miApi.createPerson(result).subscribe(data => this.personList.push(data));
        }, reason => reason);
    }

    fileChange($event) {
        if ($event.target.files.length === 0) {
            return;
        }

        var file = this.fileInput.nativeElement.files[0];
        this.busy = this.miApi.addPersonFace(this.selectedGroupId, this.selectedPerson.facePersonId, file).subscribe(result => {
           this.personFaces.push(result);
        });
    }

    uploadAudioEnrollment() {
        if (!this.checkIsAdmin()) {
            return;
        }

        let modalRef = this.modal.open(FileUploadModal);
        modalRef.componentInstance.properties = <FileUploadModalProperties>{ acceptFileTypes: '.wav' };
        modalRef.result.then(result => {
            this.busy = this.miApi.addAudioEnrollment(this.selectedPerson.id, result).subscribe(savedItem => {
                console.log('**saved audio item', savedItem);
                //this.files.push(savedItem);
                this.toastr.pop('success', 'Complete', 'New audio file added successfully');
            }, err => this.toastr.pop('error', 'Error enrolling audio', 'Possibly invalid audio file format.'));
        }, reason => reason);
    }

    deleteFace(img: PersonFace) {
        if (!this.checkIsAdmin()) {
            return;
        }

        let modalRef = this.modal.open(ConfirmModalComponent);
        modalRef.componentInstance.properties = <ConfirmModalProperties>{ title: 'Delete Face?', message: 'Are you sure you want to delete this face?', buttons: ['Yes (Delete)', 'No'] };
        modalRef.result.then(result => {
            this.busy = this.miApi.deletePersonFace(this.selectedGroupId, this.selectedPerson.facePersonId, img.persistedFaceId).subscribe(() => {
                _.remove(this.personFaces, x => x.persistedFaceId === img.persistedFaceId);
            }); 
        }, reason => reason);
    }

    deletePersonGroup() {
        if (!this.checkIsAdmin()) {
            return;
        }

        let modalRef = this.modal.open(ConfirmModalComponent);
        modalRef.componentInstance.properties = <ConfirmModalProperties>{ title: 'Delete Group?', message: 'Are you sure you want to delete this group?', buttons: ['Yes (Delete)', 'No'] };
        modalRef.result.then(result => {
            this.busy = this.miApi.deletePersonGroup(this.selectedGroupId).subscribe(() => {
                _.remove(this.personGroups, x => x.personGroupId === this.selectedGroupId);
                this.selectedGroupId = '';
            });
        }, reason => reason);
    }

    

    deletePerson() {
        if (!this.checkIsAdmin()) {
            return;
        }

        let modalRef = this.modal.open(ConfirmModalComponent);
        modalRef.componentInstance.properties = <ConfirmModalProperties>{ title: 'Delete Person?', message: 'Are you sure you want to delete this person?', buttons: ['Yes (Delete)', 'No'] };
        modalRef.result.then(result => {
            this.busy = this.miApi.deletePerson(this.selectedGroupId, this.selectedPerson.facePersonId).subscribe(() => {
                _.remove(this.personList, x => x.id === this.selectedPerson.id);
                this.selectedPerson = null;
            });
        }, reason => reason);
    }

    editPerson() {
        if (!this.checkIsAdmin()) {
            return;
        }
        
        let modalRef = this.modal.open(PersonEditorModal);
        modalRef.componentInstance.entry = this.selectedPerson;
        modalRef.result.then(result => {
            this.busy = this.miApi.updatePerson(result).subscribe(data => _.assign(this.selectedPerson, data));
        }, reason => reason);
    }

    onGroupsChange() {
        if (this.selectedGroupId) {
            this.busy = this.miApi.getPersonsByGroup(this.selectedGroupId).subscribe(result => this.personList = result);
        }
    }

    personClick(person) {
        this.selectedPerson = person;
        console.log('**selectedPerson', this.selectedPerson);
        this.busy = this.miApi.getPersonFaces(this.selectedGroupId, this.selectedPerson.facePersonId).subscribe(result => this.personFaces = result);
    }

    trainPersonGroup() {
        if (!this.checkIsAdmin()) {
            return;
        }

        this.busy = this.miApi.trainPersonGroup(this.selectedGroupId).subscribe(() => {
            this.toastr.pop('info', 'Training Initiated', 'Training has been initiated...');
        });
    }

    getGroupTrainingStatus() {
        if (!this.checkIsAdmin()) {
            return;
        }

        this.busy = this.miApi.getGroupTrainingStatus(this.selectedGroupId).subscribe(result => {
            let status = <TrainingStatus>result.status;
            switch (status) {
                case TrainingStatus.succeeded:
                    this.toastr.pop('success', 'Training Succeeded');
                    break;
                case TrainingStatus.running:
                    this.toastr.pop('info', 'Training still in progress...', 'Check back later');
                    break;
                case TrainingStatus.failed:
                    this.toastr.pop('error', 'Error during Training', result.message);
                    break;
                default:
                    break;
            }
        });
    }

    private checkIsAdmin() {
        if (!this.identityInfo.isAdmin) {
            this.toastr.pop('warning', 'Insufficient Permissions', 'You do not have permission to perform this operation.');
            return false;
        }
        return true;
    }
    

}

enum TrainingStatus {
    succeeded = 0,
    failed = 1,
    running = 2
}

