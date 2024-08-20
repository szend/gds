import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Router } from 'express';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { DataViewModule } from 'primeng/dataview';
import { TagModule } from 'primeng/tag';
import { ApiService } from '../../Services/api.service';
import { DialogService, DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { FormsModule } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { ToastModule } from 'primeng/toast';
import { MessagesModule } from 'primeng/messages';
import { MessageService } from 'primeng/api';
import { PageCreateComponent } from '../page-create/page-create.component';

@Component({
  selector: 'app-page-edit',
  standalone: true,
  imports: [DataViewModule,CardModule,TagModule,ButtonModule,CommonModule, FormsModule, InputTextModule,ToastModule,MessagesModule],
  providers: [MessageService,DialogService],
  templateUrl: './page-edit.component.html',
  styleUrl: './page-edit.component.css'
})
export class PageEditComponent implements OnInit{
  constructor(public apiService: ApiService,  protected config: DynamicDialogConfig,private messageService: MessageService, protected dialogService: DialogService) 
  {
  }

  id : string | undefined;
  pages : any[] = [];
  ref: DynamicDialogRef | undefined;


  ngOnInit(): void {
   this.Refresh();
  }

  public Refresh(){
    if(this.config.data.id){
      this.id = this.config.data.id;
    }

    if(this.id){
      this.apiService.GetAllPage(this.id).subscribe(x => {
       this.pages = x;
      });
    }

  }

  AddParam(page : any){
    if(!page.parameters){
      page.parameters = [];
    }
    page.parameters.push('');
  }
  RemoveParam(page : { parameters: any[]; }){
    page.parameters.pop();

  }

  Save(page : any){
    this.apiService.EditPage(page).subscribe(x => {
      this.messageService.add({severity:'success', summary:'Success', detail:'Page Updated'});

    },error => {
      this.messageService.add({severity:'error', summary:'Error', detail:'Error Updating Page'});

  });
  }

  Delete(page : any){
    this.apiService.DeletePage(page.tablePageId).subscribe(x => {
      this.messageService.add({severity:'success', summary:'Success', detail:'Page Deleted'});
      this.Refresh();

    },error => {
      this.messageService.add({severity:'error', summary:'Error', detail:'Error Deleting Page'});
    });
  }

  CreatePage(){
    this.ref = this.dialogService.open(PageCreateComponent,  { data: {id: this.id}, header: 'Create new page', resizable: true});
    this.ref.onClose.subscribe(x => {
      this.Refresh();
    });
  }

  onReject() {
    this.messageService.clear();
  }

}
