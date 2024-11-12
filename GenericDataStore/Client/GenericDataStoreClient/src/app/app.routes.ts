import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { DataobjectListComponent } from './Components/dataobject-list/dataobject-list.component';
import { LoginComponent } from './User/login/login.component';
import { RegisterComponent } from './User/register/register.component';
import { MessageComponent } from './User/message/message.component';

import { ListViewComponent } from './home/list-view/list-view.component';

import { SearchviewComponent } from './home/searchview/searchview.component';
import { DatabaseListComponent } from './Components/database-list/database-list.component';
import { PageViewComponent } from './Components/page-view/page-view.component';
import { ContactComponent } from './home/contact/contact.component';
import { PriceComponent } from './home/price/price.component';
import { FeaturesComponent } from './home/features/features.component';
import { ConfirmpaymentComponent } from './home/confirmpayment/confirmpayment.component';
import { MylistsComponent } from './home/mylists/mylists.component';
import { DashboardComponent } from './home/dashboard/dashboard.component';
import { TermsUseComponent } from './home/terms-use/terms-use.component';
import { DataProtectionComponent } from './home/data-protection/data-protection.component';
import { DashboardpublicComponent } from './home/dashboardpublic/dashboardpublic.component';
import { ApilistComponent } from './Components/apilist/apilist.component';


export const routes: Routes = [

{
     path: '',
    runGuardsAndResolvers: 'always',
    
    children: [
      { path: 'login', component: LoginComponent },
      { path: 'termsofuse', component: TermsUseComponent },
      { path: 'dataprotection', component: DataProtectionComponent },
      { path: 'register', component: RegisterComponent },
      { path: 'mylists', component: MylistsComponent },
      { path: 'confirm/:intcount/:extcount/:listcount', component: ConfirmpaymentComponent},
      { path: 'contact', component: ContactComponent },
      { path: 'publicdata', component: ListViewComponent },
      { path: 'dashboard', component: DashboardComponent },
      { path: 'publicdashboard/:id', component: DashboardpublicComponent },
      { path: 'home', component: HomeComponent },
      { path: 'messages', component: MessageComponent },
      { path: 'features', component: FeaturesComponent },
      { path: 'price', component: PriceComponent },
      { path: 'connection/databases', component: DatabaseListComponent },
      { path: 'list/:category', component: ListViewComponent },
      { path: 'search/:searchstring', component: SearchviewComponent },
      { path: '', component: HomeComponent },
      { path: 'page/:id/:name/:page', component: PageViewComponent },
      { path: ':id/:name', component: DataobjectListComponent },
      { path: ':private/:id/:name', component: DataobjectListComponent},


    ]
}

];
