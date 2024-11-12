import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ConfirmationService, MessageService, TreeNode } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DropdownModule } from 'primeng/dropdown';
import { DynamicDialogConfig, DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { InputGroupModule } from 'primeng/inputgroup';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { KeyFilterModule } from 'primeng/keyfilter';
import { MessagesModule } from 'primeng/messages';
import { TriStateCheckboxModule } from 'primeng/tristatecheckbox';
import { ApiService } from '../../Services/api.service';
import { RootFilter } from '../../Models/Parameters';
import { CardModule } from 'primeng/card';
import { ChartModule } from 'primeng/chart';
import { ProgressSpinner, ProgressSpinnerModule } from 'primeng/progressspinner';
import { ProgramDescriptionComponent } from '../../home/program-description/program-description.component';
import { SplitterModule } from 'primeng/splitter';
import { StepperModule } from 'primeng/stepper';

@Component({
  selector: 'app-chart-create',
  standalone: true,
  imports: [InputGroupModule, FormsModule,CommonModule,TriStateCheckboxModule,ConfirmDialogModule,DropdownModule,
    InputTextModule,InputNumberModule,ButtonModule,MessagesModule,CheckboxModule,KeyFilterModule,InputTextareaModule,CardModule,ChartModule,ProgressSpinnerModule,
    StepperModule,SplitterModule
  ],
  templateUrl: './chart-create.component.html',
  styleUrl: './chart-create.component.css'
})
export class ChartCreateComponent  implements OnInit{
  documentStyle = getComputedStyle(document.documentElement);
  textColor = this.documentStyle.getPropertyValue('--text-color');
  textColorSecondary = this.documentStyle.getPropertyValue('--text-color-secondary');
  surfaceBorder = this.documentStyle.getPropertyValue('--surface-border');
  selectedNodes!: TreeNode[];
  constructor(private ref: DynamicDialogRef, private confirmationService: ConfirmationService,public apiService: ApiService, protected changeDetector: ChangeDetectorRef,  protected config: DynamicDialogConfig,private messageService: MessageService
    , protected dialogService: DialogService
  ) {
  }


  xconditions: any[] = [];
  yconditions: any[] = [];

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

  xselectedcondition : any | undefined;
  yselectedcondition : any | undefined;



  loadnig : boolean = false;

  groupoption = [
    {label: 'None', value: 'none'},
    {label: 'Count', value: 'count'},
    {label: 'Sum', value: 'sum'},
    {label: 'Average', value: 'average'},
    {label: 'Min', value: 'min'},
    {label: 'Max', value: 'max'},
    {label: 'First', value: 'first'},
    {label: 'Last', value: 'last'},
  ];

  selectedgroupoption: any =  {label: 'None', value: 'none'};

  public optionsnum : any | undefined;

  selectedoption: any | undefined;
  ycalculation : string = "";
  xcalculation : string = "";
  id : string = "";
  filter: RootFilter | undefined;
  cahrtdata: any | undefined;
  type: string | undefined;
  fields : any | undefined;
  typeoptions : any[] = [];
  partenttables: any[] | undefined;
childtables: any[] | undefined;

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

  ngOnInit(): void {
    this.id = this.config.data.id;
    this.apiService.GetConnectedTables(this.id ?? '').subscribe(x => {
      this.partenttables = x.parents;
      this.childtables = x.childs;
    });
    this.fields = this. config.data.fields;
    this.filter = this.config.data.filter
    this.type = this.config.data.charttype
    this.typeoptions = this.config.data.typeoptions;
    this.optionsnum = this.config.data.options;
    this.selectedoption = this.typeoptions[0];
    console.log(this.fields);

  }


  AddOperator(op : any, tostring: string | undefined = "x", xcalc : boolean = true) : string | undefined{
    if(tostring != "x"){
      console.log(tostring);
      tostring += this.conselectedoperator.code;
      this.conselectedoperator = undefined;
      return tostring;
    }
    else{
      if(xcalc == true){
        if(this.xselectedcondition){
          var constring = this.xcalculation.split('$').filter((x : string) => x.includes(this.xselectedcondition))[0];
          var constringvalue = constring + op.value.code;
          this.xcalculation = this.xcalculation.replace(constring,constringvalue);
        }
        else{
          this.xcalculation += op.value.code;
        }
      }
      else{
        if(this.yselectedcondition){
          var constring = this.ycalculation.split('$').filter((x : string) => x.includes(this.yselectedcondition))[0];
          var constringvalue = constring + op.value.code;
          this.ycalculation = this.ycalculation.replace(constring,constringvalue);
        }
        else{
          this.ycalculation += op.value.code;
        }
      }


      op = undefined;
    }
    return undefined;

  }

  AddCondition(xcalc : boolean = true){
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

    if(xcalc == true){
      this.xconditions.push(constring);
      this.xcalculation += "$ if'(" + constring + ")' ??  $";
    }
    else{    
      this.yconditions.push(constring);
      this.ycalculation += "$ if'(" + constring + ")' ??  $";

    }


  }

  AddFromProp(tostring : string | undefined = "x", part1 : boolean | undefined = undefined, xcalc : boolean = true) : string | undefined{
    if(tostring != "x"){
      if(part1 == true){
        tostring +=' {'+ this.propcon1.header + '} ';

        this.chssourcecon1 = undefined;
        this.propcon1 = undefined;
      }
      else{
        tostring +=' {'+ this.propcon2.header + '} ';

        this.chssourcecon2 = undefined;
        this.propcon2 = undefined;
      }

      
      return tostring;

    }
    else{

      if(xcalc == true){
        if(this.xselectedcondition){
          var constring = this.xcalculation.split('$').filter((x : string) => x.includes(this.xselectedcondition))[0];
          var constringvalue = constring + ' {'+ this.prop.header + '} ';
  
          this.xcalculation = this.xcalculation.replace(constring,constringvalue);
        }
        else{
          this.xcalculation +=' {'+ this.prop.header + '} ';
        }
  
      }
      else{
        if(this.yselectedcondition){
          var constring = this.ycalculation.split('$').filter((x : string) => x.includes(this.yselectedcondition))[0];
          var constringvalue = constring + ' {'+ this.prop.header + '} ';
  
          this.ycalculation = this.ycalculation.replace(constring,constringvalue);
        }
        else{
          this.ycalculation +=' {'+ this.prop.header + '} ';
        }
  
      }
     
      this.chssource = undefined;
      this.prop = undefined;
    }
    return undefined;

  }

  AddFromTable(tostring : string | undefined = "x", part1 : boolean | undefined = undefined, xcalc : boolean = true) : string | undefined{
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

      if(xcalc == true){

        if(this.xselectedcondition){
          var constring = this.xcalculation.split('$').filter((x : string) => x.includes(this.xselectedcondition))[0];
  
          if(this.chldorparnt.name == 'child'){
            var constringvalue = constring + ' ['+ this.chldorparnt.name + '.' + this.connecttable.name + '.' + this.connecprop.name + '.' + this.connecfunc.name + '] ';
            this.xcalculation = this.xcalculation.replace(constring,constringvalue);
          }
          else if(this.chldorparnt.name == 'parent') {
            var constringvalue = constring + ' ['+ this.chldorparnt.name + '.' + this.connecttable.name + '.' + this.connecprop.name + '] ';
            this.xcalculation = this.xcalculation.replace(constring,constringvalue);
          }
  
        }
        else{
          if(this.chldorparnt.name == 'child'){
            this.xcalculation +=' ['+ this.chldorparnt.name + '.' + this.connecttable.name + '.' + this.connecprop.name + '.' + this.connecfunc.name + '] ';
          }
          else if(this.chldorparnt.name == 'parent') {
            this.xcalculation +=' ['+ this.chldorparnt.name + '.' + this.connecttable.name + '.' + this.connecprop.name + '] ';
          }
        }
      }
      else{
        
      if(this.yselectedcondition){
        var constring = this.ycalculation.split('$').filter((x : string) => x.includes(this.yselectedcondition))[0];

        if(this.chldorparnt.name == 'child'){
          var constringvalue = constring + ' ['+ this.chldorparnt.name + '.' + this.connecttable.name + '.' + this.connecprop.name + '.' + this.connecfunc.name + '] ';
          this.ycalculation = this.ycalculation.replace(constring,constringvalue);
        }
        else if(this.chldorparnt.name == 'parent') {
          var constringvalue = constring + ' ['+ this.chldorparnt.name + '.' + this.connecttable.name + '.' + this.connecprop.name + '] ';
          this.ycalculation = this.ycalculation.replace(constring,constringvalue);
        }

      }
      else{
        if(this.chldorparnt.name == 'child'){
          this.ycalculation +=' ['+ this.chldorparnt.name + '.' + this.connecttable.name + '.' + this.connecprop.name + '.' + this.connecfunc.name + '] ';
        }
        else if(this.chldorparnt.name == 'parent') {
          this.ycalculation +=' ['+ this.chldorparnt.name + '.' + this.connecttable.name + '.' + this.connecprop.name + '] ';
        }
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

  AddManual(tostring : string | undefined = "x", part1 : boolean | undefined = undefined, xcalc : boolean = true) : string | undefined{

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

      if(xcalc == true){
        if(this.xselectedcondition){
          var constring = this.xcalculation.split('$').filter((x : string) => x.includes(this.xselectedcondition))[0];
          var constringvalue = constring + ' '+ this.manual + ' ';
          this.xcalculation = this.xcalculation.replace(constring,constringvalue);
        }
        else{
          this.xcalculation +=' '+ this.manual + ' ';
        }
      }
      else{
        if(this.yselectedcondition){
          var constring = this.ycalculation.split('$').filter((x : string) => x.includes(this.yselectedcondition))[0];
          var constringvalue = constring + ' '+ this.manual + ' ';
          this.ycalculation = this.ycalculation.replace(constring,constringvalue);
        }
        else{
          this.ycalculation +=' '+ this.manual + ' ';
        }
      }

      this.chssource = undefined;
      this.manual = undefined;
    }

    return undefined;


  }





  Save(){
    this.loadnig = true;
    let chartcalculation = {
      xcalculation : this.xcalculation,
      ycalculation : this.ycalculation,
      id: this.id,
      type: this.selectedoption.value,
      filter: this.filter,
      groupOption: this.selectedgroupoption.value
    }
    this.apiService.CreateCalculatedChart(chartcalculation).subscribe((data: any) => {
      this.cahrtdata = data;
      this.loadnig = false;
    },(error: any) => {
      this.messageService.add({severity:'error', summary: 'Error', detail: 'Error in creating the chart'});
      this.loadnig = false;
    });

  }

  Add(){
    this.cahrtdata.xcalculation = this.xcalculation;
    this.cahrtdata.ycalculation = this.ycalculation;
    this.cahrtdata.groupOption = this.selectedgroupoption.value;
    
    let data = {
      chartdata: this.cahrtdata,
      type: this.selectedoption.value,
    }
   this.ref.close(data);
  }

  AddToDashboard(){
    this.loadnig = true;
    let chartcalculation = {
      xcalculation : this.xcalculation,
      ycalculation : this.ycalculation,
      objectTypeId: this.id,
      type: this.selectedoption.value,
      rootFilter: JSON.stringify(this.filter),
      groupOption: this.selectedgroupoption.value
    }
    this.apiService.AddToDashboardChart(chartcalculation).subscribe(x =>{
      this.loadnig = false;
      this.messageService.add({severity:'success', summary: 'Ok', detail: 'Done'});
    },(error: any) => {
      this.messageService.add({severity:'error', summary: 'Error', detail: 'Error in saveing the chart'});
      this.loadnig = false;
    })
  }

  
  ShowDesc(){
    this.dialogService.open(ProgramDescriptionComponent, {});
  }
}
