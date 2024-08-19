import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, EventEmitter, Input, OnInit, Output, ViewChild, forwardRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { DialogService, DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { InputGroupModule } from 'primeng/inputgroup';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextModule } from 'primeng/inputtext';
import { MessagesModule } from 'primeng/messages';
import { ApiService } from '../../Services/api.service';
import { CheckboxModule } from 'primeng/checkbox';
import { TriStateCheckboxModule } from 'primeng/tristatecheckbox';
import { CalendarModule } from 'primeng/calendar';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DropdownModule } from 'primeng/dropdown';
import { TabViewModule } from 'primeng/tabview';
import { RootFilter } from '../../Models/Parameters';
import { DataobjectListComponent } from '../dataobject-list/dataobject-list.component';
import { FileUpload, FileUploadModule } from 'primeng/fileupload';
import { ImageModule } from 'primeng/image';
import { MessageEditComponent } from '../../User/message-edit/message-edit.component';
import { KeyFilterModule } from 'primeng/keyfilter';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { ChipModule } from 'primeng/chip';
import {ToastModule} from 'primeng/toast';
import { DataobjectParentselectComponent } from '../dataobject-parentselect/dataobject-parentselect.component';








@Component({
  selector: 'app-dataobject-edit',
  standalone: true,
  imports: [InputGroupModule, FormsModule,CommonModule,TriStateCheckboxModule,CalendarModule,ConfirmDialogModule,DropdownModule,TabViewModule,ImageModule,ChipModule,
    InputTextModule,InputNumberModule,ButtonModule,MessagesModule,CheckboxModule,forwardRef(() => DataobjectListComponent),FileUploadModule,KeyFilterModule,
    ProgressSpinnerModule,ToastModule
  ],
    providers: [MessageService,ConfirmationService], 
  templateUrl: './dataobject-edit.component.html',
  styleUrl: './dataobject-edit.component.css'
})
export class DataobjectEditComponent implements OnInit {

  @Input() dataRec: any;
  @Input() images: {data:any, field:string, obj: string}[] = [];
  @Input() parentid: string | undefined;
  @Input() parentdataid: string | undefined;
  @Input() id: string | undefined;
  @Input() editable: boolean = false;
  @Input() fields : {field : string, header: string, type: string, option: any[]}[] = [];

  public childlist : any | undefined;
  public loading: boolean = false;
  public boolvalues: (boolean| null)[] = [];
  public datevalues: (Date| null)[] = [];
  public aimodels : string[] = [];
  ref: DynamicDialogRef | undefined;
  @ViewChild('fileUploader', {static: false}) fileUploader: FileUpload[] | undefined;
  @Output() save = new EventEmitter();

  constructor( private confirmationService: ConfirmationService,public apiService: ApiService, protected changeDetector: ChangeDetectorRef,  protected config: DynamicDialogConfig,private messageService: MessageService
    , protected dialogService: DialogService
  ) 
  {
  }
  ngOnInit(): void {
 

    this.editable == true;
    if(this.config.data.parentdataid){
      this.parentdataid = this.config.data.parentdataid;
    }
    if(this.config.data.images){
      this.images = this.config.data.images;
    }
    if(this.config.data.dataRec){
      this.dataRec = this.config.data.dataRec;

    }
    if(this.config.data.fields){
      this.fields = this.config.data.fields;
    }
    if(this.config.data.editable){
      this.editable = this.config.data.editable;
      console.log(this.editable);
    }
    if(this.config.data.parentid){
      this.parentid = this.config.data.parentid;
      this.loading = true;
      let filter : RootFilter = {valueFilters: [], valueSkip:0,valueTake:0,valueSortingParams:[],skip:0,take:0,logic:'and',sortingParams:[], filters:[{ field: 'ParentObjectTypeId', operator: 'equals', value: this.parentid }]}
      this.apiService.GetChildTypeByFilter(filter).subscribe(x =>{
        this.childlist = x;
        this.loading = false;
      })
    }
    if(this.config.data.id){
      this.id = this.config.data.id;
    }
    var idfield = this.fields.filter(x => x.type == 'id')[0].header;
    this.id = this.dataRec.filter((x : any) => x.name == idfield)[0].valueString;
    let i : number = 0;
    this.fields.forEach(x=> {
      if(x.type == "boolean"){
        if(this.dataRec[i].valueString != null){
          this.boolvalues.push(this.dataRec[i].valueString == "true" || this.dataRec[i].valueString == "True" ? true : false);
        }
        else{
          this.boolvalues.push(false);
        }
      }
      else{
        this.boolvalues.push(false);
      }
      i++;
    })

    let j : number = 0;
    this.fields.forEach(x=> {
      if(x.type == "date"){
        if(this.dataRec[j].valueString != null){
          let datenum = this.dataRec[j].valueString.split('.');
          if(datenum[2].length != 4){
            if(datenum[2].length == 2){
              datenum[2] = "20" + datenum[2];
          }
          else if (datenum[2].length > 4){
            datenum[2] = datenum[2].substring(0,4);
          }
        }
          this.datevalues.push(new Date(datenum[2],datenum[1] -1,datenum[0]));
        }
        else{
          this.datevalues.push(null);
        }
      }
      else{
        this.datevalues.push(null);

      }
      j++;
    })
   

    this.apiService.AIModels(this.parentid ?? "").subscribe(x => {
      this.aimodels = x;
    });

    var element = document.getElementById("1");
    if(element != null){
     element.innerHTML= this.dataRec[1].valueString;
    }

  }

