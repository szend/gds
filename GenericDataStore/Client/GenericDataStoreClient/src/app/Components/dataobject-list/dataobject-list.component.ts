import { CommonModule } from '@angular/common';
import { AfterViewInit, ChangeDetectorRef, Component, Input, OnInit, ViewChild, booleanAttribute } from '@angular/core';
import { MessageService, ConfirmationService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { DynamicDialogModule, DialogService, DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { MessagesModule } from 'primeng/messages';
import { Table, TableModule } from 'primeng/table';
import { ApiService } from '../../Services/api.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Filter, RootFilter } from '../../Models/Parameters';
import { DataobjectCreateComponent } from '../dataobject-create/dataobject-create.component';
import { ObjecttypeCreateComponent } from '../objecttype-create/objecttype-create.component';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { MultiSelectModule } from 'primeng/multiselect';
import { DataobjectEditComponent } from '../dataobject-edit/dataobject-edit.component';
import { DropdownModule } from 'primeng/dropdown';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextModule } from 'primeng/inputtext';
import { TieredMenuModule } from 'primeng/tieredmenu';
import { MenubarModule } from 'primeng/menubar';
import { OverlayPanel, OverlayPanelModule } from 'primeng/overlaypanel';
import { ImageModule } from 'primeng/image';
import { Observable } from 'rxjs';
import { CheckboxModule } from 'primeng/checkbox';
import { FormsModule } from '@angular/forms';
import { ChartComponent } from '../chart/chart.component';
import * as XLSX from 'xlsx';
import { FileUpload, FileUploadModule } from 'primeng/fileupload';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { CalculatedfieldComponent } from '../calculatedfield/calculatedfield.component';
import { ClustersComponent } from '../clusters/clusters.component';
import { KeyFilterModule } from 'primeng/keyfilter';
import { ImageClassificationComponent } from '../image-classification/image-classification.component';
import { SidebarModule } from 'primeng/sidebar';
import { TooltipModule } from 'primeng/tooltip';
import { RegressionComponent } from '../regression/regression.component';
import { ClassificationComponent } from '../classification/classification.component';
import { ContextMenuModule } from 'primeng/contextmenu';
import {ToastModule} from 'primeng/toast';
import { CardModule } from 'primeng/card';
import { ObjecttypeSelectComponent } from '../objecttype-select/objecttype-select.component';
import { PageCreateComponent } from '../page-create/page-create.component';
import { PageEditComponent } from '../page-edit/page-edit.component';
import { CalculatedColorComponent } from '../calculated-color/calculated-color.component';
import { MessageEditComponent } from '../../User/message-edit/message-edit.component';
import { Output, EventEmitter } from '@angular/core';
import { InputSwitchModule } from 'primeng/inputswitch';
import { Meta, Title } from '@angular/platform-browser';



@Component({
  selector: 'app-dataobject-list',
  standalone: true,
  imports: [ConfirmDialogModule,ButtonModule, DialogModule, DynamicDialogModule, MessagesModule,CommonModule,TableModule,MultiSelectModule,TieredMenuModule,MenubarModule,
    DropdownModule,InputNumberModule,InputTextModule,ImageModule,CheckboxModule,FormsModule,FileUploadModule,ProgressSpinnerModule,KeyFilterModule,SidebarModule,TooltipModule,
    ContextMenuModule,ToastModule,CardModule,InputSwitchModule
  ],
  providers: [DialogService,MessageService,ConfirmationService],
  templateUrl: './dataobject-list.component.html',
  styleUrl: './dataobject-list.component.css',
})
export class DataobjectListComponent  implements OnInit, AfterViewInit {

  @Output() rootFilterData = new EventEmitter<RootFilter>();
  @Output() chartrefreshdata = new EventEmitter<any>();

  @Output() dataclick = new EventEmitter();
  DataClick(){
    this.dataclick.emit();
  }

  @Input() dashboardmode: boolean | undefined | null;
  @Input() dashboardata: any | undefined | null;
  @Input() dashboarid: string | undefined | null;
  @Input() dashboardfilter: string | undefined | null;
  @Input() charts: any[] | undefined | null;


  @Input() name: string | undefined | null;
  @Input() id: string | undefined | null;
  @Input() child: boolean | undefined | null;
  @Input() private: boolean | undefined | null;
  @Input() parentdataid: string | undefined | null;
  @Input() editable: boolean | undefined | null;
  @Input() parenttypeid: string | undefined;

  @Input() parentrow: number | undefined;
  @Input() parentfilter: RootFilter | undefined;

  @ViewChild('overlayPanel', {static: false}) overlayPanel: OverlayPanel | undefined;
  @ViewChild('table', {static: false}) table: Table | undefined;
  @ViewChild('fileUploader', { static: false }) fileUploader: FileUpload | undefined;


  public data: any = {denyAdd : true, denyExport: true, denyChart: true, allUserFullAccess: false, noFilterMenu: false, field: [], dataObject: [], count: 0, objectTypeId: ""};
  public defaultcount : number = 25;
  public fields : any[] = [];
  public selectedColumns : any[] = [];
  public originalfields : any[] = [];

  public quickedit : boolean = false;
  public rowData :any[] = [];
  ref: DynamicDialogRef | undefined;
  public count: number = 0;
  public imgdata: {data:any, field:string, obj: string}[] = [];
  public admin: boolean = false;
  public bgch: boolean = false;
  public loading: boolean = false;
  public settingVisible: boolean = false;
  public aiVisible: boolean = false;
  selectedProduct!: any;
  public filterMetu : {items: any[], label: string, icon: string}[] = [];
  public rootFilter : RootFilter = {parentValueSortingParams: [],parentValueFilters: [], valueFilters: [], valueSortingParams: [], filters: [], logic: 'and', sortingParams: [], skip: 0, take:0, valueSkip: 0, valueTake:0};

  contextitems = [
    { label: 'View', icon: 'pi pi-fw pi-search', command: () => this.Edit(this.selectedProduct) },   
];


  constructor(private titleService: Title,private meta: Meta,private confirmationService: ConfirmationService,public apiService: ApiService,protected route: ActivatedRoute, protected router: Router, protected changeDetector: ChangeDetectorRef, protected dialogService: DialogService,public messageService: MessageService) 
  {

  }
  ngAfterViewInit(): void {
    
  }

  ngOnInit() {
    this.loading = true;
    if(this.dashboardmode == true){
        this.DashboardInit();
    }
    else{
      this.rootFilter.valueTake = this.defaultcount;
      this.rootFilter.valueSkip = 0;
      this.Refresh();
    }
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

    if(this.child){
      if(this.parentdataid == null || this.parentdataid == undefined){
        this.rootFilter.valueFilters.push({ field: 'ParentDataObjectId', operator: this.parenttypeid, value: this.parentrow });

        if(this.parentfilter){
          this.rootFilter.parentValueFilters = this.parentfilter.valueFilters;
          this.rootFilter.parentValueSortingParams = this.parentfilter.valueSortingParams;
        }
        
      }
      else{
        this.rootFilter.valueFilters.push({ field: 'ParentDataObjectId', operator: this.parenttypeid, value: this.parentdataid });
      }
      this.InitData(this.private == true);
    }
    else{
      if(this.dashboardmode == true){
        if(full){
          this.loading = true;
          this.rootFilter.filters = [];
          this.rootFilter.valueFilters = [];
          this.rootFilter.valueSortingParams = [];
          this.rootFilter.valueTake = this.defaultcount;
          this.rootFilter.valueSkip = 0;        
          this.table?.clear();
    
         }
         this.InitData(this.private ?? false)
      }
      else{
        this.route.paramMap.subscribe(params => {     
          this.name = params.get("name");
          this.id = params.get("id");
          if(full){
            this.loading = true;
            this.rootFilter.filters = [];
            this.rootFilter.valueFilters = [];
            this.rootFilter.valueSortingParams = [];
            this.rootFilter.valueTake = this.defaultcount;
            this.rootFilter.valueSkip = 0;
            this.table?.clear();
      
           }
           this.InitData(params.get("private") != undefined)
       }); 
      }
 
    }

 
  }

 

  InitData(privatelist: boolean){




    this.rootFilter.filters = [];

    if(localStorage.getItem('private') == 'true'){
      privatelist = true;
    }
    localStorage.removeItem('private');
    this.private = privatelist;

    if(localStorage.getItem('id')){
      this.id = localStorage.getItem('id') ?? "";
    }
    if(localStorage.getItem('name')){
      this.name = localStorage.getItem('name') ?? "";
    }
    this.rootFilter.filters.push({ field: 'Name', operator: 'equals', value: this.name });
    this.rootFilter.filters.push({ field: 'ObjectTypeId', operator: 'equals', value: this.id });
    localStorage.removeItem('name');
     localStorage.removeItem('id');

   
    if(localStorage.getItem('filter')){
      this.rootFilter.valueFilters.push(JSON.parse(localStorage.getItem('filter') ?? ""));
    }
     localStorage.removeItem('filter');

     if(localStorage.getItem('filterfull')){
      this.rootFilter = JSON.parse(localStorage.getItem('filterfull') ?? "");
    }
     localStorage.removeItem('filterfull');

    this.apiService.GetTypeByFilter(this.rootFilter,privatelist).subscribe(x =>{
      if(x.length == 0){
        this.loading = false;
      }
      var dashboardid = this.data?.dashboardTableId;
     this.data = x[0];
     this.data.dashboardTableId = dashboardid;
     this.count = x[0].count;
     this.id = x[0].objectTypeId;
     if(!this.data) {
       this.router.navigateByUrl('home');
     }
     this.titleService.setTitle(this.data.name); 
     this.meta.updateTag({ name: 'description', content: this.data.description });
     this.meta.updateTag({ name: 'keywords', content: this.data.description });

     this.rootFilterData.emit(this.rootFilter);
     this.InitList();
     this.loading = false;   
    }, () => {
      this.loading = false;
    },);

  //   this.apiService.GetCount(this.rootFilter).subscribe(x => {
  //    this.count = x;
  //  });
  }

  DashboardRefresh(chart : any, value : any, connectedcharts : any[],datalabel : any = null){
    this.loading = true;
    chart.charts = connectedcharts;
    this.apiService.GetTypeByChart(chart,this.private ?? false,value,datalabel).subscribe(x =>{
      if(x.type){
        this.loading = false;
      }
      var dashboardid = this.data.dashboardTableId;
     this.data = x.type;
     this.data.dashboardTableId = dashboardid;
     this.count = x.type.count;

     

     this.InitList();
     this.loading = false;   

     x.charts.forEach((element : any) => {
      this.chartrefreshdata.emit(element);
     });

    }, () => {
      this.loading = false;
    },);
  }

  InitList(){
    this.apiService.ListOwner(this.data.objectTypeId).subscribe(y => {
      this.admin = y;
      if(this.admin){
       this.contextitems =[
         { label: 'View', icon: 'pi pi-fw pi-search', command: () => this.Edit(this.selectedProduct) },
         { label: 'Edit type', icon: 'pi pi-spin pi-cog', command: () => this.EditType() },
         { label: 'Add child', icon: 'pi pi-spin pi-cog', command: () => this.AddChild() },
         { label: 'Add calculated field', icon: 'pi pi-spin pi-cog', command: () => this.AddCalculatedField() },
       ];
      }
    });

    this.fields =((this.data.field as object as any[]).map(y => {
      return{field: y.name, header: y.name, type: y.type, option: y.option, visible: y.visible}
    }));
    this.selectedColumns = this.fields.filter(x => x.visible == false || x.visible == undefined || x.visible == null);
    this.originalfields =((this.data.field as object as any[]).map(y => {
     return{field: y.name, header: y.name, type: y.type, option: y.option, visible: y.visible}
   }));
    this.rowData = this.data.dataObject;
    this.apiService.GetObjectAccess().subscribe(x =>{
     this.rowData.forEach(item => {
       item.editable = this.admin;
       if(this.dashboardmode == true){
        this.hiddenactive = false;
        this.HideSpam();
       }
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
        x.option.forEach((y : any) =>{
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
  }

  DashboardInit(){


    this.data = this.dashboardata;
    this.rootFilter = JSON.parse(this.dashboardfilter ?? '');
    this.count = this.dashboardata.count;
    this.private = this.dashboardata.private;
    this.name = this.dashboardata.name;
    this.id = this.dashboardata.objectTypeId;

    this.InitList();
    this.loading = false;

  }

  GetAccess(crud : string = "") : boolean{
    if(!this.data){
      return false;
    }
    if(crud == "f"){
      if(!this.data.noFilterMenu){
        return true;
      }
      else {
        return false;
      }     
    }

    if(this.editable == false){
      return false;
    }


    if(crud == "c"){
      if(!this.data.denyAdd){
        return true;
      }   
    }
    if(crud == "e"){
      if(!this.data.denyExport){
        return true;
      } 
    }

    if(crud == "ch"){
      if(!this.data.denyChart){
        return true;
      } 
    }


    if(this.admin){
      return true;
    }

    if(this.data.allUserFullAccess){
      return true;
    }

    return false;

  }

  FilterOption(field : any, option: any){
    this.rootFilter.valueFilters = [];
    this.rootFilter.valueFilters?.push( { field: field.field, operator: 'equals', value: option.optionName });
    this.Refresh(false);
  }


  Pages(){
    this.ref = this.dialogService.open(PageEditComponent,  { data: {id: this.id}, header: 'See pages', resizable: true});
    this.ref.onClose.subscribe(x => {
    });
  }

  CreateNew(){
    this.ref = this.dialogService.open(DataobjectCreateComponent,  { data: {child: this.child,  dataRec: [],parentdataid: this.parentdataid, parentid: this.data.objectTypeId, fields: this.originalfields}, header: 'Create new object', resizable: true});
    this.ref.onClose.subscribe(x => {
      this.Refresh(false);
    });
  }

  EditAll(){
    this.ref = this.dialogService.open(DataobjectCreateComponent,  { data: {dataRec: [],parentdataid: this.parentdataid, parentid: this.data.objectTypeId, fields: this.originalfields, filter: this.rootFilter, alledit: true}, header: 'Edit all filtered '+ this.name + ' ('+ this.count + ')', resizable: true});
    this.ref.onClose.subscribe(x => {
      this.Refresh(false);
    });
  }

  Edit(row: any){ 
    var rowidx = this.data.dataObject.indexOf(row);

    let objimages = this.imgdata.filter(x => x.obj == row.value.find((y: any) => y.name == this.fields.find(z => z.type == "id")?.header).valueString);
    this.ref = this.dialogService.open(DataobjectEditComponent,  { data: { rowindex : rowidx,parentfilter : this.rootFilter,editable:this.admin, images: objimages, parentdataid: this.parentdataid,dataRec: row.value, id: row.dataObjectId, parentid: this.data.objectTypeId, fields: this.originalfields}, header: 'Edit '+this.data.name + ' element', resizable: true});
    this.ref.onClose.subscribe(x => {
     // this.Refresh();
    });
  }

  AddCalculatedField(){
    this.ref = this.dialogService.open(CalculatedfieldComponent,  { data: {id: this.data.objectTypeId, fields: this.data.field}, header: 'Add calculated field', resizable: true});
    this.ref.onClose.subscribe(x => {
      // this.Refresh();
    });
  }

  AddColor(){
    this.ref = this.dialogService.open(CalculatedColorComponent,  { data: {id: this.data.objectTypeId, fields: this.data.field}, header: 'Add Color background', resizable: true});
    this.ref.onClose.subscribe(x => {
      // this.Refresh();
    });
  }

  AddColorLabel(){
    this.ref = this.dialogService.open(CalculatedColorComponent,  { data: {id: this.data.objectTypeId, fields: this.data.field, type: 'label'}, header: 'Add Color text', resizable: true});
    this.ref.onClose.subscribe(x => {
      // this.Refresh();
    });
  }

  TextSize(){
    this.ref = this.dialogService.open(CalculatedColorComponent,  { data: {id: this.data.objectTypeId, fields: this.data.field, type: 'size'}, header: 'Text Size', resizable: true});
    this.ref.onClose.subscribe(x => {
      // this.Refresh();
    });
  }

  hiddenactive : boolean = true;
  HideSpam(){
    this.hiddenactive = !this.hiddenactive;
  //   if(this.hiddenactive){
  //     var spams = document.getElementsByClassName("hidden1024");
      
  //     for (let index = 0; index < spams.length; index++) {
  //       if((spams.item(index) as HTMLElement).id == this.id) {
  //         (spams.item(index) as HTMLElement).style.display = "block"; 
  //       }
        
  // }
  // this.hiddenactive = false;
  //   }
  //   else{
  //     var spams = document.getElementsByClassName("hidden1024");
    
  //     for (let index = 0; index < spams.length; index++) {
  //     (spams.item(index) as HTMLElement).style.display = "none";   
  // }
  // this.hiddenactive = true;

  //   }
  }


  OpenChart(){
    this.loading = true;
    this.apiService.ChartData(this.rootFilter).subscribe(x => {
      this.loading = false;
      this.ref = this.dialogService.open(ChartComponent,  { data: {fields: this.fields ,filter : this.rootFilter,dataRec: x, name : this.name,id: this.id, private: this.private}, header: 'Chart', resizable: true});
      this.ref.onClose.subscribe(x => {
       // this.Refresh();
      });
    },error => {
      this.messageService.add({ 
        key: 'key1',
        severity: "error", 
        summary: "Error", 
        detail: "Something went wrong. Check if your field types are correct.", 
        life: 300000
      });
      this.loading = false;
    
    });

  }

  Clusters(){
    this.loading = true;
    this.apiService.ML(this.rootFilter).subscribe(x => {
      this.loading = false;
      this.ref = this.dialogService.open(ClustersComponent,  { data: {dataRec: x}, header: 'Clusters', resizable: true});
      this.ref.onClose.subscribe(x => {
        this.Refresh();
      });
    },error => {
      this.messageService.add({ 
        key: 'key1',
        severity: "error", 
        summary: "Error", 
        detail: "Something went wrong. Maybe you have not enough data.", 
        life: 300000
      });
      this.loading = false;
    
    });

  }

  Classification(){
    this.ref = this.dialogService.open(ClassificationComponent,  { data: {fields: this.originalfields, rootFilter: this.rootFilter}, header: 'Classification', resizable: true});
    this.ref.onClose.subscribe(x => {
     // this.Refresh();
    });
  }


  ImageClassification(){
    this.ref = this.dialogService.open(ImageClassificationComponent,  { data: {fields: this.originalfields, rootFilter: this.rootFilter}, header: 'Image Classification', resizable: true});
    this.ref.onClose.subscribe(x => {
     // this.Refresh();
    });
  }

  Regression(){
    this.ref = this.dialogService.open(RegressionComponent,  { data: {fields: this.originalfields, rootFilter: this.rootFilter}, header: 'Regression', resizable: true});
    this.ref.onClose.subscribe(x => {
     // this.Refresh();
    });
  }

  EditType(){
    this.ref = this.dialogService.open(ObjecttypeCreateComponent,  { data: { data: this.data}, header: 'Edit Object Definitoin', resizable: true});
    this.ref.onClose.subscribe(x => {
      //this.Refresh();
    });
  }

  AddChild(){
    this.ref = this.dialogService.open(ObjecttypeCreateComponent,  { data: {parenttype: this.data.objectTypeId}, header: 'Add child list', resizable: true});
    this.ref.onClose.subscribe(x => {
      //this.Refresh();
    });
  }

  RemoveChild(){
    this.ref = this.dialogService.open(ObjecttypeSelectComponent,  { data: {remove : true, id: this.id}, header: 'Remove child relation', resizable: true});
    this.ref.onClose.subscribe(x => {
      //this.Refresh();
    });
  }

  
  Export() {
    var filter : RootFilter = this.rootFilter;
    filter.valueTake = 0;
    this.apiService.GetTypeByFilter(this.rootFilter,this.private ?? false).subscribe(x =>{

      let xldata: { [x: string]: any; }[] = [];
      let idx = 0;
      let expdata : any[] = x[0].dataObject;
      expdata.forEach(element => {
        xldata[idx] = {};
        idx++;
      });
      this.selectedColumns.forEach(col => {
  
        let idx2 = 0;
        expdata.forEach(row => {
          xldata[idx2][col.header] = this.getValue(row.value, col);
          idx2++;
        });
  
      });
      let worksheet = XLSX.utils.json_to_sheet(xldata);
      const workbook = XLSX.utils.book_new();
      XLSX.utils.book_append_sheet(workbook, worksheet, "Table1");
      XLSX.writeFile(workbook, this.name + '.xlsx');
    });
    
  }

  
  DeleteDialog(event: Event) {
    this.confirmationService.confirm({
        target: event.target as EventTarget,
        header: 'Delete',
        acceptIcon:"none",
        rejectIcon:"none",
        rejectButtonStyleClass:"p-button-text",
        acceptButtonStyleClass:"p-button-danger",
        accept: () => {
          this.Delete();
        },
    });
}

  Delete(){
    this.loading = true;
    this.apiService.DeleteList(this.data.objectTypeId).subscribe({
      next: (v) => {
        this.router.navigateByUrl('');
      },
      error: (e) => {
        this.messageService.add({ 
          key: 'key1',
          severity: "error", 
          summary: "Error", 
          detail: e.error, 
          life: 300000
        });
        this.loading = false;
      },
      complete: () => {} 
  });
  }

  Back(){
    this.router.navigateByUrl('');
  }

  getValue(data: any[], col: any) : any{
    let res = data.find(x => x.name == col.field);
    return res?.valueString ?? "";
  }

  GetStyle(data: any[], col: any) : any{
    let res = data.find(x => x.name == col.field);
    if(res != null && res?.color != null && res?.color != undefined && res?.color != ""){
      return { "background-color" : res.color};
    }
    return {};
  }

  GetBackgroundColor(){
    if(this.data.color){
      return { "background-color" : this.data.color};
    }
    return {};
  }

  GetStyleLabel(data: any[], col: any) : any{
    let res = data.find(x => x.name == col.field);
    if(res){
      if((res.labelColor != null && res.labelColor != undefined && res.labelColor != "") && (res.textSize != null && res.textSize != undefined && res.textSize != "")){
        return { "color" : res.labelColor, "font-size" : res.textSize + "px"};
      }
      else if(res.textSize != null && res.textSize != undefined && res.textSize != ""){
        return { "font-size" : res.textSize + "px"};
      }
      else if(res.labelColor != null && res.labelColor != undefined && res.labelColor != ""){
        return { "color" : res.labelColor};
      }
    }

    return {};
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
        this.Refresh(false);
      

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

ShowMine(){
  this.apiService.GetUser(localStorage.getItem('username') ?? "").subscribe(x => {
    this.rootFilter.valueFilters.push({ field: 'AppUserId', operator: 'user', value: x.id });
    this.Refresh(false);
  });

}

importFile(event: any) {
  this.loading = true;
  var file = event.files[0];
  event.files = null;

  let reader: FileReader = new FileReader();
  reader.readAsBinaryString(file);

  reader.onload = (e) => {
    document.getElementById('button-clicker')?.focus();
    let result: any | undefined= e?.target?.result;
    const wb: XLSX.WorkBook = XLSX.read(result, { type: 'binary' });
    const wsname: string = wb.SheetNames[0];
    const ws: XLSX.WorkSheet = wb.Sheets[wsname];

    const data = XLSX.utils.sheet_to_json(ws);
    this.apiService.Import(data, this.data.objectTypeId,this.parentdataid).subscribe(x => {
      this.Refresh();
      this.fileUploader?.clear();
    }, error => {
      this.messageService.add({
        key: 'key1',
        severity: "error",
        summary: "Error",
        detail: error.error,
        life: 300000
      });
      this.loading = false;
      this.fileUploader?.clear();
    });
  };
}

onReject() {
  this.messageService.clear();
}

SendMessage(){
  this.ref = this.dialogService.open(MessageEditComponent,  { data: {remove : true, id: this.id}, header: 'Remove child relation', resizable: true});
  this.ref.onClose.subscribe(x => {
    //this.Refresh();
  });
}

AddToDashboard(){
  let dashboard = {
    ObjectTypeId : this.id,
    RootFilter : JSON.stringify(this.rootFilter),
    Size : 2,
    Position : 1,
  }

  this.loading = true;
  this.apiService.AddToDashboard(dashboard).subscribe(x => {
    this.messageService.add({ 
      severity: "success", 
      summary: "Ok:", 
      detail: "Success", 
      life: 300000
    });
    this.loading = false;
  }, error => {
    this.messageService.add({ 
      severity: "error", 
      summary: "Error", 
      detail: error.error, 
      life: 300000
    });
    this.loading = false;
  })
}

RemoveFromDashboard(){
  if(this.dashboarid){
    this.apiService.RemoveFromDashboard(this.dashboarid).subscribe(x =>{
      this.messageService.add({ 
        severity: "success", 
        summary: "Ok:", 
        detail: "Success", 
        life: 300000
      });
    });
  }
}

}
