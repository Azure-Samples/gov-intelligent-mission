import { Injectable } from '@angular/core';

@Injectable()
export class IdentityInfo {
    isAuthenticated: boolean;
    name: string;
    isAdmin: boolean;
    isUser: boolean;
}