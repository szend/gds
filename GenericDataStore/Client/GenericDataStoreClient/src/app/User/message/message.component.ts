import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DataViewModule } from 'primeng/dataview';

import { ApiService } from '../../Services/api.service';
import { TabViewModule } from 'primeng/tabview';
import { ButtonModule } from 'primeng/button';
import { MessageService } from 'primeng/api';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { DataobjectEditComponent } from '../../Components/dataobject-edit/dataobject-edit.component';
import { MessageEditComponent } from '../message-edit/message-edit.component';
import { FieldsetModule } from 'primeng/fieldset';
import { Title, Meta } from '@angular/platform-browser';


@Component({
  selector: 'app-message',
  standalone: true,
  imports: [DataViewModule,FormsModule,CommonModule,TabViewModule,ButtonModule,FieldsetModule],
  providers: [DialogService,MessageService],
  templateUrl: './message.component.html',
  styleUrl: './message.component.css'
})
export class MessageComponent implements OnInit {
  constructor( private titleService: Title,private meta: Meta,public apiService: ApiService, protected changeDetector: ChangeDetectorRef, protected dialogService: DialogService,private messageService: MessageService) 
  {
  }

  public rec : any[] = [];
  public sen : any[] = [];
  ref: DynamicDialogRef | undefined;


  ngOnInit(): void {
    this.titleService.setTitle("Messages"); 
    this.meta.updateTag({ name: 'description', content: "Messages" });
    this.meta.updateTag({ name: 'keywords', content: "Messages" });  
    this.Refresh();
  }

  Refresh(){
    this.apiService.GetMessages().subscribe((x: any[]) => {
      this.rec = x.filter(x => x.receiverMail == localStorage.getItem('username'));
      this.sen = x.filter(x => x.senderMail == localStorage.getItem('username'));
    })
  }

  DeleteMessage(id: string){
    this.apiService.DeleteMessage(id).subscribe(x => {
      this.Refresh();
    })
  }

  CreateMessage(message: any,chs = false){
    let recid= message.sendUserId;
    if(chs){
      recid= message.receivUserId;

    }

    this.ref = this.dialogService.open(MessageEditComponent,  {  data:  {lastmessageid: message.userMessageId, receiverid: recid}, header: 'Reply', resizable: true});
    this.ref.onClose.subscribe(x => {
      this.Refresh();
    });
  }
}
