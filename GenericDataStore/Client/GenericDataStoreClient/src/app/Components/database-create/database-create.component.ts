import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit, forwardRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MessageService, ConfirmationService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { CalendarModule } from 'primeng/calendar';
import { CheckboxModule } from 'primeng/checkbox';
import { ChipModule } from 'primeng/chip';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DropdownModule } from 'primeng/dropdown';
import { DynamicDialogConfig, DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
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
import { ApiService } from '../../Services/api.service';
import { DataobjectListComponent } from '../dataobject-list/dataobject-list.component';
import { DatabaseTablesComponent } from '../database-tables/database-tables.component';
import { ChartComponent } from '../chart/chart.component';

@Component({
  selector: 'app-database-create',
  standalone: true,
  imports: [InputGroupModule, FormsModule,CommonModule,TriStateCheckboxModule,CalendarModule,ConfirmDialogModule,DropdownModule,TabViewModule,ImageModule,ChipModule,
    InputTextModule,InputNumberModule,ButtonModule,MessagesModule,CheckboxModule,forwardRef(() => DataobjectListComponent),FileUploadModule,KeyFilterModule,
    ProgressSpinnerModule,ToastModule
  ],
    providers: [MessageService,ConfirmationService], 
  templateUrl: './database-create.component.html',
  styleUrl: './database-create.component.css'
})
export class DatabaseCreateComponent implements OnInit {


  constructor( private confirmationService: ConfirmationService,public apiService: ApiService, protected changeDetector: ChangeDetectorRef,  protected config: DynamicDialogConfig,private messageService: MessageService
    , protected dialogService: DialogService
  ) 
  {
  }

  options = [
    {label: 'SQL Server', value: 'SQL Server'},
    {label: 'MYSQL', value: 'MYSQL'},

  ];
  idoptions = [
    {label: 'int', value: 'int'},
    {label: 'uniqueidentifier', value: 'uniqueidentifier'},

  ];
  ref: DynamicDialogRef | undefined;

  public childlist : any | undefined;
  public loading: boolean = false;
  public database: any = {};
  public createmode : boolean = false;

  ngOnInit(): void {
    if(this.config.data.createmode == true){
      this.createmode = true;
    }
    else if(this.config.data.createmode == false){     
      this.database = this.config.data.dataRec;
    }

  }

  OptionSelect(event : any){
    this.database.databaseType = event.value.value;

  }
  IdTypeSelect(event : any){
    this.database.defaultIdType = event.value.value;

  }

  onReject() {
    this.messageService.clear();
  }

  Save() {
    this.loading = true;
    if(this.config.data.createmode == true){
      this.apiService.Connect(this.database).subscribe((data: any) => {
        this.loading = false;
        this.messageService.add({severity:'success', summary: 'Success', detail: 'Database created successfully'});
      }, (error: any) => {
        this.loading = false;
        this.messageService.add({severity:'error', summary: 'Error', detail: 'Database creation failed'});
      });
    }
    else if(this.config.data.createmode == false){
      this.apiService.EditConnection(this.database).subscribe((data: any) => {
        this.loading = false;
        this.messageService.add({severity:'success', summary: 'Success', detail: 'Database updated successfully'});
      }, (error: any) => {
        this.loading = false;
        this.messageService.add({severity:'error', summary: 'Error', detail: 'Database update failed'});
      });
    }
  }

  ImportTables(){
    this.ref = this.dialogService.open(DatabaseTablesComponent,  { data: {id: this.database.databaseConnectionPropertyId, action: 'Connect'}, header: 'Connect tables', resizable: true});
  }

  RefreshTables(){
    this.ref = this.dialogService.open(DatabaseTablesComponent,  { data: {id: this.database.databaseConnectionPropertyId, action: 'Refresh'}, header: 'Refresh tables', resizable: true});
  }

  DisconnectTables(){
    this.ref = this.dialogService.open(DatabaseTablesComponent,  { data: {id: this.database.databaseConnectionPropertyId, action: 'Disconnect'}, header: 'Disconnect tables', resizable: true});

  }

  DisconnectDatabase(){
    this.apiService.DeleteDatabase(this.database.databaseConnectionPropertyId).subscribe((data: any) => {
      this.messageService.add({severity:'success', summary: 'Success', detail: 'Database disconnected successfully'});
    }, (error: any) => {
      this.messageService.add({severity:'error', summary: 'Error', detail: 'Database disconnection failed'});
  });
  }

  Chart(){
    this.loading = true;
    this.apiService.ChartDataBase(this.database.databaseConnectionPropertyId).subscribe(x => {
      this.loading = false;
      let dataRec : any= {};
      dataRec.organisation = x;
      this.ref = this.dialogService.open(ChartComponent,  { data: {dataRec: dataRec, }, header: 'Diagram', resizable: true});
      this.ref.onClose.subscribe(x => {
       // this.Refresh();
      });
    },error => {
      this.messageService.add({ 
        severity: "error", 
        summary: "Error", 
        detail: "Something went wrong.", 
        life: 300000
      });
      this.loading = false;
    
    });

  }

}
