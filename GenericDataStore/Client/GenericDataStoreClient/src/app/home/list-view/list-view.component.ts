import { Component, Input, OnInit } from '@angular/core';
import { CardModule } from 'primeng/card';
import { DataViewModule } from 'primeng/dataview';
import { TagModule } from 'primeng/tag';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { ApiService } from '../../Services/api.service';
import { ActivatedRoute, Router } from '@angular/router';
import { RootFilter } from '../../Models/Parameters';

@Component({
  selector: 'app-list-view',
  standalone: true,
  imports: [DataViewModule,CardModule,TagModule,ButtonModule,CommonModule],
  templateUrl: './list-view.component.html',
  styleUrl: './list-view.component.css'
})
export class ListViewComponent implements OnInit{
  constructor(protected route: ActivatedRoute,public apiService: ApiService, protected router: Router) 
  {
  }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {     
      this.category = params.get("category");
      this.Refresh();
    });


   this.Refresh();
  }

  public Refresh(){
    if(this.category != undefined && this.category != null && this.rootFilter != undefined){
      this.rootFilter.filters = [{
        value: this.category,
        field: 'category',
        operator: 'equals'
      }];
    }

    this.apiService.GetTypeByFilter(this.rootFilter).subscribe(x => {
      this.types = x;
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
    valueSkip: 0
  };
  @Input() category: string | undefined | null;
  types : any[] = [];

  GoToType(name : string, id: string){
    this.router.navigateByUrl('/'+ id +'/'+ name);
  }
}
