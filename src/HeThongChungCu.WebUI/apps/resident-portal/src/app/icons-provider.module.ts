import { NgModule } from '@angular/core';
import { NZ_ICONS, NzIconModule } from 'ng-zorro-antd/icon';

import {
  MenuFoldOutline,
  MenuUnfoldOutline,
  FormOutline,
  DashboardOutline,
  UserOutline,
  LockOutline,
  EyeOutline,
  EyeInvisibleOutline,
  LoginOutline,
  HomeOutline,
  TeamOutline,
  BankOutline,
  ToolOutline,
  PayCircleOutline,
  AppstoreOutline,
  NotificationOutline,
  SettingOutline,
  LogoutOutline,
  DownOutline,
  SearchOutline,
  FilterOutline,
  ReloadOutline,
  PlusOutline,
  UnorderedListOutline,
  DeleteOutline,
  CommentOutline,
  StopOutline,
  ClearOutline,
  EnvironmentOutline,
  ArrowDownOutline,
  ArrowUpOutline,
  CarOutline,
  CreditCardOutline,
  FileTextOutline,
  LinkOutline,
  BellOutline,
  CaretRightOutline,
  PlayCircleOutline
} from '@ant-design/icons-angular/icons';

const icons = [
  MenuFoldOutline,
  MenuUnfoldOutline,
  DashboardOutline,
  FormOutline,
  UserOutline,
  LockOutline,
  EyeOutline,
  EyeInvisibleOutline,
  LoginOutline,
  HomeOutline,
  TeamOutline,
  BankOutline,
  ToolOutline,
  PayCircleOutline,
  AppstoreOutline,
  NotificationOutline,
  SettingOutline,
  LogoutOutline,
  SearchOutline,
  FilterOutline,
  ReloadOutline,
  PlusOutline,
  UnorderedListOutline,
  DeleteOutline,
  DownOutline,
  CommentOutline,
  StopOutline,
  ClearOutline,
  EnvironmentOutline,
  ArrowDownOutline,
  ArrowUpOutline,
  CarOutline,
  CreditCardOutline,
  FileTextOutline,
  LinkOutline,
  BellOutline,
  CaretRightOutline,
  PlayCircleOutline
];

@NgModule({
  imports: [NzIconModule],
  exports: [NzIconModule],
  providers: [
    { provide: NZ_ICONS, useValue: icons }
  ]
})
export class IconsProviderModule {
}
