import { Inject, Injectable } from '@angular/core';
import { Http, Response, RequestOptions, Headers } from '@angular/http';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map';

import { FileUpload } from '../shared/shared';

@Injectable()
export class MIApiService {
    //private baseUrl = 'https://localhost:44396';
    private baseUrl = '';
    constructor(private http: Http) {}

    createPerson(person) {
        return this.http.post(`${this.baseUrl}/api/persons`, person).map(response => response.json());
    }

    updatePerson(person) {
        return this.http.put(`${this.baseUrl}/api/persons/${person.id}`, person).map(response => response.json());
    }

    getPersonsByGroup(personGroupId) {
        return this.http.get(`${this.baseUrl}/api/persons?personGroupId=${personGroupId}`).map(response => response.json());
    }

    getPerson(id) {
        return this.http.get(`${this.baseUrl}/api/persons/${id}`).map(response => response.json());
    }

    getPersonBySpeakerProfileId(speakerIdentificationProfileId) {
        return this.http.get(`${this.baseUrl}/api/persons?speakerIdentificationProfileId=${speakerIdentificationProfileId}`).map(response => response.json());
    }

    getPersonGroups() {
        return this.http.get(`${this.baseUrl}/api/face/person-groups`).map(response => response.json());
    }

    createPersonGroup(personGroup) {
        return this.http.post(`${this.baseUrl}/api/face/person-groups`, personGroup).map(response => response.json());
    }

    createGroupPerson(personGroupId, person) {
        return this.http.post(`${this.baseUrl}/api/face/person-groups/${personGroupId}/persons`, person).map(response => response.json());
    }

    getPersonGroupPersonList(personGroupId) {
        return this.http.get(`${this.baseUrl}/api/face/person-groups/${personGroupId}/persons`).map(response => response.json());
    }

    getPersonFaces(personGroupId, personId) {
        return this.http.get(`${this.baseUrl}/api/face/person-groups/${personGroupId}/persons/${personId}/faces`).map(response => response.json());
    }

    addPersonFace(personGroupId, personId, imgFile) {
        let formData = new FormData();
        formData.append('uploadFile', imgFile);
        let headers = new Headers();
        //headers.append('Content-Type', 'multipart/form-data');
        headers.append('Content-Type', undefined);
        headers.append('Accept', 'application/json');
        let options = new RequestOptions({ headers: headers});
        return this.http.post(`${this.baseUrl}/api/face/person-groups/${personGroupId}/persons/${personId}/faces`, formData)//, options)
            .map(response => response.json());
    }

    addAudioEnrollment(personId, fileToUpload: FileUpload) {
        return this.addCatalogFile(`${this.baseUrl}/api/audio/${personId}/enroll`, fileToUpload);
    }

    deletePersonFace(personGroupId, personId, faceId) {
        return this.http.delete(`${this.baseUrl}/api/face/person-groups/${personGroupId}/persons/${personId}/faces/${faceId}`);
    }

    deletePersonGroup(personGroupId) {
        return this.http.delete(`${this.baseUrl}/api/face/person-groups/${personGroupId}`);
    }

    deletePerson(personGroupId, personId) {
        return this.http.delete(`${this.baseUrl}/api/face/person-groups/${personGroupId}/persons/${personId}`);
    }

    trainPersonGroup(personGroupId) {
        return this.http.post(`${this.baseUrl}/api/face/person-groups/${personGroupId}/train`, null);//.map(response => response.json());
    }

    getGroupTrainingStatus(personGroupId) {
        return this.http.get(`${this.baseUrl}/api/face/person-groups/${personGroupId}/train`).map(response => response.json());
    }


    getAudioCatalogFiles() {
        return this.http.get(`${this.baseUrl}/api/audio/catalog-files`).map(response => response.json());
    }

    getAudioCatalogFile(audioId) {
        return this.http.get(`${this.baseUrl}/api/audio/catalog-files/${audioId}`).map(response => response.json());
    }

    executeAudioRecognition(audioId) {
        return this.http.post(`${this.baseUrl}/api/audio/${audioId}/recognize`, null).map(response => response.json());
    }

    getImageCatalogFiles() {
        return this.http.get(`${this.baseUrl}/api/image/catalog-files`).map(response => response.json());
    }

    getImageCatalogFile(id) {
        return this.http.get(`${this.baseUrl}/api/image/catalog-files/${id}`).map(response => response.json());
    }

    getVideoCatalogFiles() {
        return this.http.get(`${this.baseUrl}/api/video/catalog-files`).map(response => response.json());
    }

    getVideoCatalogFile(id) {
        return this.http.get(`${this.baseUrl}/api/video/catalog-files/${id}`).map(response => response.json());
    }

    deleteImageCatalogFile(id) {
        return this.http.delete(`${this.baseUrl}/api/image/catalog-files/${id}`);
    }

    deleteVideoCatalogFile(id) {
        return this.http.delete(`${this.baseUrl}/api/video/catalog-files/${id}`);
    }

    addAudioCatalogFile(fileToUpload: FileUpload) {
        return this.addCatalogFile(`${this.baseUrl}/api/audio/catalog-files`, fileToUpload);
    }

    addImageCatalogFile(fileToUpload: FileUpload) {
        return this.addCatalogFile(`${this.baseUrl}/api/image/catalog-files`, fileToUpload);
    }

    addVideoCatalogFile(fileToUpload: FileUpload) {
        return this.addCatalogFile(`${this.baseUrl}/api/video/catalog-files`, fileToUpload);
    }

    getLatestNews() {
        return this.http.get(`${this.baseUrl}/api/text/latest-news`).map(response => response.json());
    }

    analyzeText(id) {
        return this.http.post(`${this.baseUrl}/api/text/latest-news/${id}/analyze`, null).map(response => response.json());
    }

    translateText(id) {
        return this.http.get(`${this.baseUrl}/api/translate?id=${id}`).map(response => response.json());
    }

    // Image Analysis
    detectFaces(imageId) {
        return this.http.post(`${this.baseUrl}/api/face/analysis/${imageId}/detect`, null).map(response => response.json());
    }

    identifyFaces(imageId) {
        return this.http.post(`${this.baseUrl}/api/image/analysis/${imageId}/identify`, null).map(response => response.json());
    }

    identifyObjects(imageId) {
        return this.http.post(`${this.baseUrl}/api/image/object-analysis/${imageId}/identify`, null).map(response => response.json());
    }

    // Video Analysis
    analyzeVideo(videoId) {
        return this.http.post(`${this.baseUrl}/api/video/analysis/${videoId}`, null).map(response => response.json());
    }

    // Analysis Results
    saveAnalysisResults(results) {
        if (!results.id) {
            throw 'You cannot save an Analysis Result unless it has an "id" property.';
        }
        return this.http.post(`${this.baseUrl}/api/analysis-results`, results).map(response => response.json());
    }

    getAnalysisResults(id) {
        return this.http.get(`${this.baseUrl}/api/analysis-results/${id}`).map(response => response.json());
    }


    // ***** Private Members ***** //

    private addCatalogFile(url: string, uploadFile: FileUpload) {
        let formData = new FormData();
        formData.append('uploadFile', uploadFile.file);
        formData.append('name', uploadFile.name);
        formData.append('description', uploadFile.description);
        let headers = new Headers();
        //headers.append('Content-Type', 'multipart/form-data');
        headers.append('Content-Type', undefined);
        headers.append('Accept', 'application/json');
        let options = new RequestOptions({ headers: headers });
        return this.http.post(url, formData)//, options)
            .map(response => response.json());
    }

}
