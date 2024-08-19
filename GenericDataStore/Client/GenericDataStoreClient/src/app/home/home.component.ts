import { ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { ApiService } from '../Services/api.service';
import { Router } from '@angular/router';
import { Dialog, DialogModule  } from 'primeng/dialog';
import { DialogService, DynamicDialogModule, DynamicDialogRef } from 'primeng/dynamicdialog';
import { ObjecttypeCreateComponent } from '../Components/objecttype-create/objecttype-create.component';
import { CommonModule } from '@angular/common';
import { CardModule } from 'primeng/card';
import { AuthService } from '../Services/auth.service';
import { DataViewModule } from 'primeng/dataview';
import { ListViewComponent } from './list-view/list-view.component';
import { TagModule } from 'primeng/tag';
import { AnimateOnScrollModule } from 'primeng/animateonscroll';



@Component({
  selector: 'app-home',
  standalone: true,
  imports: [ButtonModule, DialogModule, DynamicDialogModule, CommonModule,CardModule,DataViewModule, ListViewComponent,TagModule,AnimateOnScrollModule],
  providers: [DialogService],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {

  constructor(public apiService: ApiService, protected router: Router, protected changeDetector: ChangeDetectorRef, protected dialogService: DialogService,public authService: AuthService) 
  {
  }

  ref: DynamicDialogRef | undefined;
  public types : any[] = [];
  public loggedin : boolean = true;
  @ViewChild('list', {static: false}) list: ListViewComponent | undefined;


  ngOnInit() {
    this.Refresh()
  }

  Refresh(){
    this.list?.Refresh();
    this.loggedin = this.authService.isLoggedIn()

      
  }

  CreateNew(){
    if(!this.loggedin){
      this.router.navigateByUrl('/login');
    }
    else{
      this.ref = this.dialogService.open(ObjecttypeCreateComponent,  { data: {}, header: 'Create Object Definition', resizable: true});
      this.ref.onClose.subscribe(x => {
        
  
        this.Refresh();
      });
    }
  }

  Contact(){
    this.router.navigateByUrl('/contact');
  }

  GoToType(name : string){
    this.router.navigateByUrl('/'+ name);
  }

  DemoList(){
    this.router.navigateByUrl('/demo');
  }

  Route(route : string){
    this.router.navigateByUrl('/'+route);
  }
}
