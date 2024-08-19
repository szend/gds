import { ChangeDetectorRef, Component, Input, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfirmationService, MessageService } from 'primeng/api';
import { DynamicDialogRef, DialogService, DynamicDialogConfig, DynamicDialogModule } from 'primeng/dynamicdialog';
import { FileUpload, FileUploadModule } from 'primeng/fileupload';
import { OverlayPanel } from 'primeng/overlaypanel';
import { Table, TableModule } from 'primeng/table';
import { RootFilter } from '../../Models/Parameters';
import { ApiService } from '../../Services/api.service';
import { Observable } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { CheckboxModule } from 'primeng/checkbox';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ContextMenuModule } from 'primeng/contextmenu';
import { DialogModule } from 'primeng/dialog';
import { DropdownModule } from 'primeng/dropdown';
import { ImageModule } from 'primeng/image';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextModule } from 'primeng/inputtext';
import { KeyFilterModule } from 'primeng/keyfilter';
import { MenubarModule } from 'primeng/menubar';
import { MessagesModule } from 'primeng/messages';
import { MultiSelectModule } from 'primeng/multiselect';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { SidebarModule } from 'primeng/sidebar';
import { TieredMenuModule } from 'primeng/tieredmenu';
import { ToastModule } from 'primeng/toast';
import { TooltipModule } from 'primeng/tooltip';

@Component({
  selector: 'app-dataobject-parentselect',
  standalone: true,
  imports: [ConfirmDialogModule,ButtonModule, DialogModule, DynamicDialogModule, MessagesModule,CommonModule,TableModule,MultiSelectModule,TieredMenuModule,MenubarModule,
    DropdownModule,InputNumberModule,InputTextModule,ImageModule,CheckboxModule,FormsModule,FileUploadModule,ProgressSpinnerModule,KeyFilterModule,SidebarModule,TooltipModule,
    ContextMenuModule,ToastModule,CardModule
  ],
  providers: [MessageService],
  templateUrl: './dataobject-parentselect.component.html',
  styleUrl: './dataobject-parentselect.component.css'
})
export class DataobjectParentselectComponent {

  @Input() name: string | undefined | null;
  @Input() id: string | undefined | null;
  @Input() child: boolean | undefined | null;
  @Input() private: boolean | undefined | null;
  @Input() parentdataid: string | undefined | null;
  @Input() editable: boolean | undefined | null;
  @Input() parenttypeid: string | undefined;

  @ViewChild('overlayPanel', {static: false}) overlayPanel: OverlayPanel | undefined;
  @ViewChild('table', {static: false}) table: Table | undefined;
  @ViewChild('fileUploader', { static: false }) fileUploader: FileUpload | undefined;


  public data: any = {denyAdd : true, denyExport: true, denyChart: true, allUserFullAccess: false, noFilterMenu: false, field: [], dataObject: [], count: 0, objectTypeId: ""};
  public defaultcount : number = 100;
  public fields : {field : string, header: string, type: string, option: any[]}[] = [];
  public selectedColumns : {field : string, header: string, type: string, option: any[]}[] = [];
  public originalfields : {field : string, header: string, type: string, option: any[]}[] = [];

  public rowData :any[] = [];
  public count: number = 0;
  public imgdata: {data:any, field:string, obj: string}[] = [];
  public admin: boolean = false;
  public bgch: boolean = false;
  public loading: boolean = false;
  public settingVisible: boolean = false;
  public aiVisible: boolean = false;
  selectedProduct!: any;
  public filterMetu : {items: any[], label: string, icon: string}[] = [];
  public rootFilter : RootFilter = { valueFilters: [], valueSortingParams: [], filters: [], logic: 'and', sortingParams: [], skip: 0, take:0, valueSkip: 0, valueTake:0};

  constructor( private ref: DynamicDialogRef,protected config: DynamicDialogConfig,public apiService: ApiService,protected route: ActivatedRoute, protected router: Router, protected changeDetector: ChangeDetectorRef,public messageService: MessageService) 
  {
  }

  ngOnInit() {
    this.loading = true;
    this.rootFilter.valueTake = this.defaultcount;
    this.rootFilter.valueSkip = 0;
    this.Refresh();
  }

   Refresh(full: boolean = true) {
    this.loading = true;
    if(full){
      this.rootFilter.filters = [];
      this.rootFilter.valueFilters = [];
      this.rootFilter.valueSortingParams = [];
      this.rootFilter.valueTake = this.defaultcount;
      this.rootFilter.valueSkip = 0;
      this.table?.clear();
     }
    
  
        if(full){
          this.loading = true;
          this.rootFilter.filters = [];
          this.rootFilter.valueFilters = [];
          this.rootFilter.valueSortingParams = [];
          this.rootFilter.valueTake = this.defaultcount;
          this.rootFilter.valueSkip = 0;
          this.table?.clear();
    
         }
         if(this.config.data.id){
          this.id = this.config.data.id;
        }
        if(this.config.data.name){
          this.name = this.config.data.name;
        }
        if(this.config.data.private){
          this.private = this.config.data.private;
        }
         this.InitData(this.private ?? false);
         
 
  }

