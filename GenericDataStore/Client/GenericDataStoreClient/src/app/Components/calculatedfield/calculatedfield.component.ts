import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ConfirmationService, MessageService } from 'primeng/api';
import { DynamicDialogConfig, DialogService } from 'primeng/dynamicdialog';
import { ApiService } from '../../Services/api.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DropdownModule } from 'primeng/dropdown';
import { InputGroupModule } from 'primeng/inputgroup';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextModule } from 'primeng/inputtext';
import { KeyFilterModule } from 'primeng/keyfilter';
import { MessagesModule } from 'primeng/messages';
import { TriStateCheckboxModule } from 'primeng/tristatecheckbox';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { ProgramDescriptionComponent } from '../../home/program-description/program-description.component';

@Component({
  selector: 'app-calculatedfield',
  standalone: true,
  imports: [InputGroupModule, FormsModule,CommonModule,TriStateCheckboxModule,ConfirmDialogModule,DropdownModule,
    InputTextModule,InputNumberModule,ButtonModule,MessagesModule,CheckboxModule,KeyFilterModule,InputTextareaModule
  ],
  templateUrl: './calculatedfield.component.html',
  styleUrl: './calculatedfield.component.css'
})
export class CalculatedfieldComponent  implements OnInit {

  constructor( private confirmationService: ConfirmationService,public apiService: ApiService, protected changeDetector: ChangeDetectorRef,  protected config: DynamicDialogConfig,private messageService: MessageService
    , protected dialogService: DialogService
  ) {
  }

