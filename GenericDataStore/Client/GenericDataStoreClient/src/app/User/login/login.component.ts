import { CommonModule } from '@angular/common';
import { Component, OnInit, ViewChild, ViewEncapsulation, ViewRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { PasswordModule } from 'primeng/password';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { AuthService } from '../../Services/auth.service';
import { Router } from '@angular/router';
import { CardModule } from 'primeng/card';
import { MessageService } from "primeng/api"; 
import { MessagesModule } from 'primeng/messages';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { MenuComponent } from '../../../menu/menu.component';
import { FooterComponent } from '../../home/footer/footer.component';


@Component({
  selector: 'app-login',
  standalone: true,
  imports: [PasswordModule, CommonModule, FormsModule,InputTextModule, ButtonModule,CardModule,ProgressSpinnerModule,MessagesModule, FooterComponent],
  providers: [MessageService], 
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit {
  public email : string | undefined;
  public password : string | undefined;
  public loading : boolean = false;

  constructor(public authService: AuthService,private router: Router,private messageService: MessageService ) 
  {
  }

  ngOnInit() {
    if(!localStorage.getItem("firstload")){
      //location.reload();
      localStorage.setItem("firstload","true");
    }
    else{
      localStorage.removeItem("firstload");
    }
  }

  Login(){
    this.loading = true;
    if(this.email != undefined && this.password != undefined){
      this.authService.login(this.email,this.password).subscribe(x => {
        this.authService.setSession(x);
        localStorage.setItem('username', this.email ?? '');

        this.authService.newlogin.emit(true);
        this.router.navigateByUrl('/home');

        
      },er =>{
        this.messageService.add({ 
          severity: "error", 
          summary: "Error", 
          detail: er.message, 
          life: 300000
        });
        this.loading = false;
      })
    }
    else {
      this.messageService.add({ 
        severity: "error", 
        summary: "Error", 
        detail: 'Name and password cant be empty', 
        life: 300000
      });
    }
    this.loading = false;
    }

  
}
