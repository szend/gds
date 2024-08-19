import { ChangeDetectorRef, Component } from '@angular/core';
import { ConfirmationService, MessageService } from 'primeng/api';
import { DynamicDialogConfig, DialogService } from 'primeng/dynamicdialog';
import { RootFilter } from '../../Models/Parameters';
import { ApiService } from '../../Services/api.service';
import { DropdownModule } from 'primeng/dropdown';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { MessagesModule } from 'primeng/messages';


@Component({
  selector: 'app-image-classification',
  standalone: true,
  imports: [DropdownModule,FormsModule,CommonModule,ButtonModule,ProgressSpinnerModule,MessagesModule],
  providers: [MessageService], 
  templateUrl: './image-classification.component.html',
  styleUrl: './image-classification.component.css'
})
export class ImageClassificationComponent {
  constructor(public apiService: ApiService,  protected config: DynamicDialogConfig,private messageService: MessageService
    , protected dialogService: DialogService
  ) 
  {
  }
  fields : {name : string, code : string}[] = [];
  rootFilter: RootFilter | undefined;
  loading: boolean = false;
  choosenfield : any | undefined = undefined;
  ngOnInit(): void {  
    if(this.config.data.fields){
      this.fields = this.config.data.fields.filter((x: { type: string; }) => x.type == "image").map((x: { header: any; })  => {return {name: x.header, code: x.header}});
    }
    if(this.config.data.rootFilter){
      this.rootFilter = this.config.data.rootFilter;
    }
  }


  Classify(){
    this.loading = true;
    this.apiService.ImageClassification(this.rootFilter, this.choosenfield?.name ?? "").subscribe(x => {
      this.loading = false;
      this.messageService.add({
        severity: 'success',
        summary: 'Success',
        detail: 'Classification finished',
        life: 300000
      });
      // this.ref = this.dialogService.open(ClustersComponent,  { data: {dataRec: x}, header: 'Clusters', resizable: true});
      // this.ref.onClose.subscribe(x => {
      //  // this.Refresh();
      // });
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
