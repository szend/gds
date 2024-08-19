import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ConfirmationService, MessageService } from 'primeng/api';
import { CardModule } from 'primeng/card';
import { ChartModule } from 'primeng/chart';
import { DynamicDialogConfig, DialogService } from 'primeng/dynamicdialog';
import { OrganizationChartModule } from 'primeng/organizationchart';
import { TabViewModule } from 'primeng/tabview';
import { ApiService } from '../../Services/api.service';
@Component({
  selector: 'app-clusters',
  standalone: true,
  imports: [TabViewModule,ChartModule, CommonModule,CardModule,OrganizationChartModule],
  templateUrl: './clusters.component.html',
  styleUrl: './clusters.component.css'
})
export class ClustersComponent implements OnInit {

  constructor(private confirmationService: ConfirmationService, public apiService: ApiService, protected changeDetector: ChangeDetectorRef, protected config: DynamicDialogConfig, private messageService: MessageService, protected dialogService: DialogService) {
  }
  documentStyle = getComputedStyle(document.documentElement);
  textColor = this.documentStyle.getPropertyValue('--text-color');
  textColorSecondary = this.documentStyle.getPropertyValue('--text-color-secondary');
  surfaceBorder = this.documentStyle.getPropertyValue('--surface-border');

  public dataRec: any;
  public dimensions: number | undefined;

 data = {
  datasets: [
    {
      label: 'Dataset 1',
      data: [[0.3,0.5]],
      borderColor: '#ff0000',
      backgroundColor: '#ff0000',
    }
  ]
};

 chconfig = {
  type: 'scatter',
  data: [[0.3,0.5,9.9]],
  options: {
    scales: {
      x: {
        type: 'linear',
        position: 'bottom'
      },
      z: {
        type: 'linear',
        position: 'bottom'
      }
    }
  }
};

chartdata: any[] = [];
headers: string[] = [];

  ngOnInit(): void {
    if(this.config.data.dataRec){
      this.dataRec = this.config.data.dataRec;
      if(this.dataRec.datasets.length < 2){
        this.dimensions = 0;
      }
      else{
        this.dimensions = (this.dataRec.datasets.length - 1) * (this.dataRec.datasets.length) / 2;
      }
      let idx1 = 0;
      let idx2 = 1;
      for (let index = 0; index < this.dimensions; index++) {
        let tempdata : {datasets : any[]} =  {datasets: []}


        this.dataRec.datasets.forEach((element: any) => { 
          let newdata : any[] = [];
          element.data.forEach((x: any) => {
            newdata.push([x[idx1],x[idx2]]);
          });
          let newset :any = { };
          newset.label = element.label;
          newset.data = newdata;
          newset.borderColor = element.borderColor;
          newset.backgroundColor = element.backgroundColor;
          tempdata.datasets.push(newset);
        });
        this.headers.push('Cluster ' + (idx1 + 1) + ' vs Cluster ' + (idx2 + 1));
        if(idx2 < this.dataRec.datasets.length -1){
          idx2++;
        }
        else{
          idx1++;
          idx2 = idx1 + 1;
        }
        this.chartdata.push({
         datasets: tempdata.datasets
        });

      }



    }
  }
}
