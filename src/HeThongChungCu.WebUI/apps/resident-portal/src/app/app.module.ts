import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule } from '@angular/forms';
import { registerLocaleData } from '@angular/common';
import en from '@angular/common/locales/en';
import { NZ_I18N, en_US } from 'ng-zorro-antd/i18n';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzMenuModule } from 'ng-zorro-antd/menu';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzNotificationModule } from 'ng-zorro-antd/notification';

import { AppRoutingModule } from './app-routing.module';
import { IconsProviderModule } from './icons-provider.module';
import { ShellComponent } from './shell/shell.component';
import { AuthInterceptor, ResponseNotifierInterceptor } from '@core/interceptors';
import { SharedUiModule } from '@shared/ui';
import { environment } from '@env/environment';

registerLocaleData(en);

@NgModule({
  declarations: [ShellComponent],
  imports: [
    BrowserModule,
    SharedUiModule,
    AppRoutingModule,
    FormsModule,
    HttpClientModule,
    BrowserAnimationsModule,
    IconsProviderModule,
    NzLayoutModule,
    NzMenuModule,
    NzModalModule,
    NzNotificationModule
  ],
  providers: [
    { provide: NZ_I18N, useValue: en_US },
    { provide: 'API_BASE', useValue: environment.apiBaseUrl },
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ResponseNotifierInterceptor, multi: true }
  ],
  bootstrap: [ShellComponent]
})
export class AppModule {}
