import { ChangeDetectorRef, Component, OnInit, forwardRef } from '@angular/core';
import { ConfirmationService, MessageService } from 'primeng/api';
import { DynamicDialogConfig, DialogService } from 'primeng/dynamicdialog';
import { ApiService } from '../../Services/api.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { CalendarModule } from 'primeng/calendar';
import { CheckboxModule } from 'primeng/checkbox';
import { ChipModule } from 'primeng/chip';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DropdownModule } from 'primeng/dropdown';
import { FileUploadModule } from 'primeng/fileupload';
import { ImageModule } from 'primeng/image';
import { InputGroupModule } from 'primeng/inputgroup';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextModule } from 'primeng/inputtext';
import { KeyFilterModule } from 'primeng/keyfilter';
import { MessagesModule } from 'primeng/messages';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { TabViewModule } from 'primeng/tabview';
import { ToastModule } from 'primeng/toast';
import { TriStateCheckboxModule } from 'primeng/tristatecheckbox';
import { DataobjectListComponent } from '../dataobject-list/dataobject-list.component';
import { error } from 'console';

@Component({
  selector: 'app-database-tables',
  standalone: true,
  imports: [InputGroupModule, FormsModule,CommonModule,TriStateCheckboxModule,CalendarModule,ConfirmDialogModule,DropdownModule,TabViewModule,ImageModule,ChipModule,
    InputTextModule,InputNumberModule,ButtonModule,MessagesModule,CheckboxModule,forwardRef(() => DataobjectListComponent),FileUploadModule,KeyFilterModule,
    ProgressSpinnerModule,ToastModule
  ],
    providers: [MessageService], 
  templateUrl: './database-tables.component.html',
  styleUrl: './database-tables.component.css'
})
export class DatabaseTablesComponent  implements OnInit {

  constructor( private confirmationService: ConfirmationService,public apiService: ApiService, protected changeDetector: ChangeDetectorRef,  protected config: DynamicDialogConfig,private messageService: MessageService
    , protected dialogService: DialogService
  ) 
  {
  }
  public loading: boolean = false;
  public action : string | undefined;
  public tables: any[] = [];
  public selectedtalbes : any[] = [];
  public boolvalues : boolean[] = [];

  ngOnInit(): void {
    if(this.config.data.id){
      this.loading = true;
      this.apiService.GetTableNames(this.config.data.id).subscribe((data: any) => {
        this.loading = false;
        this.tables = data;
      },error => {
        this.loading = false;
        this.messageService.add({severity:'error', summary: 'Error', detail: 'Error'});
      });
    } 
    if(this.config.data.action){
      this.action = this.config.data.action;
    }
  }

  Save(){
    this.loading = true;
    if(this.action == "Connect"){
      this.apiService.ImportTables(this.config.data.id,this.selectedtalbes).subscribe((data: any) => {
        this.loading = false;
        this.messageService.add({severity:'success', summary: 'Success', detail: 'Tables saved'});
      },error => {
        this.loading = false;
        this.messageService.add({severity:'error', summary: 'Error', detail: error.error});
      
      });
    }
    else if(this.action == "Refresh"){
      this.apiService.RefreshTables(this.config.data.id,this.selectedtalbes).subscribe((data: any) => {
        this.loading = false;
        this.messageService.add({severity:'success', summary: 'Success', detail: 'Tables saved'});
      },error => {
        this.loading = false;
        this.messageService.add({severity:'error', summary: 'Error', detail: error.error});
      
      });
    }
    else if(this.action == "Disconnect"){
      this.apiService.DisconnectTables(this.config.data.id,this.selectedtalbes).subscribe((data: any) => {
        this.loading = false;
        this.messageService.add({severity:'success', summary: 'Success', detail: 'Tables disconnected'});
      },error => {
        this.loading = false;
        this.messageService.add({severity:'error', summary: 'Error', detail: 'Error saving tables'});
      
      });
    }
   
  }

  AllSelected(){
    if(this.selectedtalbes.length != this.tables.length){
      this.selectedtalbes = this.tables; 
      let idx = 0;
      for(let t of this.tables){
        this.boolvalues[idx] = true;
        idx++;
      }
    }
    else{
      this.selectedtalbes = [];
      let idx = 0;
      for(let t of this.tables){
        this.boolvalues[idx] = false;
        idx++;
      }
  }
}

  Select(table: string){
    if(this.selectedtalbes.includes(table)){
      this.selectedtalbes = this.selectedtalbes.filter((value: any) => value != table);
    }
    else{
      this.selectedtalbes.push(table);
    }
  }

  onReject() {
    this.messageService.clear();
  }
}