  HasAiModel(name: string){
    return this.aimodels.includes(name);
  }

  getImageObj(row: any, field: string){
    console.log(this.images);
    let res = this.images.find(x => x.field == field);
    console.log(res);
    if(res?.data){
      return res?.data;

    }
    return undefined;

  }

  getOption(opt: any[]){
    return opt
  }

  UploadFile(event : any, name: string)
  {
    var file : File = event.files[0];
    const formData = new FormData();
    formData.append('file', file, file.name);
    event.files = null;
    if(this.id != undefined && this.parentid != undefined){
    this.apiService.SaveFile(formData, this.id,name,this.parentid).subscribe(x => {
     // this.fileUploader?.forEach(x => {x.clear()});
      this.messageService.add({ 
        severity: "success", 
        summary: "Ok:", 
        detail: "Object Saved", 
        life: 300000
      });
     // this.Refresh();
    }, error => {
      this.messageService.add({ 
        severity: "error", 
        summary: "Error", 
        detail: error.message, 
        life: 300000
      });
    });
  }

  }

  setDate(event: any, i: number){
    this.dataRec[i].valueString = event.inputFieldValue;
  }

  OptionSelect(event: any, i: number){
    this.dataRec[i].valueString = event.value.optionName;
  }

  boolChange(event: any, i: number){
    this.dataRec[i].valueString = event.value;
  }

  
  Save(){
    let obj : {
      objectTypeId : string | undefined,
      dataObjectId : string | undefined,
      parentDataObjectId:  string | undefined,
      value : {name: string, valueString: string , dataObjectId:string, valueId: string}[]
    } = {value: [], objectTypeId: this.parentid, dataObjectId: this.id, parentDataObjectId: this.parentdataid}

    let index = 0;
    this.fields.forEach(element => {
      obj.value.push({name: element.field, valueString: String(this.dataRec[index].valueString != null ? this.dataRec[index].valueString : ""),
         dataObjectId: this.dataRec[index].dataObjectId,valueId: this.dataRec[index].valueId}); 
      index++;    
    });
    this.apiService.SaveDataObject(obj).subscribe({
      next: (v) => {
        this.messageService.add({ 
          severity: "success", 
          summary: "Ok:", 
          detail: "Object Saved", 
          life: 3000
        });
      },
      error: (e) => {
        console.log(e);
        this.messageService.add({ 
          severity: "error", 
          summary: "Error", 
          detail: e.error, 
          life: 3000
        });
      },
      complete: () => {} 
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

GetIdDict(){
  let idsfield = this.fields.filter(x => x.type == 'id');
  let idvalues : {key : string, value : string}[] = [];
  idsfield.forEach(x => {
    let index = this.dataRec.findIndex((y : any) => y.name == x.header);
    if(index != -1){
      idvalues.push({key: x.header, value: this.dataRec[index].valueString});
    }
  });
  return idvalues;
}

  Delete(){
    this.loading = true;
   let idvalues = this.GetIdDict();
    this.apiService.DeleteObject(idvalues,this.parentid).subscribe({
      next: (v) => {
        this.loading = false;
        this.messageService.add({ 
          severity: "success", 
          summary: "Ok:", 
          detail: "Object Deleted", 
          life: 300000
        });
      },
      error: (e) => {
        this.loading = false;
        this.messageService.add({ 
          severity: "error", 
          summary: "Error", 
          detail: e.message, 
          life: 300000
        });
      },
      complete: () => {} 
  });
  }


  SendMessage(){
    this.ref = this.dialogService.open(MessageEditComponent,  { data:  {dataobjectid: this.id}, header: 'Send Message', resizable: true});
  }

  ClassifyImage(event: Event,name : string){
    this.loading = true;
    this.apiService.ClassifySingleImage(this.parentid ?? "",name,this.GetIdDict()).subscribe(x => {
    this.loading = false;
    if(x.length == 0){
      this.messageService.add({
        severity: "error", 
        summary: "Error", 
        detail: 'No classification found. Create an Image Classification first.',
        life: 9000
      
      });
    }
    else{
      let message : string = "";
      x.forEach((element: any) => {
        message += element.field + " : " + element.value + ",\n";
      
      });
      this.confirmationService.confirm({
        target: event.target as EventTarget,
        header: 'Result of classification',
        acceptIcon:"none",
        rejectIcon:"none",
        message: 'Do you want to fill the following values with the classified values?\n' + message,
        rejectButtonStyleClass:"p-button-danger",
        acceptButtonStyleClass:"p-button-text",
        accept: () => {
          this.AutoFill(x);
        },
    });
    }

    });
  }

  PredictValueNum(event: Event,name : string){
    this.loading = true;
    this.apiService.PredictRegression(this.parentid ?? "",name,this.GetIdDict()).subscribe(x => {
    this.loading = false;
   
    
      let message : string =  x.field + " : " + x.value + ",\n";
     
      this.confirmationService.confirm({
        target: event.target as EventTarget,
        header: 'Result of regression prediction',
        acceptIcon:"none",
        rejectIcon:"none",
        message: 'Do you want to fill the following values with the predicted values?\n' + message,
        rejectButtonStyleClass:"p-button-danger",
        acceptButtonStyleClass:"p-button-text",
        accept: () => {
          this.AutoFill([x]);
        },
    });
    

    }, error => {
      this.messageService.add({
        severity: "error", 
        summary: "Error", 
        detail: error.message,
        life: 9000
      });
      this.loading = false;
    
    });
  }

  PredictValueStr(event: Event,name : string){
    this.loading = true;
    this.apiService.PredictClass(this.parentid ?? "",name,this.GetIdDict()).subscribe(x => {
    this.loading = false;
   
    
      let message : string = x.field + " : " + x.value + ",\n";
     
      this.confirmationService.confirm({
        target: event.target as EventTarget,
        header: 'Result of classification',
        acceptIcon:"none",
        rejectIcon:"none",
        message: 'Do you want to fill the following values with the classified values?\n' + message,
        rejectButtonStyleClass:"p-button-danger",
        acceptButtonStyleClass:"p-button-text",
        accept: () => {
          this.AutoFill([x]);
        },
    });
    

    }, error => {
      this.messageService.add({
        severity: "error", 
        summary: "Error", 
        detail: error.message,
        life: 9000
      });
      this.loading = false;
    
    });
  }


  AutoFill(x: any){
    x.forEach((element: any) => {
      let index = this.fields.findIndex(y => y.field == element.field);
      if(index != -1){
        this.dataRec[index].valueString = element.value;
      }
    });
  }

  SelectParent(field : string){
    this.apiService.SelectParent(this.parentid ?? "",field).subscribe(x => {
      console.log(x);
      this.ref = this.dialogService.open(DataobjectParentselectComponent,  {data: {id: x.objectTypeId, name: x.name, private: x.private }, header: 'Select Parent', resizable: true});
      this.ref.onClose.subscribe((x : any) => {
        console.log(x);
        if(x != undefined){
          this.dataRec.find((y : any) => y.name == field).valueString = x;
        }
      });
    });

  }

  onReject() {
    this.messageService.clear();
  }

}
