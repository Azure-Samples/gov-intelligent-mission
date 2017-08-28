import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent, KnownIndividualsComponent, ImageAnalysisComponent, ImageAnalysisDetailComponent, AudioAnalysisComponent, AudioAnalysisDetailComponent, VideoAnalysisComponent, VideoAnalysisDetailComponent, TextAnalysisComponent } from './components';

const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'individuals', component: KnownIndividualsComponent },
  { path: 'image-analysis', component: ImageAnalysisComponent },
  { path: 'image-analysis/:imageId', component: ImageAnalysisDetailComponent },
  { path: 'audio-analysis', component: AudioAnalysisComponent },
  { path: 'audio-analysis/:audioId', component: AudioAnalysisDetailComponent },
  { path: 'video-analysis', component: VideoAnalysisComponent },
  { path: 'video-analysis/:videoId', component: VideoAnalysisDetailComponent },
  { path: 'text-analysis', component: TextAnalysisComponent },
  { path: '**', redirectTo: 'home' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
