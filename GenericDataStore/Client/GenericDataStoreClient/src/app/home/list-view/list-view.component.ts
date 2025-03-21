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
import { Title, Meta } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';



@Component({
  selector: 'app-list-view',
  standalone: true,
  imports: [DataViewModule,CardModule,TagModule,ButtonModule,CommonModule,ProgressSpinnerModule,InputTextModule, TabViewModule, FormsModule],
  templateUrl: './list-view.component.html',
  styleUrl: './list-view.component.css'
})
export class ListViewComponent implements OnInit{
  constructor(private titleService: Title,private meta: Meta,protected route: ActivatedRoute,public apiService: ApiService, protected router: Router) 
  {
  }

  public loading: boolean = false;

  ngOnInit(): void {
    this.titleService.setTitle("GenericDataStore - Online NoSQL Database Solution"); 
        this.meta.updateTag({ name: 'description', content: "List view" });
    this.meta.updateTag({ name: 'keywords', content: "list view" });
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

      loading1 = false;
      if(!loading1 && !loading2){
        this.loading = false;
      }
    });

    this.apiService.GetAllPublicDashboard().subscribe(x => {
      this.dashboards = x;

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

  GetStyle(item : any){
    if(item?.promoted){
      if(item.color){
        return { "background-image" : "linear-gradient(0.15turn,"+  item.color + ", #ffffff)" , "margin" : "0px"};
      }
    }
    return {  "margin" : "0px"};
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
