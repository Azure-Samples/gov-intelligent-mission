import { Component, OnInit } from '@angular/core';
import { NgbModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';

import { MIApiService } from '../services/services';
import { Subscription } from 'rxjs';
import * as _ from 'lodash';

@Component({
    selector: 'text-analysis',
    moduleId: module.id,
    templateUrl: './text-analysis.component.html'
})
export class TextAnalysisComponent implements OnInit {
    public busy: Subscription;
    public newsItems = [];
    public newsItemsChina = [];
    public analysisResult: any;
    public formattedResults = [];
    public resultsCollapsed = true;
    public selectedItem = null;
    public titlesVisible = true;

    constructor(
        private modal: NgbModal,
        private miApi: MIApiService) { }

    ngOnInit() {
        this.busy = this.miApi.getLatestNews().subscribe(data => {
            console.log('**news data', data);
            this.newsItems = data.worldNews.rss.channel.item;
            this.newsItemsChina = data.chinaNews.rss.channel.item;
        });
    }

    analyze() {
        this.busy = this.miApi.analyzeText(this.selectedItem.id).subscribe(data => {
            console.log('***data', data);
            this.analysisResult = data;
            this.resultsCollapsed = false;

            var length = this.analysisResult.keyPhrases.documents.length;
            this.formattedResults = [];
            for (var i = 0; i < length; i++) {
                if (this.analysisResult.keyPhrases.documents[i].keyPhrases[0]) {
                    this.formattedResults.push({
                        keyPhrases: this.analysisResult.keyPhrases.documents[i],
                        sentiment: this.analysisResult.sentiment.documents[i]
                    });
                }
            }
        });
    }

    translate() {
        this.busy = this.miApi.translateText(this.selectedItem.id).subscribe(data => {
            this.selectedItem = data;
        });
    }

    selectItem(item) {
        this.selectedItem = item;
        this.titlesVisible = false;
        this.resultsCollapsed = true;
    }

    formatArray(list) {
        return _.join(list, ', ');
    }

    viewResults(content) {
        this.modal.open(content);
    }
}