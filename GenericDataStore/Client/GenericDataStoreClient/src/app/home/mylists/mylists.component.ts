import { Component, Input, OnInit } from '@angular/core';
import { CardModule } from 'primeng/card';
import { DataViewModule } from 'primeng/dataview';
import { TagModule } from 'primeng/tag';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { ApiService } from '../../Services/api.service';
import { ActivatedRoute, Router } from '@angular/router';
import { RootFilter } from '../../Models/Parameters';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { Title, Meta } from '@angular/platform-browser';

@Component({
  selector: 'app-mylists',
  standalone: true,
  imports: [CardModule,ButtonModule,TagModule,CommonModule,DataViewModule,ProgressSpinnerModule],
  templateUrl: './mylists.component.html',
  styleUrl: './mylists.component.css'
})
export class MylistsComponent implements OnInit {
  constructor(private titleService: Title,private meta: Meta,protected route: ActivatedRoute,public apiService: ApiService, protected router: Router) 
  {
  }
  @Input()
  types : any[] = [];
  public loading: boolean = false;

  
  ngOnInit(): void {
    this.titleService.setTitle("Mylists"); 
    this.meta.updateTag({ name: 'description', content: "Mylists" });
    this.meta.updateTag({ name: 'keywords', content: "Mylists" }); 
   this.Refresh();
  }

  public Refresh(){
    this.loading = true;

    this.apiService.GetTypeByFilter(undefined,true).subscribe(x => {
      this.types = x;
      this.loading = false;
    });
  }


  GoToType(name : string, id: string, privatemode : boolean){
    if(privatemode == true){
      this.router.navigateByUrl('/private/'+ id +'/'+ name);
    }
    else{
      this.router.navigateByUrl('/'+ id +'/'+ name);
    }
  }


}
