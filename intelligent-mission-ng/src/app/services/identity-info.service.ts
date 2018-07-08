import { Injectable } from '@angular/core';
import { Http, Response, RequestOptions, Headers } from '@angular/http';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map';
import {IdentityInfo} from '../shared/shared';

@Injectable()
export class IdentityInfoService {
  public info: IdentityInfo;

  constructor(private http:Http) { }

  public load() {
    //let url = 'https://localhost:44396/api/identity-info';
    let url = '/api/identity-info';
    return new Promise((resolve, reject) => {
      this.http.get(url)
        .map(res => res.json())
        .catch((error: any): any => {
          console.log('Configuration endpoint could not be read');
          resolve(true);
          return Observable.throw(error.json().error || 'Server error');
        })
        .subscribe((response) => {
          console.log('****GOT A RESPONSE:', response);
          this.info = response;
          resolve(true);
        });
    });
  }
}
