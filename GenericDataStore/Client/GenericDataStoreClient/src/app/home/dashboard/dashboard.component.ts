import { ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { DialogService, DynamicDialogModule, DynamicDialogRef } from 'primeng/dynamicdialog';
import { ObjecttypeCreateComponent } from '../../Components/objecttype-create/objecttype-create.component';
import { ApiService } from '../../Services/api.service';
import { AuthService } from '../../Services/auth.service';
import { ListViewComponent } from '../list-view/list-view.component';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AnimateOnScrollModule } from 'primeng/animateonscroll';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { DataViewModule } from 'primeng/dataview';
import { DialogModule } from 'primeng/dialog';
import { TagModule } from 'primeng/tag';
import { FooterComponent } from '../footer/footer.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [ButtonModule, DialogModule, DynamicDialogModule, CommonModule,CardModule,DataViewModule, ListViewComponent,TagModule,AnimateOnScrollModule,FooterComponent],
  providers: [DialogService],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent  implements OnInit {

  constructor(public apiService: ApiService, protected router: Router, protected changeDetector: ChangeDetectorRef, protected dialogService: DialogService,public authService: AuthService) 
  {
  }

  ref: DynamicDialogRef | undefined;
  public types : any[] = [];
  @ViewChild('list', {static: false}) list: ListViewComponent | undefined;


  ngOnInit() {
    this.Refresh()
  }

  Refresh(){
    this.list?.Refresh();

      
  }

  CreateNew(){
      this.ref = this.dialogService.open(ObjecttypeCreateComponent,  { data: {}, header: 'Create Object Definition', resizable: true});
      this.ref.onClose.subscribe(x => {
        this.Refresh();
      });
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
