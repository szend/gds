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
import { StepperModule } from 'primeng/stepper';
import { SplitterModule } from 'primeng/splitter';
import { CardModule } from 'primeng/card';

@Component({
  selector: 'app-calculatedfield',
  standalone: true,
  imports: [InputGroupModule, FormsModule,CommonModule,TriStateCheckboxModule,ConfirmDialogModule,DropdownModule,
    InputTextModule,InputNumberModule,ButtonModule,MessagesModule,CheckboxModule,KeyFilterModule,InputTextareaModule,StepperModule,SplitterModule,CardModule

  ],
  templateUrl: './calculatedfield.component.html',
  styleUrl: './calculatedfield.component.css'
})
export class CalculatedfieldComponent  implements OnInit {

  constructor( private confirmationService: ConfirmationService,public apiService: ApiService, protected changeDetector: ChangeDetectorRef,  protected config: DynamicDialogConfig,private messageService: MessageService
    , protected dialogService: DialogService
  ) {
  }

  conditions: any[] = [];

  chssourceopt : any[] | undefined = [
    { name: 'property', code: 'property' },
    { name: 'connected table', code: 'connected table' },
    { name: 'manual input', code: 'manual input' },
  ]
  chldorparntopt : any[] | undefined = [
    { name: 'child', code: 'child' },
    { name: 'parent', code: 'parent' },
  ]

  connecfuncopt : any[] | undefined = [
    { name: 'sum', code: 'sum' },
    { name: 'avg', code: 'avg' },
    { name: 'min', code: 'min' },
    { name: 'max', code: 'avg' },
    { name: 'count', code: 'count' },
    { name: 'countdistinct', code: 'countdistinct' },
    { name: 'countnull', code: 'countnull' },
    { name: 'countnotnull', code: 'countnotnull' },
    { name: 'first', code: 'first' },
    { name: 'last', code: 'last' },
    { name: 'concat', code: 'concat' },
    { name: 'minlenght', code: 'minlenght' },
    { name: 'maxlenght', code: 'maxlenght' },
    { name: 'sumlenght', code: 'sumlenght' },
    { name: 'avglength', code: 'avglength' },
    { name: 'all', code: 'all' },
    { name: 'anynot', code: 'anynot' },
    { name: 'moretrue', code: 'moretrue' },
    { name: 'morefalse', code: 'morefalse' },
  ]

  prop : any | undefined;
  chssource : any | undefined;
  manual : any | undefined;
  connecttable : any | undefined;
  connecprop : any | undefined;
  chldorparnt : any | undefined;
  connecfunc : any | undefined;

  propcon1 : any | undefined;
  chssourcecon1 : any | undefined;
  manualcon1 : any | undefined;
  connecttablecon1 : any | undefined;
  connecpropcon1 : any | undefined;
  chldorparntcon1 : any | undefined;
  connecfunccon1 : any | undefined;

  propcon2 : any | undefined;
  chssourcecon2 : any | undefined;
  manualcon2 : any | undefined;
  connecttablecon2 : any | undefined;
  connecpropcon2 : any | undefined;
  chldorparntcon2 : any | undefined;
  connecfunccon2 : any | undefined;

  conselectedoperator : any | undefined;

  selectedcondition : any | undefined;

