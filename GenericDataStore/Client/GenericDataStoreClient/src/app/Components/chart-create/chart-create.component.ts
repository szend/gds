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

@Component({
  selector: 'app-chart-create',
  standalone: true,
  imports: [InputGroupModule, FormsModule,CommonModule,TriStateCheckboxModule,ConfirmDialogModule,DropdownModule,
    InputTextModule,InputNumberModule,ButtonModule,MessagesModule,CheckboxModule,KeyFilterModule,InputTextareaModule,CardModule,ChartModule,ProgressSpinnerModule
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

  typeoptions : any[] = [];

  ngOnInit(): void {
    this.id = this.config.data.id;
    this.filter = this.config.data.filter
    this.type = this.config.data.charttype
    this.typeoptions = this.config.data.typeoptions;
    this.optionsnum = this.config.data.options;
    this.selectedoption = this.typeoptions[0];

  }

  Save(){
    this.loadnig = true;
    let chartcalculation = {
      xcalculation : this.xcalculation,
      ycalculation : this.ycalculation,
      id: this.id,
      type: this.type,
      filter: this.filter,
      groupOption: this.selectedgroupoption.value
    }
    console.log(chartcalculation);
    this.apiService.CreateCalculatedChart(chartcalculation).subscribe((data: any) => {
      this.cahrtdata = data;
      this.loadnig = false;
    },(error: any) => {
      this.messageService.add({severity:'error', summary: 'Error', detail: 'Error in creating chart'});
      this.loadnig = false;
    });

  }
  Add(){
    let data = {
      chartdata: this.cahrtdata,
      type: this.selectedoption.value
    }
   this.ref.close(data);
  }

  
  ShowDesc(){
    this.dialogService.open(ProgramDescriptionComponent, {});
  }
}
