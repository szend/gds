import { Component } from '@angular/core';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TabViewModule } from 'primeng/tabview';
import { DataViewModule } from 'primeng/dataview';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FooterComponent } from '../footer/footer.component';
import { ApiService } from '../../Services/api.service';

@Component({
  selector: 'app-price',
  standalone: true,
  imports: [CardModule,ButtonModule,TabViewModule,DataViewModule, CommonModule, FormsModule,FooterComponent],
  templateUrl: './price.component.html',
  styleUrl: './price.component.css'
})
export class PriceComponent {

  constructor(protected router: Router,public apiService: ApiService) 
  {

  }

  options : string = "intro";

  SelectOption(opt : string){
    this.options = opt;
  }

  Free(){
    this.apiService.SetLimit(0,10000,10,"xxy93").subscribe( x => {
      this.router.navigateByUrl("/connection/databases")
    });
  }

  Select(link : string){
    console.log(link);
    localStorage.setItem('link',link)
  
    window.open(link, '_blank');
  }

  Contact(){
    this.router.navigateByUrl("/contact")
  }

}