  options: any[] | undefined = [
    { name: 'numeric', code: 'numeric' },
    { name: 'text', code: 'text' },
    { name: 'boolean', code: 'boolean' },
];

conoperators: any[] | undefined = [
  { name: '=', code: '=' },
  { name: '!=', code: '!=' },
  { name: '>', code: '>' },
  { name: '<', code: '<' },
  { name: '<=', code: '<=' },
  { name: '>=', code: '>=' },
  { name: '->', code: '->' },
  { name: '<-', code: '<-' },
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

partenttables: any[] | undefined;
childtables: any[] | undefined;


calculationString : string = "";
// choosenfields : any[] = [];
  ngOnInit(): void {
    if(this.config.data.id){
      this.id = this.config.data.id;
      this.apiService.GetConnectedTables(this.id ?? '').subscribe(x => {
        this.partenttables = x.parents;
        this.childtables = x.childs;
      });

    }
    if(this.config.data.fields){
      this.fields = this.config.data.fields;
    }
  }

  AddOperator(op : any, tostring: string | undefined = "x") : string | undefined{
    if(tostring != "x"){
      console.log(tostring);
      tostring += this.conselectedoperator.code;
      this.conselectedoperator = undefined;
      return tostring;
    }
    else{
      if(this.selectedcondition){
        var constring = this.calculationString.split('$').filter((x : string) => x.includes(this.selectedcondition))[0];
        var constringvalue = constring + op.value.code;
        this.calculationString = this.calculationString.replace(constring,constringvalue);
      }
      else{
        this.calculationString += op.value.code;
      }

      op = undefined;
    }
    return undefined;

  }

  AddCondition(){
    var constring : string | undefined = "";
    if(this.manualcon1){
      constring = this.AddManual(constring, true);
    }
    else if(this.propcon1){
      constring = this.AddFromProp(constring, true);
    }
    else if (this.connecfunccon1 || this.chldorparntcon1?.name == 'parent'){
      constring = this.AddFromTable(constring, true);
    }

    constring = this.AddOperator(null,constring);
console.log(this.connecfunccon2);
    if(this.manualcon2){
      constring = this.AddManual(constring, false);
    }
    else if(this.propcon2){
      constring = this.AddFromProp(constring, false);
    }
    else if (this.connecfunccon2 || this.chldorparntcon2?.name == 'parent'){
      constring = this.AddFromTable(constring, false);
    }

    this.conditions.push(constring);
    this.calculationString += "$ if'(" + constring + ")' ??  $";

  }

  AddFromProp(tostring : string | undefined = "x", part1 : boolean | undefined = undefined) : string | undefined{
    if(tostring != "x"){
      if(part1 == true){
        tostring +=' {'+ this.propcon1.name + '} ';

        this.chssourcecon1 = undefined;
        this.propcon1 = undefined;
      }
      else{
        tostring +=' {'+ this.propcon2.name + '} ';

        this.chssourcecon2 = undefined;
        this.propcon2 = undefined;
      }

      
      return tostring;

    }
    else{
      if(this.selectedcondition){
        var constring = this.calculationString.split('$').filter((x : string) => x.includes(this.selectedcondition))[0];
        var constringvalue = constring + ' {'+ this.prop.name + '} ';

        this.calculationString = this.calculationString.replace(constring,constringvalue);
      }
      else{
        this.calculationString +=' {'+ this.prop.name + '} ';
      }

      this.chssource = undefined;
      this.prop = undefined;
    }
    return undefined;

  }

  AddFromTable(tostring : string | undefined = "x", part1 : boolean | undefined = undefined) : string | undefined{
    console.log(part1);
    if(tostring != "x"){
      if(part1 == true){
        if(this.chldorparntcon1.name == 'child'){
          tostring +=' ['+ this.chldorparntcon1.name + '.' + this.connecttablecon1.name + '.' + this.connecpropcon1.name + '.' + this.connecfunccon1.name + '] ';
        }
        else if(this.chldorparnt.name == 'parent') {
          tostring +=' ['+ this.chldorparntcon1.name + '.' + this.connecttablecon1.name + '.' + this.connecpropcon1.name + '] ';
        }
    
        this.chssourcecon1 = undefined;
        this.chldorparntcon1 = undefined;
        this.connecttablecon1 = undefined;
        this.connecpropcon1 = undefined;
        this.connecfunccon1 = undefined;
      }
      else{
        if(this.chldorparntcon2.name == 'child'){
          tostring +=' ['+ this.chldorparntcon2.name + '.' + this.connecttablecon2.name + '.' + this.connecpropcon2.name + '.' + this.connecfunccon2.name + '] ';
        }
        else if(this.chldorparntcon2.name == 'parent') {
          tostring +=' ['+ this.chldorparntcon2.name + '.' + this.connecttablecon2.name + '.' + this.connecpropcon2.name + '] ';
        }
    
        this.chssourcecon2= undefined;
        this.chldorparntcon2 = undefined;
        this.connecttablecon2 = undefined;
        this.connecpropcon2 = undefined;
        this.connecfunccon2 = undefined;
      }

      return tostring;
     
    }
    else{
      if(this.selectedcondition){
        var constring = this.calculationString.split('$').filter((x : string) => x.includes(this.selectedcondition))[0];

        if(this.chldorparnt.name == 'child'){
          var constringvalue = constring + ' ['+ this.chldorparnt.name + '.' + this.connecttable.name + '.' + this.connecprop.name + '.' + this.connecfunc.name + '] ';
          this.calculationString = this.calculationString.replace(constring,constringvalue);
        }
        else if(this.chldorparnt.name == 'parent') {
          var constringvalue = constring + ' ['+ this.chldorparnt.name + '.' + this.connecttable.name + '.' + this.connecprop.name + '] ';
          this.calculationString = this.calculationString.replace(constring,constringvalue);
        }

      }
      else{
        if(this.chldorparnt.name == 'child'){
          this.calculationString +=' ['+ this.chldorparnt.name + '.' + this.connecttable.name + '.' + this.connecprop.name + '.' + this.connecfunc.name + '] ';
        }
        else if(this.chldorparnt.name == 'parent') {
          this.calculationString +=' ['+ this.chldorparnt.name + '.' + this.connecttable.name + '.' + this.connecprop.name + '] ';
        }
      }

  
      this.chssource = undefined;
      this.chldorparnt = undefined;
      this.connecttable = undefined;
      this.connecprop = undefined;
      this.connecfunc = undefined;
    }

    return undefined;

   

  }

  AddManual(tostring : string | undefined = "x", part1 : boolean | undefined = undefined) : string | undefined{

    if(tostring != "x"){
      if(part1 == true){
        tostring +=' '+ this.manualcon1 + ' ';
        this.chssourcecon1 = undefined;
        this.manualcon1 = undefined;
      }
      else{
        tostring +=' '+ this.manualcon2 + ' ';
        this.chssourcecon2 = undefined;
        this.manualcon2 = undefined;
      }

      return tostring;

    }
    else{
      if(this.selectedcondition){
        var constring = this.calculationString.split('$').filter((x : string) => x.includes(this.selectedcondition))[0];
        var constringvalue = constring + ' '+ this.manual + ' ';
        this.calculationString = this.calculationString.replace(constring,constringvalue);
      }
      else{
        this.calculationString +=' '+ this.manual + ' ';
      }
      this.chssource = undefined;
      this.manual = undefined;
    }

    return undefined;


  }

  RemoveFromCalc(event : any){
    var lastcalc : string | undefined = this.calculationString.split(' ').filter(x => x != ' ' && x != '').pop();
    console.log(lastcalc);
    if(lastcalc){
    this.calculationString = this.calculationString.replace(lastcalc,'');
    }
    event.emit();
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