  InitData(privatelist: boolean){
    this.rootFilter.filters.push({ field: 'Name', operator: 'equals', value: this.name });
    this.rootFilter.filters.push({ field: 'ObjectTypeId', operator: 'equals', value: this.id });

    this.apiService.GetTypeByFilter(this.rootFilter,privatelist).subscribe(x =>{
      if(x.length == 0){
        this.loading = false;
      }
     this.data = x[0];
     this.count = x[0].count;
     if(!this.data) {
       this.router.navigateByUrl('home');
     }

     this.apiService.ListOwner(this.data.objectTypeId).subscribe(y => {
       this.admin = y;
     });

     this.fields =((this.data.field as object as any[]).map(y => {
       return{field: y.name, header: y.name, type: y.type, option: y.option}
     }));
     this.selectedColumns = this.fields;
     this.originalfields =((this.data.field as object as any[]).map(y => {
      return{field: y.name, header: y.name, type: y.type, option: y.option}
    }));
     this.rowData = this.data.dataObject;
     this.apiService.GetObjectAccess().subscribe(x =>{
      this.rowData.forEach(item => {
        item.editable = this.admin;
       });
    });
    
     this.filterMetu = [];
     this.imgdata = [];
     this.fields.forEach(x => {
       if(x.type == 'option'){
         let mainitem : {items: any[], label: string, icon: string} = {
           label: x.header,
           icon: 'pi pi-ellipsis-v',
           items: []
         }
         x.option.forEach(y =>{
           let childitem = {
             label: y.optionName,
             icon: 'pi pi-circle',
             command: () => this.FilterOption(x,y),
           }
           mainitem.items.push(childitem)
         })
         this.filterMetu.push(mainitem);
       }
       if(x.type == 'image'){
         this.rowData.forEach(y => {
          this.getImage(x,y.value)?.subscribe(z => {
            if(z.size > 0){
              this.imgdata.push({ data: URL.createObjectURL(z), field: x.header, obj: y.value.find((x : any) => x.name == this.fields.find(y => y.type == "id")?.header).valueString});
            }
          },e => {

          });

        });
      }
       
     })
     this.loading = false;   
    }, () => {
      this.loading = false;
    },);

  //   this.apiService.GetCount(this.rootFilter).subscribe(x => {
  //    this.count = x;
  //  });
  }

  Select(row:any){
   let res = row.value.find((x : any) => x.name == this.fields.find(y => y.type == "id")?.header).valueString;
    this.ref.close(res);
  }

  FilterOption(field : any, option: any){
    this.rootFilter.valueFilters = [];
    this.rootFilter.valueFilters?.push( { field: field.field, operator: 'equals', value: option.optionName });
    this.Refresh(false);
  }


  getValue(data: any[], col: any) : any{
    let res = data.find(x => x.name == col.field);
    return res?.valueString ?? "";
  }

  getImage(col : any, data: any[]) : Observable<any> | undefined{
    let res = data.find(x => x.name == col.field);
    if(col.type == 'image' && res.valueString != null && res.valueString != undefined && res.valueString != ""){
    return  this.apiService.DownloadFile(res.valueString);
    }
    return undefined;
  }

  getImageObj(row: any, field: string){
    let res = this.imgdata.find(x => x.obj == row.value.find((y: any) => y.name == this.fields.find(z => z.type == "id")?.header).valueString && x.field == field);
    if(res?.data){
      return res?.data;

    }
    return undefined;

  }

  onSort(event : any){
    this.rootFilter.valueSortingParams = [];
    event.type = this.fields.find(x => x.field == event.field)?.type;
    this.rootFilter.valueSortingParams.push(event);
    if(false){
      this.data.dataObject.sort((a: any, b: any) => {
        let valueA = a.value.find((x: { name: any; }) => x.name == event.field)?.valueString;
        let valueB = b.value.find((x: { name: any; }) => x.name == event.field)?.valueString;
        if(valueA == null && valueB == null){
          return 0;
        }
        if(event.order == 1){
          if(valueA == null || valueA == undefined){
            return -1;
          }
          if(event.type == 'numeric' || event.type == 'calculatednumeric'){
            return Number(valueA) - Number(valueB);
          }
          else{
            return valueA?.localeCompare(valueB);
          }
        }
        else{
          if(valueB == null || valueB == undefined){
            return -1;
          }
          if(event.type == 'numeric' || event.type == 'calculatednumeric'){
            return Number(valueB) - Number(valueA);
          }
          else{
            return valueB?.localeCompare(valueA);
          }
        }
      });
    }
    else{
   this.Refresh(false);
    }
}

  onFilter(event : any) {
    this.rootFilter.valueTake = this.defaultcount;
    this.rootFilter.valueSkip = 0;

      this.rootFilter.valueFilters = [];
      if (event) {
        let filterCnt = 0;
        Object.keys(event.filters).forEach(colName => {
          let filter = event.filters[colName];
          if (filter != null && filter[0].value != null) {
            this.rootFilter.logic = filter[0].operator;
            filter.forEach((f: { matchMode: any; value: any; })  => {
              this.rootFilter.valueFilters?.push( { field: colName, operator: f.matchMode, value: f.value });
              filterCnt++;
            })
          }
        });
      }

      if(false){
        
        this.data.dataObject = this.data.dataObject.filter((x: any) => {
          let res = true;
          this.rootFilter.valueFilters?.forEach(f => {
            let value = x.value.find((y: { name: any; }) => y.name == f.field)?.valueString;

            if(value){
              if(f.operator == 'contains'){
                res = res && value.includes(f.value);
              }
              if(f.operator == 'equals'){
                res = res && value == f.value;
              }
              if(f.operator == 'startsWith'){
                res = res && value.startsWith(f.value);
              }
              if(f.operator == 'endsWith'){
                res = res && value.endsWith(f.value);
              }
            }
            else{
              res = false;
            }
          });
          return res;
        });
      }
      else{
        this.Refresh(false);
      }   

}

onPageChange(event : any) {
  this.rootFilter.valueTake = event.rows;
  this.defaultcount = event.rows;
  this.rootFilter.valueSkip = event.first;
  this.Refresh(false);
}

filterNumber(event: any, field: string){
  this.rootFilter.valueFilters?.push( { field: field, operator: "and", value: event.srcElement.value });
}

}
