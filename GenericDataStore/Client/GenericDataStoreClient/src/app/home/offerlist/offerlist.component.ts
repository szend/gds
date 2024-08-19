import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { MessagesModule } from 'primeng/messages';
import { TableModule } from 'primeng/table';
import { ApiService } from '../../Services/api.service';

@Component({
  selector: 'app-offerlist',
  standalone: true,
  imports: [FormsModule,ButtonModule,CardModule,TableModule,MessagesModule,CommonModule,FormsModule],
  templateUrl: './offerlist.component.html',
  styleUrl: './offerlist.component.css'
})
export class OfferlistComponent implements OnInit{

  constructor(public apiService: ApiService) 
  {
  }
  offers : any = [];

  ngOnInit() {
    this.apiService.GetOffers().subscribe(x => {
      this.offers = x;
    });
  }
}
