import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '../../Services/api.service';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Title, Meta } from '@angular/platform-browser';

@Component({
  selector: 'app-confirmpayment',
  standalone: true,
  imports: [ProgressSpinnerModule,CommonModule, FormsModule],
  templateUrl: './confirmpayment.component.html',
  styleUrl: './confirmpayment.component.css'
})
export class ConfirmpaymentComponent implements OnInit {

  
  constructor(private titleService: Title,private meta: Meta,public apiService: ApiService,protected route: ActivatedRoute, protected router: Router) 
  {

  }

  loading : boolean = true;
  message : string = "";

  ngOnInit(): void {
    this.titleService.setTitle("Confirm payment"); 

    if(localStorage.getItem('link')){
      localStorage.removeItem('link');

      let intcount = Number.parseInt(this.route.snapshot.paramMap.get('intcount') ?? "0");
      let extcount = Number.parseInt(this.route.snapshot.paramMap.get('extcount') ?? "0");
      let listcount = Number.parseInt(this.route.snapshot.paramMap.get('listcount') ?? "0");
      this.apiService.SetLimit(intcount,extcount,listcount,'xxy93').subscribe(x => {
        this.loading = false;
        this.message = 'Successful subscription.';
      }, e => {
        this.loading = false;
        this.message = 'Something wrong. Contact us if your payment was successful.'
      });

    }
    else{
      this.message = 'Something wrong. Contact us if your payment was successful.'
      this.loading = false;

    }
  }



}
