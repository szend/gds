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
import { InputTextModule } from 'primeng/inputtext';
import { TabViewModule } from 'primeng/tabview';



@Component({
  selector: 'app-list-view',
  standalone: true,
  imports: [DataViewModule,CardModule,TagModule,ButtonModule,CommonModule,ProgressSpinnerModule,InputTextModule, TabViewModule],
  templateUrl: './list-view.component.html',
  styleUrl: './list-view.component.css'
})
export class ListViewComponent implements OnInit{
  constructor(protected route: ActivatedRoute,public apiService: ApiService, protected router: Router) 
  {
  }

  public loading: boolean = false;

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {     
      this.category = params.get("category");
      this.Refresh();
    });


   this.Refresh();
  }

  public Refresh(){
    this.loading = true;
    if(this.category != undefined && this.category != null && this.rootFilter != undefined){
      this.rootFilter.filters = [{
        value: this.category,
        field: 'category',
        operator: 'equals'
      }];
    }

   var loading1 = true;
    var loading2 = true; 

    this.apiService.GetTypeByFilter(this.rootFilter).subscribe(x => {
      this.types = x;
      console.log(x);
      loading1 = false;
      if(!loading1 && !loading2){
        this.loading = false;
      }
    });

    this.apiService.GetAllPublicDashboard().subscribe(x => {
      this.dashboards = x;
      console.log(x);
      loading2 = false;
      if(!loading1 && !loading2){
        this.loading = false;
      }
      
    });
  }

  rootFilter: RootFilter | undefined = {
    filters: [],
    valueFilters: [],
    logic: 'and',
    sortingParams: [{field: 'promoted', order: 1, type: 'boolean'}],
    valueSortingParams: [], 
    take: 0, 
    skip: 0,
    valueTake: 0,
    valueSkip: 0,
    parentValueFilters: [],
    parentValueSortingParams: []
  };
  @Input() category: string | undefined | null;
  types : any[] = [];
  dashboards : any[] = [];

  GoToType(name : string, id: string){
    this.router.navigateByUrl('/'+ id +'/'+ name);
  }

  GoToDashboard(id : string){
    this.router.navigateByUrl('/publicdashboard/'+id);
  }

  Search(event : any, element : any){
    var str = element.value;
    element.value = "";
    this.router.navigateByUrl('/search/'+str);
  }
}
