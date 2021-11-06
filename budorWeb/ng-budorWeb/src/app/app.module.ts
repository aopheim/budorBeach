import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ImageComponent } from './images-list/image/image.component';
import { ImagesListComponent } from './images-list/images-list.component';

@NgModule({
  declarations: [
    AppComponent,
    ImageComponent,
    ImagesListComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
