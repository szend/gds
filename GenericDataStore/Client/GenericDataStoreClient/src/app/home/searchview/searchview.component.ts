import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '../../Services/api.service';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { DataViewModule } from 'primeng/dataview';
import { TagModule } from 'primeng/tag';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { TabViewModule } from 'primeng/tabview';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { DataobjectEditComponent } from '../../Components/dataobject-edit/dataobject-edit.component';
import { FooterComponent } from '../footer/footer.component';
import { Title, Meta } from '@angular/platform-browser';


@Component({
  selector: 'app-searchview',
  standalone: true,
  imports: [DataViewModule,CardModule,TagModule,ButtonModule,CommonModule,ProgressSpinnerModule,TabViewModule,FooterComponent],
  providers: [DialogService],
  templateUrl: './searchview.component.html',
  styleUrl: './searchview.component.css'
})
export class SearchviewComponent implements OnInit{
  constructor(private titleService: Title,private meta: Meta,protected route: ActivatedRoute,public apiService: ApiService, protected router: Router, protected dialogService: DialogService) 
  {
  }

  searchstring : string | null = '';
  lists : any[] = [];
  values : any[] = [];
  public searchloading: boolean = false;
  ref: DynamicDialogRef | undefined;

  ngOnInit(): void {
    this.titleService.setTitle("GenericDataStore"); 
    this.meta.updateTag({ name: 'description', content: "Search" });
    this.meta.updateTag({ name: 'keywords', content: "Search, data, find" });
    this.route.paramMap.subscribe(params => {     
      this.searchstring = params.get("searchstring");
      this.Refresh();
    });


  // this.Refresh();
  }

  Refresh(){
    this.searchloading = true;
    this.apiService.Search(this.searchstring ?? '').subscribe(x => {
      this.lists = x.lists;
      this.values = x.values;
      this.searchloading = false;
    },e => {
      this.searchloading = false;
    
    });
  }

  GoToType(name : string, id: string){
    this.router.navigateByUrl('/'+ id +'/'+ name);
  }

  OpenObject(id : string){
    this.apiService.GetListByObjectId(id).subscribe(x => {
      if(x[0]){
        x[0].field = x[0].field.map((y: {fieldId : string, name: string, type: string, objectTypeId : string}) => { return { fieldId: y.fieldId, field: y.name, type: y.type, objectTypeId: y.objectTypeId }; });
        this.ref = this.dialogService.open(DataobjectEditComponent,  { data: {dataRec: x[0].dataObject[0].value, editable: false,parentid: x[0].objectTypeId, id: x[0].dataObject[0].dataObjectId,fields: x[0].field}, header: 'List name:  '+x[0].name, resizable: true});
        this.ref.onClose.subscribe(x => {
          //this.Refresh();
        });
      }

    });
  }

}
