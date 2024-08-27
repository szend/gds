import { ChangeDetectorRef, Component, Input, OnInit, ViewChild } from '@angular/core';
import { DialogService, DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { TooltipModule } from 'primeng/tooltip';
import { InputGroupModule } from 'primeng/inputgroup';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';
import { MessagesModule } from 'primeng/messages';
import { ApiService } from '../../Services/api.service';
import { MessageService } from 'primeng/api';
import { TabViewModule } from 'primeng/tabview';
import { CheckboxModule } from 'primeng/checkbox';
import { CardModule } from 'primeng/card';
import { FileUpload, FileUploadModule } from 'primeng/fileupload';
import * as XLSX from 'xlsx';
import {ToastModule} from 'primeng/toast';
import { ObjecttypeSelectComponent } from '../objecttype-select/objecttype-select.component';




@Component({
  selector: 'app-objecttype-create',
  standalone: true,
  imports: [InputGroupModule, FormsModule,CommonModule,TabViewModule,CheckboxModule,CardModule,InputTextareaModule,
    InputTextModule,InputNumberModule,ButtonModule,MessagesModule,DropdownModule, TooltipModule,FileUploadModule,ToastModule],
    providers: [MessageService,DialogService], 
  templateUrl: './objecttype-create.component.html',
  styleUrl: './objecttype-create.component.css'
})
export class ObjecttypeCreateComponent implements OnInit {
  @ViewChild('fileUploader', { static: false }) fileUploader: FileUpload | undefined;
  @Input() dataRec: any;
  public data: any[] | undefined;
  options: any[] | undefined = [
    { name: 'numeric', code: 'numeric' },
    { name: 'text', code: 'text' },
    { name: 'date', code: 'date' },
    { name: 'id', code: 'id' },
    { name: 'boolean', code: 'boolean' },
    { name: 'option', code: 'option' },
    // { name: 'file', code: 'file' },
    { name: 'image', code: 'image' },
    { name: 'foreignkey', code: 'foreignkey' },
];
  
 categories : {cat : string, id: string}[] = [];
 editablefieldtype : boolean = true;
 public databases: any[] = [];
 ref: DynamicDialogRef | undefined;


  constructor(public apiService: ApiService, protected changeDetector: ChangeDetectorRef,  protected config: DynamicDialogConfig,private messageService: MessageService, protected dialogService: DialogService) 
  {
  }

  ngOnInit() {
    this.apiService.GetAllCategory().subscribe(x => {
      this.categories = x; 
    });
    this.apiService.GetPublicDatabases().subscribe(x => {
      this.databases = x; 
      this.databases.forEach((x) => {
        if(x.default == true){
          x.databaseName = x.databaseName + " (Internal)";
        }
        else{
          x.databaseName = x.databaseName + " (External)";
        }
      });
    })
    if(this.config.data.data){
      this.dataRec = this.config.data.data;
      this.props = this.dataRec.field;
      this.name = this.dataRec.name;
      this.desc = this.dataRec.description;
      this.datid = this.dataRec.databaseConnectionPropertyId;
      this.cat = this.dataRec.category;
      this.editable = false;
    }
    else{
      this.editable = true;
      this.props.push({name: "", type: "text"});
      this.dataRec = {};
      this.dataRec.denyAdd = true;
      if(this.config.data.parenttype){
        this.dataRec.parentObjectTypeId = this.config.data.parenttype;
      }

    }
  }

  public props : any[] = [];
  public name : string = "";
  public desc : string = "";
  public cat : string = "";
  public datid : string = "";
  public editable: boolean = true;


  AddProperty(){
    this.props.push({name: "", type: "text", editable: true});
  }

  GetDatabaseName(){
    if(!this.databases.find(x => x.databaseConnectionPropertyId == this.datid) ||this.datid == "" || this.datid == undefined || this.datid == "00000000-0000-0000-0000-000000000000" || this.datid == null){
      return "Select Database";
    }
    return this.databases.find(x => x.databaseConnectionPropertyId == this.datid).databaseName;
  }

  AddOption(prop : any){
    if(this.props.find(x => x.name == prop.name).option == undefined){
      this.props.find(x => x.name == prop.name).option = [];
    }
    let length = this.props.find(x => x.name == prop.name).option.length;
    this.props.find(x => x.name == prop.name).option.push({optionName: 'option1', optionValue: length });


  }

  RemoveOption(prop : any, index: number){
    this.props.find(x => x.name == prop.name).option.splice(index,1);  

  }

  SelectChild(){
    this.ref = this.dialogService.open(ObjecttypeSelectComponent,  { data: {id: this.dataRec.parentObjectTypeId}, header: 'Select table', resizable: true});
    this.ref.onClose.subscribe(x => {
    });
  }

  Save(){
    
    if(this.cat?.length > 30){
      this.messageService.add({ 
        severity: "error", 
        summary: "Error", 
        detail: "Category name is too long. Max 30 characters.", 
        life: 3000
      });
      return;
    }
    if(this.name?.length > 100){
      this.messageService.add({ 
        severity: "error", 
        summary: "Error", 
        detail: "Name is too long. Max 100 characters.", 
        life: 3000
      });
      return;
    }
    if(this.name?.includes(' ') || this.name?.includes('/')|| this.name?.includes('#')|| this.name?.includes("'")|| this.name?.includes('"')|| this.name?.includes('%')){
      this.messageService.add({ 
        severity: "error", 
        summary: "Error", 
        detail: "Name can not contains special characters", 
        life: 3000
      });
      return;
    }
    if(this.desc?.length > 1000){
      this.messageService.add({ 
        severity: "error", 
        summary: "Error", 
        detail: "Description is too long. Max 1000 characters.", 
        life: 3000
      });
      return;
    }
    let newtype = {
      objectTypeId:  this.dataRec ? this.dataRec.objectTypeId : "00000000-0000-0000-0000-000000000000",
      name: this.name,
      databaseConnectionPropertyId: this.datid && this.datid != "" ? this.datid :  "00000000-0000-0000-0000-000000000000",
      description: this.desc,
      category : this.cat,
      field: this.props,
      private: this.dataRec.private,
      noFilterMenu:  this.dataRec.noFilterMenu,
      denyAdd:  this.dataRec.denyAdd,
      denyExport:  this.dataRec.denyExport,
      denyChart:  this.dataRec.denyChart,
      allUserFullAccess:  this.dataRec.allUserFullAccess,
      parentObjectTypeId: this.dataRec.parentObjectTypeId
    };

    if(newtype.parentObjectTypeId){
      this.apiService.AddChild(newtype,newtype.parentObjectTypeId).subscribe({
        next: (v) => {
          this.messageService.add({ 
            severity: "success", 
            summary: "Ok:", 
            detail: "Object Saved. Refresh to see changes.", 
            life: 3000
          });
  
        },
        error: (e) => {
          this.messageService.add({ 
            severity: "error", 
            summary: "Error", 
            detail: e.error, 
            life: 3000
          });
        },
        complete: () => {     
  
        } 
    });
    }
    else if(this.data == undefined){
      this.apiService.SaveObjectType(newtype).subscribe({
        next: (v) => {
          this.messageService.add({ 
            severity: "success", 
            summary: "Ok:", 
            detail: "Object Saved. Refresh to see changes.", 
            life: 3000
          });
  
        },
        error: (e) => {
          this.messageService.add({ 
            severity: "error", 
            summary: "Error", 
            detail: e.error, 
            life: 3000
          });
        },
        complete: () => {     
  
        } 
    });
    }
    else{
      let model : {File: any[], ObjectType: any} = {File: this.data, ObjectType: newtype};
      this.apiService.ImportCreate(model).subscribe({
        next: (v) => {
          this.messageService.add({ 
            severity: "success", 
            summary: "Ok:", 
            detail: "Table created. Refresh to see changes.", 
            life: 3000
          });
  
        },
        error: (e) => {
          this.messageService.add({ 
            severity: "error", 
            summary: "Error", 
            detail: e.error, 
            life: 3000
          });
        },
        complete: () => {     
  
        }
      });
    }

  }

  RemoveProperty(index: number){
    this.props.splice(index, 1);  
  }

  SetType(event : any, i : number){
    this.props[i].type = event.value.code;
  }

  SetCategory(event : any){

    if(event?.value?.cat){
      this.cat = event.value.cat;
    }
    else{
      this.cat = event?.value;
    }
  }

  SetDatabase(event : any){

    if(event?.value?.databaseConnectionPropertyId){
      this.datid = event.value.databaseConnectionPropertyId;
    }
    else{
      this.datid = event?.value;
    }
  }

  AccessAllUser(event : any){
    if(this.dataRec.allUserFullAccess){
      this.dataRec.denyAdd = false;
      this.dataRec.denyChart = false;
      this.dataRec.denyExport = false;
    }

  }

  AccessDeny(event : any){
    if(this.dataRec.denyAdd || this.dataRec.denyChart || this.dataRec.denyExport){
      this.dataRec.allUserFullAccess = false;
    }
  }

  importFile(event: any) {
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
  
      this.data = XLSX.utils.sheet_to_json(ws);
      
      this.props = [];
      Object.keys(this.data[0] as any).forEach((key) => {
        GetKeyTypes(this.data, key, this.props);
      });
      

    };
  }

  onReject() {
    this.messageService.clear();
  }

}

function GetKeyTypes(data: any[] | undefined, key: string, props: any[]) {
  if(data == undefined) return;

  const value = data[0][key];
  const type = typeof value;
  
  if (type === 'number') {
    props.push({ name: key, type: 'numeric' });
  } else if (type === 'string') {
    props.push({ name: key, type: 'text' });
  } else if (type === 'boolean') {
    props.push({ name: key, type: 'boolean' });
  } else if (type === 'object' && Array.isArray(value)) {
    props.push({ name: key, type: 'option' });
  } else if (type === 'object' && value instanceof Date) {
    props.push({ name: key, type: 'date' });
  } else {
    props.push({ name: key, type: 'text' });
  }

}


