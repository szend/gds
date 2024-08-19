import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { DynamicDialogConfig, DialogService } from 'primeng/dynamicdialog';
import { ApiService } from '../../Services/api.service';
import { MessagesModule } from 'primeng/messages';
import { InputTextModule } from 'primeng/inputtext';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InputTextareaModule } from 'primeng/inputtextarea';

@Component({
  selector: 'app-message-edit',
  standalone: true,
  imports: [ButtonModule,MessagesModule,InputTextModule,CommonModule,FormsModule,InputTextareaModule],
  providers: [MessageService],
  templateUrl: './message-edit.component.html',
  styleUrl: './message-edit.component.css'
})
export class MessageEditComponent implements OnInit {

  constructor( public apiService: ApiService, protected changeDetector: ChangeDetectorRef,  protected config: DynamicDialogConfig,private messageService: MessageService
    , protected dialogService: DialogService
  ) 
  {
  }

  public id: string | undefined;
  public message: string | undefined;
  public lastmessageid: string | undefined;

  ngOnInit(): void {
    if(this.config.data.id){
      this.id = this.config.data.id;
    }
    if(this.config.data.lastmessageid){
      this.lastmessageid = this.config.data.lastmessageid;
    }
  }

  SendMessage(){
    let obj = {
      comment:this.message,
      objectTypeId: this.id,
      lastMessageId: this.lastmessageid
    }
    this.apiService.CreateMessage(obj).subscribe(x => {
      this.messageService.add({ 
        severity: "success", 
        summary: "Ok:", 
        detail: "Message Sent", 
        life: 3000
      });
    }, err => {
      this.messageService.add({ 
        severity: "error", 
        summary: "Error:", 
        detail: "Message not Sent", 
        life: 3000
      });
    });
  }
}
