import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Router } from 'express';
import { MessageService, ConfirmationService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { CheckboxModule } from 'primeng/checkbox';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ContextMenuModule } from 'primeng/contextmenu';
import { DialogModule } from 'primeng/dialog';
import { DropdownModule } from 'primeng/dropdown';
import { DynamicDialogModule, DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { FileUploadModule } from 'primeng/fileupload';
import { ImageModule } from 'primeng/image';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextModule } from 'primeng/inputtext';
import { KeyFilterModule } from 'primeng/keyfilter';
import { MenubarModule } from 'primeng/menubar';
import { MessagesModule } from 'primeng/messages';
import { MultiSelectModule } from 'primeng/multiselect';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { SidebarModule } from 'primeng/sidebar';
import { TableModule } from 'primeng/table';
import { TieredMenuModule } from 'primeng/tieredmenu';
import { ToastModule } from 'primeng/toast';
import { TooltipModule } from 'primeng/tooltip';
import { ApiService } from '../../Services/api.service';
import { DatabaseCreateComponent } from '../database-create/database-create.component';
import { DatabaseTablesComponent } from '../database-tables/database-tables.component';

@Component({
  selector: 'app-database-list',
  standalone: true,
  imports: [ConfirmDialogModule,ButtonModule, DialogModule, DynamicDialogModule, MessagesModule,CommonModule,TableModule,MultiSelectModule,TieredMenuModule,MenubarModule,
    DropdownModule,InputNumberModule,InputTextModule,ImageModule,CheckboxModule,FormsModule,FileUploadModule,ProgressSpinnerModule,KeyFilterModule,SidebarModule,TooltipModule,
    ContextMenuModule,ToastModule,CardModule
  ],
  providers: [DialogService,MessageService,ConfirmationService],
  templateUrl: './database-list.component.html',
  styleUrl: './database-list.component.css'
})
export class DatabaseListComponent implements OnInit {


  constructor(private confirmationService: ConfirmationService,public apiService: ApiService, protected changeDetector: ChangeDetectorRef, protected dialogService: DialogService,public messageService: MessageService) 
  {

  }
  public loading: boolean = false;
  ref: DynamicDialogRef | undefined;
  public count : number = 0;
  public databases: any[] = [];
  public selectedColumns : {field : string, header: string, type: string, option: any[]}[] = [
    { field: 'databaseName', header: 'DatabaseName', type: 'text', option: [] },
    { field: 'connectionString', header: 'ConnectionString', type: 'text', option: [] },
    { field: 'public', header: 'Public', type: 'boolean', option: [] },
    { field: 'databaseType', header: 'DatabaseType', type: 'option', option: ['SQL'] },
  ];


  ngOnInit() {
    this.Refresh();
  }

   Refresh() {
    this.loading = true;
    this.apiService.GetDatabases().subscribe((data: any) => {
      this.databases = data;
      this.count = this.databases.length;
      this.loading = false;
    });
  }

  CreateNew() {
    this.ref = this.dialogService.open(DatabaseCreateComponent,  { data: {dataRec: null, createmode : true}, header: 'Create new database connection', resizable: true});
    this.ref.onClose.subscribe(x => {
      this.Refresh();
    });
  }

  Edit(row : any){
    this.ref = this.dialogService.open(DatabaseCreateComponent,  { data: {dataRec: row, createmode : false}, header: 'Edit database connection', resizable: true});
    this.ref.onClose.subscribe(x => {
      this.Refresh();
    });
  }

}
