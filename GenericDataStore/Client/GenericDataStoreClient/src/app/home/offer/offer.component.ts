import { Component } from '@angular/core';
import { CardModule } from 'primeng/card';
import { InputGroupModule } from 'primeng/inputgroup';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextModule } from 'primeng/inputtext';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { ApiService } from '../../Services/api.service';
import { MessageService } from 'primeng/api';
import { MessagesModule } from 'primeng/messages';

@Component({
  selector: 'app-offer',
  standalone: true,
  imports: [CardModule, InputGroupModule, InputTextareaModule, InputNumberModule, InputTextModule, CommonModule, FormsModule, ButtonModule,MessagesModule],
  providers: [MessageService], 
  templateUrl: './offer.component.html',
  styleUrl: './offer.component.css'
})
export class OfferComponent {

  constructor(public apiService: ApiService, public messageService: MessageService) 
  {
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
