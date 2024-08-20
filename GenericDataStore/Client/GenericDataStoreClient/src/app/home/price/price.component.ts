import { Component } from '@angular/core';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TabViewModule } from 'primeng/tabview';
import { DataViewModule } from 'primeng/dataview';
import { Router } from '@angular/router';

@Component({
  selector: 'app-price',
  standalone: true,
  imports: [CardModule,ButtonModule,TabViewModule,DataViewModule],
  templateUrl: './price.component.html',
  styleUrl: './price.component.css'
})
export class PriceComponent {

  constructor(protected router: Router) 
  {

  }

  Select(link : string){
    localStorage.setItem('link',link)
  
    window.open(link, '_blank');
  }

  Contact(){
    this.router.navigateByUrl("/contact")
  }

}
