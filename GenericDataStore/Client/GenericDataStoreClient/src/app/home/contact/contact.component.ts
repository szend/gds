import { Component, OnInit } from '@angular/core';
import { CardModule } from 'primeng/card';
import { FooterComponent } from '../footer/footer.component';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { InputGroupModule } from 'primeng/inputgroup';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { MessagesModule } from 'primeng/messages';
import { ApiService } from '../../Services/api.service';
import { Title, Meta } from '@angular/platform-browser';

@Component({
  selector: 'app-contact',
  standalone: true,
  imports: [CardModule, InputGroupModule, InputTextareaModule, InputNumberModule, InputTextModule, CommonModule, FormsModule, ButtonModule,MessagesModule, FooterComponent],
  providers: [MessageService], 
  templateUrl: './contact.component.html',
  styleUrl: './contact.component.css'
})
export class ContactComponent implements OnInit {

  constructor(private titleService: Title,private meta: Meta,public apiService: ApiService, public messageService: MessageService) 
  {
  }
  ngOnInit(): void {
    this.titleService.setTitle("Contact"); 
    this.meta.updateTag({ name: 'description', content: "Contact" });
    this.meta.updateTag({ name: 'keywords', content: "Contact" }); 
  }

  public name: string = '';
  public email: string = '';
  public comment: string = '';

  Send(){
    if(this.name != '' && this.email != ''){
      let offer = {name : this.name, email : this.email, comment : this.comment};
      this.apiService.SendOffer(offer).subscribe(x => {
        this.messageService.add({severity:'success', summary:'Success', detail:'Offer sent successfully'});
      }, err => {
        this.messageService.add({severity:'error', summary:'Error', detail:'Error sending offer'});
      });
    }
    else{
      this.messageService.add({severity:'error', summary:'Error', detail:'Please fill in all required fields'});
    }
   
  }
}