  options: any[] | undefined = [
    { name: 'numeric', code: 'numeric' },
    { name: 'text', code: 'text' },
    { name: 'boolean', code: 'boolean' },
];

numfunctions: any[] | undefined = [ 
  { name: 'max from child', code: '[child.yourchildtablename.yourchildtablefield.max]' },
  { name: 'min from child', code: '[child.yourchildtablename.yourchildtablefield.min]' },
  { name: 'average from child', code: '[child.yourchildtablename.yourchildtablefield.avg]' },
  { name: 'sum from child', code: '[child.yourchildtablename.yourchildtablefield.sum]' },
  { name: 'count from child', code: '[child.yourchildtablename.yourchildtablefield.count]' },
  { name: 'first from child', code: '[child.yourchildtablename.yourchildtablefield.first]' },
  { name: 'last from child', code: '[child.yourchildtablename.yourchildtablefield.last]' },
  { name: 'count distinct from child', code: '[child.yourchildtablename.yourchildtablefield.countdistinct]' },
  { name: 'count null from child', code: '[child.yourchildtablename.yourchildtablefield.countnull]' },
  { name: 'count not null from child', code: '[child.yourchildtablename.yourchildtablefield.countnotnull]' },
  

  { name: 'max from child where field equals 1', code: '[child.yourchildtablename.yourchildtablefield.max.where(yourchildotherfieldname=1)]' },
  { name: 'min from child where field not equals 1', code: '[child.yourchildtablename.yourchildtablefield.min.where(yourchildotherfieldname!=1)]' },
  { name: 'average from child where field less than 1', code: '[child.yourchildtablename.yourchildtablefield.avg.where(yourchildotherfieldname<1)]' },
  { name: 'sum from child where field bigger than 1', code: '[child.yourchildtablename.yourchildtablefield.sum.where(yourchildotherfieldname>1)]' },
  { name: 'count from child where field bigger or equals 1', code: '[child.yourchildtablename.yourchildtablefield.count.where(yourchildotherfieldname<=1)]' },
  { name: 'count distinct from child where field less or equals 1', code: '[child.yourchildtablename.yourchildtablefield.countdistinct.where(yourchildotherfieldname>=1)]' },
  { name: 'first from child ordered by field', code: '[child.yourchildtablename.yourchildtablefield.first]' },
  { name: 'last from child ordered by field descending', code: '[child.yourchildtablename.yourchildtablefield.last]' },

];

textfunctions: any[] | undefined = [
  { name: 'first from child', code: '[child.yourchildtablename.yourchildtablefield.first]' },
  { name: 'last from child', code: '[child.yourchildtablename.yourchildtablefield.last]' },
  { name: 'concat from child', code: '[child.yourchildtablename.yourchildtablefield.concat]' },

  { name: 'sum lenght from child', code: '[child.yourchildtablename.yourchildtablefield.sumlenght]' },
  { name: 'min lenght from child', code: '[child.yourchildtablename.yourchildtablefield.minlenght]' },
  { name: 'max lenght from child', code: '[child.yourchildtablename.yourchildtablefield.maxlenght]' },
  { name: 'avg length from child', code: '[child.yourchildtablename.yourchildtablefield.avglength]' },

];

boolfunctions: any[] | undefined = [
  { name: 'all true from child', code: '[child.yourchildtablename.yourchildtablefield.all]' },
  { name: 'any true from child', code: '[child.yourchildtablename.yourchildtablefield.any]' },
  { name: 'all false from child', code: '[child.yourchildtablename.yourchildtablefield.allnot]' },

  { name: 'any false from child', code: '[child.yourchildtablename.yourchildtablefield.anynot]' },
  { name: 'more true from child', code: '[child.yourchildtablename.yourchildtablefield.moretrue]' },
  { name: 'more false from child', code: '[child.yourchildtablename.yourchildtablefield.morefalse]' },
  { name: 'first true from child', code: '[child.yourchildtablename.yourchildtablefield.first]' },
  { name: 'last true from child', code: '[child.yourchildtablename.yourchildtablefield.last]' },

];


numoperators: any[] | undefined = [
  { name: '+', code: '+' },
  { name: '-', code: '-' },
  { name: '*', code: '*' },
  { name: '/', code: '/' },
  { name: '^', code: '^' },
  { name: '(', code: '(' },
  { name: ')', code: ')' },
];

textoperators: any[] | undefined = [
  { name: '+', code: '+' },
  { name: '-', code: '-' },
];

booloperators: any[] | undefined = [
  { name: 'and', code: '&' },
  { name: 'or', code: ' |' },
  { name: 'not', code: '!' },
  { name: '=', code: '=' },
  { name: '!=', code: '!=' },

  { name: '<', code: '<' },
  { name: '>', code: '>' },
  { name: '<=', code: '<=' },
  { name: '>=', code: '>=' },
  { name: 'a contains b', code: '->' },
  { name: 'b contains a', code: '<-' },

  { name: '(', code: '(' },
  { name: ')', code: ')' },
];


chtype : any | undefined = { name: 'numeric', code: 'numeric' };
chnum : any | undefined;
chstr : any | undefined;
chbool : any | undefined;

fields : any[] = [];
id : string | undefined;
name : string | undefined;


calculationString : string = "";
// choosenfields : any[] = [];
  ngOnInit(): void {
    if(this.config.data.id){
      this.id = this.config.data.id;
    }
    if(this.config.data.fields){
      this.fields = this.config.data.fields;
    }
  }
  AddOperator(op : any){
    this.calculationString += op.value.code;
  }

  AddToCalc(prop : any){
    this.calculationString +='{'+ prop.name + '}';
    // this.choosenfields.push({fieldId: prop.fieldId, operator: null, type: prop.type, name: prop.name,option: prop.option, objectTypeId: prop.objectTypeId});
  }

  RemoveFromCalc(){
    // this.choosenfields.pop();
  }

  SetOperator(event: any, i: number){
    // this.choosenfields[i].operator = event.value;
  }

  Save(){
    if(this.name == undefined || this.name == ""){
      this.messageService.add({severity:'error', summary: 'Error', detail: 'Name is required'});
      return;
    }
    let calculatedfield: {} = {
      name : this.name,
      calculationString : this.calculationString,
      typeId : this.id,
      originalType: this.chtype.code,
    }
    this.apiService.SaveCalculatedField(calculatedfield).subscribe(x =>{
      this.messageService.add({severity:'success', summary: 'Success', detail: 'Calculated field saved'});
    }, err => {
      this.messageService.add({severity:'error', summary: 'Error', detail: err.error});
    })
  }

  ShowDesc(){
    this.dialogService.open(ProgramDescriptionComponent, {});
  }
}
