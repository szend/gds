import { EventEmitter, Injectable, Output } from '@angular/core';

import moment from "moment";
import { ApiService } from './api.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  @Output() newlogin= new EventEmitter<boolean>(); 
  constructor(public apiService: ApiService) {

  }

  login(name:string, password:string ) {
      return this.apiService.GetToken(name, password);
  }
        
  public setSession(authResult : any) {
      const expiresAt = moment().add(authResult.expiration);
      localStorage.setItem('id_token', authResult.token);
      localStorage.setItem("expires_at",authResult.expiration );
  }          

  logout() {
    
      localStorage.removeItem("id_token");
      localStorage.removeItem("expires_at");
      localStorage.removeItem("username");
      this.newlogin.emit(false);
  }

  public isLoggedIn() {
    let exp = this.getExpiration();
    if(exp == null || exp == undefined) {
      return false;
    }
      return moment().isBefore(this.getExpiration());
  }

  isLoggedOut() {
      return !this.isLoggedIn();
  }

  getExpiration() {
      const expiration = localStorage.getItem("expires_at");
      if(expiration != undefined && expiration != null){
        return moment(expiration);
      }
      return undefined;
  }    
}
