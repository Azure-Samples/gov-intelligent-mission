import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { AppModule } from './app/app.module';
import { environment } from './environments/environment';
// import { AppInitializerService } from './app/services/services';
// import { HttpModule } from '@angular/http';

if (environment.production) {
  enableProdMode();
}

platformBrowserDynamic().bootstrapModule(AppModule);
// platformBrowserDynamic({
//   provide: APP_INITIALIZER, 
//   useFactory: (appInit:AppInitializerService) => () => appInit.load(), 
//   deps:[AppInitializerService, HttpModule], 
//   multi: true}).bootstrapModule(AppModule);

// window["imRunApp"] = function(identityInfo: any) {
//   platformBrowserDynamic([{provide: 'IdentityInfo', useValue: identityInfo }])
//     .bootstrapModule(AppModule);
//}