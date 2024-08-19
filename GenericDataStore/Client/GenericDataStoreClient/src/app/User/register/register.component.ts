import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ApiService } from '../../Services/api.service';
import { CardModule } from 'primeng/card';
import { MessageService } from "primeng/api"; 
import { MessagesModule } from 'primeng/messages';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [PasswordModule, CommonModule, FormsModule,InputTextModule, ButtonModule,CardModule,ProgressSpinnerModule,MessagesModule],
  providers: [MessageService], 
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  public username : string | undefined;
  public password : string | undefined;
  public password2 : string | undefined;
  public email : string | undefined;
  
  constructor(public apiService: ApiService,private router: Router,private messageService: MessageService ) 
  {
  }

  Register(){
    if(this.password != this.password2){
      this.messageService.add({ 
        severity: "error", 
        summary: "Error", 
        detail: 'Passwords not match', 
        life: 300000
      });
      return;
    }
    if(this.username != undefined && this.password != undefined && this.email != undefined){
      this.apiService.CreateUser(this.username,this.password,this.email).subscribe(x => {
        this.router.navigateByUrl('/home');
      }, error => {
        
        error.error.forEach((element: any) => {
          this.messageService.add({ 
            severity: "error", 
            summary: "Error",
            detail: element.description,
            life: 300000
          });
        }
        );
      })
    }
    else {
      this.messageService.add({ 
        severity: "error", 
        summary: "Error", 
        detail: 'Empty field', 
        life: 300000
      });
    }
  }
}
