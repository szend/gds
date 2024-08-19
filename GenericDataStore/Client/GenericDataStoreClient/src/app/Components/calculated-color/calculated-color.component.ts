import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DropdownModule } from 'primeng/dropdown';
import { DynamicDialogConfig, DialogService } from 'primeng/dynamicdialog';
import { InputGroupModule } from 'primeng/inputgroup';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { KeyFilterModule } from 'primeng/keyfilter';
import { MessagesModule } from 'primeng/messages';
import { TriStateCheckboxModule } from 'primeng/tristatecheckbox';
import { ApiService } from '../../Services/api.service';
import { ProgramDescriptionComponent } from '../../home/program-description/program-description.component';

@Component({
  selector: 'app-calculated-color',
  standalone: true,
  imports: [InputGroupModule, FormsModule,CommonModule,TriStateCheckboxModule,ConfirmDialogModule,DropdownModule,
    InputTextModule,InputNumberModule,ButtonModule,MessagesModule,CheckboxModule,KeyFilterModule,InputTextareaModule
  ],
  templateUrl: './calculated-color.component.html',
  styleUrl: './calculated-color.component.css'
})
export class CalculatedColorComponent implements OnInit{

  constructor( private confirmationService: ConfirmationService,public apiService: ApiService, protected changeDetector: ChangeDetectorRef,  protected config: DynamicDialogConfig,private messageService: MessageService
    , protected dialogService: DialogService
  ) {
  }
  fields : any[] = [];
id : string | undefined;
calculationColor : string = "";

  choosenfield : any | undefined = { name: 'numeric', code: 'numeric' };

  fieldsoption: any[] | undefined = [
    { name: 'numeric', code: 'numeric' },
    { name: 'text', code: 'text' },
    { name: 'boolean', code: 'boolean' },
];


  ngOnInit(): void {
    if(this.config.data.id){
      this.id = this.config.data.id;
    }
    if(this.config.data.fields){
      this.fieldsoption = this.config.data.fields.map((x : any) => { return { name: x.name, code: x.fieldId } });
      if(this.fieldsoption){
        this.choosenfield = this.fieldsoption[0];
      }
    }
  }

  Save(){
    let calculatedcolor: {} = {
      fieldId : this.choosenfield.code,
      calculationColor : this.calculationColor,
      typeId : this.id,
    }
    this.apiService.SaveCalculatedColor(calculatedcolor).subscribe(x =>{
      this.messageService.add({severity:'success', summary: 'Success', detail: 'Calculated field saved'});
    }, err => {
      this.messageService.add({severity:'error', summary: 'Error', detail: err.error});
    })
  }

  ShowDesc(){
    this.dialogService.open(ProgramDescriptionComponent, {});
  }
  
}
