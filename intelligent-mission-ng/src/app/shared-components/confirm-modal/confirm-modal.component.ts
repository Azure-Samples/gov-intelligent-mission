import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
    moduleId: module.id,
    selector: 'confirm-modal',
    templateUrl: 'confirm-modal.component.html'
})
export class ConfirmModalComponent implements OnInit {
    public properties: ConfirmModalProperties;

    constructor(public activeModal: NgbActiveModal) { }

    ngOnInit() { }
}

export class ConfirmModalProperties {
    title: string;
    message: string;
    buttons: string[];
}