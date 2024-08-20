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
import { OfferComponent } from './home/offer/offer.component';
import { OfferlistComponent } from './home/offerlist/offerlist.component';
import { ContactComponent } from './home/contact/contact.component';
import { PriceComponent } from './home/price/price.component';
import { FeaturesComponent } from './home/features/features.component';
import { ConfirmpaymentComponent } from './home/confirmpayment/confirmpayment.component';


export const routes: Routes = [

{
     path: '',
    runGuardsAndResolvers: 'always',
    
    children: [
      { path: 'login', component: LoginComponent },
      { path: 'register', component: RegisterComponent },
      { path: 'confirm/:intcount/:extcount/:listcount', component: ConfirmpaymentComponent},
      { path: 'contact', component: ContactComponent },
      { path: 'home', component: HomeComponent },
      { path: 'messages', component: MessageComponent },
      { path: 'features', component: FeaturesComponent },
      { path: 'price', component: PriceComponent },
      { path: 'getalloffer', component: OfferlistComponent },
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
