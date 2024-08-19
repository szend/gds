import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { DataViewModule } from 'primeng/dataview';
import { TagModule } from 'primeng/tag';
import { RootFilter } from '../../Models/Parameters';
import { ApiService } from '../../Services/api.service';
import { DynamicDialogConfig } from 'primeng/dynamicdialog';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { MessagesModule } from 'primeng/messages';



@Component({
  selector: 'app-objecttype-select',
  standalone: true,
  imports: [DataViewModule,CardModule,TagModule,ButtonModule,CommonModule,ProgressSpinnerModule,ToastModule,MessagesModule],
  providers: [MessageService],
  templateUrl: './objecttype-select.component.html',
  styleUrl: './objecttype-select.component.css'
})
export class ObjecttypeSelectComponent {
  constructor(protected route: ActivatedRoute,public apiService: ApiService, protected router: Router,  protected config: DynamicDialogConfig,public messageService: MessageService) 
  {
  }
  loading : boolean = false;

  rootFilter: RootFilter | undefined = {
    filters: [],
    valueFilters: [],
    logic: 'and',
    sortingParams: [{field: 'promoted', order: 1, type: 'boolean'}],
    valueSortingParams: [], 
    take: 0, 
    skip: 0,
    valueTake: 0,
    valueSkip: 0
  };

  types : any[] = [];
  parentid : string = '';
  remove : boolean = false;

  ngOnInit(): void {
    if(this.config.data.id){
      this.parentid = this.config.data.id;
    }
    if(!this.config.data.remove){
      this.apiService.GetTypeByFilter(this.rootFilter,true).subscribe(x => {
        this.types = x;
      });
    }
    else{
      this.remove = true;
      this.apiService.GetAllChild(this.parentid).subscribe(x => {
        this.types = x;
      });
    }

  }

  AddChild(childid : string){
    this.loading = true;
    this.apiService.SelectChild(childid,this.parentid).subscribe(x => {
      this.loading = false;
      this.messageService.add({severity:'success', summary:'Success', detail:'Child added successfully', life: 3000});
  }, error => {
    this.loading = false;
    this.messageService.add({severity:'error', summary:'Error', detail:'Error while adding child', life: 3000});
  });
}

RemoveChild(childid : string){
  this.loading = true;
  this.apiService.RemoveChild(childid,this.parentid).subscribe(x => {
    this.loading = false;
    this.messageService.add({severity:'success', summary:'Success', detail:'Child removed successfully', life: 3000});
}, error => {
  this.loading = false;
  this.messageService.add({severity:'error', summary:'Error', detail:'Error while removing child', life: 3000});
});

}

onReject() {
  this.messageService.clear();
}

}
