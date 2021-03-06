import { BrowserModule, HammerGestureConfig, HAMMER_GESTURE_CONFIG } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import {NgxGalleryModule} from 'ngx-gallery';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';

import { AppComponent } from './app.component';
import {HttpClientModule} from '@angular/common/http';
import { NavComponent } from './nav/nav.component';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import { AuthService } from './_services/auth.service';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';
import { ErrorInterceptorProvider } from './_services/error.interceptor';
import { BsDropdownModule, TabsModule, BsDatepickerModule, PaginationModule, ButtonsModule } from 'ngx-bootstrap';
import { MemberListComponent } from './members/member-list/member-list.component';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import {RouterModule} from '@angular/router';
import {appRoutes} from './routes';
import { MemberCardComponent } from './members/member-list/member-card/member-card.component';
import { JwtModule } from '@auth0/angular-jwt';
import { MemberDetailComponent } from './members/member-list/member-detail/member-detail.component';
import { MemberDetailResolver } from './_resolvers/member-detail.resolver';
import { MemberListResolver } from './_resolvers/member-list.resolver';
import { MemberEditComponent } from './members/member-list/member-edit/member-edit.component';
import { MemberEditResolver } from './_resolvers/member-edit.resolver';
import { PreventUnsavedChanges } from './_guards/prevent-unsaved-changes.guard';
import { PhotoEditorComponent } from './members/member-list/photo-editor/photo-editor.component';
import { FileUploadModule } from 'ng2-file-upload';
import {TimeAgoPipe} from 'time-ago-pipe';
import { Button } from 'protractor';
import { ListsResolver } from './_resolvers/lists.resolver';
import { MessagesResolver } from './_resolvers/messages.resolver';
import { MemberMessagesComponent } from './members/member-list/member-messages/member-messages.component';





export function tokenGetter() {
 return localStorage.getItem('token');
}

export class CustomHammerConfig extends HammerGestureConfig {
   overides = {
      pinch: { enable: false},
      rotate: {enable: false}
   };
}

@NgModule({
   declarations: [
      AppComponent,
      NavComponent,
      HomeComponent,
      RegisterComponent,
      MemberListComponent,
      ListsComponent,
      MessagesComponent,
      MemberCardComponent,
      MemberDetailComponent,
      MemberEditComponent,
      MemberMessagesComponent,
      PhotoEditorComponent,
      TimeAgoPipe

   ],
   imports: [
      BrowserModule,
      BrowserAnimationsModule,
      HttpClientModule,
      FormsModule,
      FileUploadModule,
      NgxGalleryModule,
      ReactiveFormsModule,
      ButtonsModule.forRoot(),
      TabsModule.forRoot(),
      BsDatepickerModule.forRoot(),
      PaginationModule.forRoot(),
      BsDropdownModule.forRoot(),
      RouterModule.forRoot(appRoutes),
      JwtModule.forRoot({
         config: {tokenGetter,
         whitelistedDomains: ['localhost:5000'],
         blacklistedRoutes: ['localhost:5000/api/auth']
      }
      })
   ],
   providers: [
      AuthService,
      PreventUnsavedChanges,
      MessagesResolver,
      ErrorInterceptorProvider,
      MemberDetailResolver,
      MemberListResolver,
      ListsResolver,
      MemberEditResolver,
      { provide: HAMMER_GESTURE_CONFIG, useClass: CustomHammerConfig}

   ],
   bootstrap: [
      AppComponent
   ]
})
export class AppModule { }
