import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { DynamicDialogConfig, DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { RootFilter } from '../../Models/Parameters';
import { ApiService } from '../../Services/api.service';
import { ClustersComponent } from '../clusters/clusters.component';
import { MessagesModule } from 'primeng/messages';

@Component({
  selector: 'app-classification',
  standalone: true,
  imports: [DropdownModule,FormsModule,CommonModule,ButtonModule,ProgressSpinnerModule,MessagesModule],
  providers: [MessageService,DialogService],
  templateUrl: './classification.component.html',
  styleUrl: './classification.component.css'
})
export class ClassificationComponent {
  constructor(public apiService: ApiService,  protected config: DynamicDialogConfig,private messageService: MessageService
    , protected dialogService: DialogService,
  ) 
  {
  }
  ref: DynamicDialogRef | undefined;

  fields : {name : string, code : string}[] = [];
  rootFilter: RootFilter | undefined;
  loading: boolean = false;
  choosenfield : any | undefined = undefined;
  ngOnInit(): void {  
    if(this.config.data.fields){
      this.fields = this.config.data.fields.filter((x: { type: string; }) => x.type == "text" || x.type == "option").map((x: { header: any; })  => {return {name: x.header, code: x.header}});
    }
    if(this.config.data.rootFilter){
      this.rootFilter = this.config.data.rootFilter;
    }
  }


  Classify(){
    this.loading = true;
    this.apiService.Classification(this.rootFilter, this.choosenfield?.name ?? "").subscribe(x => {
      this.loading = false;
      this.messageService.add({
        severity: 'success',
        summary: 'Success',
        detail: 'Classification finished',
        life: 300000
      });
    },error => {
      this.messageService.add({ 
        severity: "error", 
        summary: "Error", 
        detail: error.error, 
        life: 300000
      });
      this.loading = false;
    
    });
   }
}
