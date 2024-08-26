import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { MessageService } from 'primeng/api';
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
import { DropdownModule } from 'primeng/dropdown';
import { FileUpload, FileUploadModule } from 'primeng/fileupload';
import { KeyFilterModule } from 'primeng/keyfilter';
import {ToastModule} from 'primeng/toast';
import { RootFilter } from '../../Models/Parameters';
import { DataobjectParentselectComponent } from '../dataobject-parentselect/dataobject-parentselect.component';


@Component({
  selector: 'app-dataobject-create',
  standalone: true,
  imports: [InputGroupModule, FormsModule,CommonModule,CheckboxModule,TriStateCheckboxModule,CalendarModule,DropdownModule,FileUploadModule,KeyFilterModule,
    InputTextModule,InputNumberModule,ButtonModule,MessagesModule,ToastModule],
    providers: [MessageService], 
  templateUrl: './dataobject-create.component.html',
  styleUrl: './dataobject-create.component.css'
})
export class DataobjectCreateComponent  implements OnInit {
  ref: DynamicDialogRef | undefined;
  @Input() dataRec: any;
  @Input() alledit: boolean = false;
  @Input() rootFilter: RootFilter | undefined;
  @Input() parentid: string | undefined;
  @Input() parentdataid: string | undefined;
  @Input() fields : {field : string, header: string, type: string, option: any[]}[] = [];
  @ViewChild('fileUploader', {static: false}) fileUploader: FileUpload | undefined;
  constructor(public apiService: ApiService, protected changeDetector: ChangeDetectorRef,  protected config: DynamicDialogConfig,private messageService: MessageService
    , protected dialogService: DialogService
  ) 
  {
  }

  ngOnInit(): void {
    if(this.config.data.parentdataid){
      this.parentdataid = this.config.data.parentdataid;
    }
    if(this.config.data.dataRec){
      this.dataRec = this.config.data.dataRec;
    }
    if(this.config.data.fields){
      this.fields = this.config.data.fields;
    }
    if(this.config.data.parentid){
      this.parentid = this.config.data.parentid;
    }
    if(this.config.data.filter){
      this.rootFilter = this.config.data.filter;
    }
    if(this.config.data.alledit){
      this.alledit = this.config.data.alledit;
    }
  }

  setDate(event: any, prop: any){
    this.dataRec[prop.field] = event.inputFieldValue;
  }

  OptionSelect(event: any, prop: any){
    this.dataRec[prop.field] = event.value.optionName;
  }

  boolChange(event: any, prop: any){

    this.dataRec[prop.field] = event.checked;
  }

  // UploadFile(event : any, id: string)
  // {
  //   var file : File = event.files[0];
  //   const formData = new FormData();
  //   formData.append('file', file, file.name);
  //   event.files = null;
  //   if(id != undefined){
  //   this.apiService.SaveFile(formData, id).subscribe(x => {
  //   //  this.fileUploader?.clear();
  //     this.messageService.add({ 
  //       severity: "success", 
  //       summary: "Ok:", 
  //       detail: "Object Saved", 
  //       life: 300000
  //     });
  //    // this.Refresh();
  //   }, error => {
  //     this.messageService.add({ 
  //       severity: "error", 
  //       summary: "Error", 
  //       detail: error.message, 
  //       life: 300000
  //     });
  //   });
  // }

  // }

  Save(){
    let obj : {
      objectTypeId : string | undefined,
      parentDataObjectId:  string | undefined,
      value : {name: string, valueString: string}[]
    } = {value: [], objectTypeId: this.parentid, parentDataObjectId: this.parentdataid}

    this.fields.forEach(element => {
      obj.value.push({name: element.field, valueString: String(this.dataRec[element.field] != null ? this.dataRec[element.field] : "")});     
    });

    if(this.alledit == true && this.rootFilter != undefined){
      this.rootFilter.filters = this.rootFilter.filters.filter(x => x.value != undefined);
      this.apiService.EditAllFiltered(obj,this.rootFilter).subscribe({
        next: (v) => {
          this.messageService.add({ 
            severity: "success", 
            summary: "Ok:", 
            detail: "Object Saved", 
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
        complete: () => {} 
    });
    }
    else{
      this.apiService.CreateDataObject(obj).subscribe({
        next: (v) => {
          this.messageService.add({ 
            severity: "success", 
            summary: "Ok:", 
            detail: "Object Saved", 
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
        complete: () => {} 
    });      
    }

  }

  SelectParent(field : string){
    this.apiService.SelectParent(this.parentid ?? "",field).subscribe(x => {
      this.ref = this.dialogService.open(DataobjectParentselectComponent,  {data: {id: x.objectTypeId, name: x.name, private: x.private }, header: 'Select Parent', resizable: true});
      this.ref.onClose.subscribe((x : any) => {
        if(x != undefined){
          this.dataRec[field] = x;
        }
      });
    });

  }

  onReject() {
    this.messageService.clear();
  }
}
