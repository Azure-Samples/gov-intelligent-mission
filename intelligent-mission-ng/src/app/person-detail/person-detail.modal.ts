import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Subscription } from 'rxjs';

import { MIApiService } from '../services/services';
import { Person } from '../shared/shared';

@Component({
    selector: 'person-detail-modal',
    moduleId: module.id,
    templateUrl: 'person-detail.modal.html'
})
export class PersonDetailModal implements OnInit {
    public busy: Subscription;
    public person: Person;
    public faces = [];

    constructor(public activeModal: NgbActiveModal, private miApi: MIApiService) { }

    ngOnInit() {
        this.busy = this.miApi.getPersonFaces(this.person.facePersonGroupId, this.person.facePersonId).subscribe(data => this.faces = data);
    }
}