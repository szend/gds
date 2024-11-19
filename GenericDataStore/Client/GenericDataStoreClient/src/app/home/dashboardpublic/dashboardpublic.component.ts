import { ChangeDetectorRef, Component, ElementRef, EventEmitter, OnInit, Output, QueryList, ViewChild, ViewChildren } from '@angular/core';
import { DialogService, DynamicDialogModule, DynamicDialogRef } from 'primeng/dynamicdialog';
import { ObjecttypeCreateComponent } from '../../Components/objecttype-create/objecttype-create.component';
import { ApiService } from '../../Services/api.service';
import { AuthService } from '../../Services/auth.service';
import { ListViewComponent } from '../list-view/list-view.component';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AnimateOnScrollModule } from 'primeng/animateonscroll';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { DataViewModule } from 'primeng/dataview';
import { DialogModule } from 'primeng/dialog';
import { TagModule } from 'primeng/tag';
import { FooterComponent } from '../footer/footer.component';
import { DataobjectListComponent } from '../../Components/dataobject-list/dataobject-list.component';
import { FormsModule } from '@angular/forms';
import { DropdownModule } from 'primeng/dropdown';
import { ChartModule } from 'primeng/chart';
import { MenuItem, TreeNode } from 'primeng/api';
import { RootFilter } from '../../Models/Parameters';
import { DragDropModule } from 'primeng/dragdrop';
import { TooltipModule } from 'primeng/tooltip';
import { MenubarModule } from 'primeng/menubar';
import { SplitterModule } from 'primeng/splitter';
import { SpeedDialModule } from 'primeng/speeddial';
import { Chart } from 'chart.js';
import { InputSwitchModule } from 'primeng/inputswitch';
import { ProgressBarModule } from 'primeng/progressbar';
import { Title, Meta } from '@angular/platform-browser';
@Component({
  selector: 'app-dashboardpublic',
  standalone: true,
  imports: [ButtonModule, DialogModule, DynamicDialogModule, CommonModule,CardModule,DataViewModule,TooltipModule,MenubarModule,SplitterModule,SpeedDialModule,InputSwitchModule,
    ListViewComponent,TagModule,AnimateOnScrollModule,FooterComponent, DataobjectListComponent,CommonModule,FormsModule,DropdownModule,ChartModule,DragDropModule,ProgressBarModule
   ],
 providers: [DialogService],
  templateUrl: './dashboardpublic.component.html',
  styleUrl: './dashboardpublic.component.css'
})
export class DashboardpublicComponent   implements OnInit {
  documentStyle = getComputedStyle(document.documentElement);
  textColor = this.documentStyle.getPropertyValue('--text-color');
  textColorSecondary = this.documentStyle.getPropertyValue('--text-color-secondary');
  surfaceBorder = this.documentStyle.getPropertyValue('--surface-border');
  selectedNodes!: TreeNode[];
  constructor(private titleService: Title,private meta: Meta,protected route: ActivatedRoute, public apiService: ApiService, protected router: Router, protected changeDetector: ChangeDetectorRef, protected dialogService: DialogService,public authService: AuthService) 
  {
  }


  ref: DynamicDialogRef | undefined;
  public types : any[] = [];
  @ViewChild('widgetsContent') widgetsContent: ElementRef | undefined;
  @ViewChild('widgetsContent2') widgetsContent2: ElementRef | undefined;
  @ViewChild('widgetsContent3') widgetsContent3: ElementRef | undefined;

  @ViewChildren('table') dataobjectlist:QueryList<DataobjectListComponent> | undefined;

  rootFilter : RootFilter | undefined;
  public data : any[] = [];
  connecterrorvisible : boolean = false;
  editchartvisible : boolean = false;
  loading : boolean = false;
  selectedoption: any | undefined = {
    label: 'Pie', value : 'pie'
  }

  id : string | undefined | null;
  chartmenuitems : MenuItem[][] = []


  typeoptions = [
    {label: 'Line', value: 'line'},
    {label: 'Bar', value: 'bar'},
    {label: 'Scatter', value: 'scatter'},
    {label: 'Radar', value: 'radar'},
    {label: 'Pie', value: 'pie'},
    {label: 'Doughnut', value: 'doughnut'},
    {label: 'PolarArea', value: 'polarArea'}
  ];


  ngOnInit() {
    this.titleService.setTitle("Public dashboard"); 
    this.meta.updateTag({ name: 'description', content: "Public dashboard" });
    this.meta.updateTag({ name: 'keywords', content: "Public dashboard" });  
    this.Refresh()
  }

  Refresh(all : boolean = true){
    this.loading = true;
    this.route.paramMap.subscribe(params => {     
      this.id = params.get("id");
      if(this.id){
        this.apiService.GetDashboardDataOther(this.id).subscribe(x => {
          this.data = x;
          this.CreateMenu();
          this.loading = false;
        });
      }
   }); 
    if(all){
      this.connections = [];
    }

      
  }



  CreateMenu(){
    let idx = 0;
    this.chartmenuitems = [];
    this.data.forEach((element : any) => {
      let menuitems : MenuItem[] = 
      [
        {
          label: 'Delete',
          icon: 'pi pi-trash redicon',
          command: () => {
            this.RemoveChart(element);}
        },
        {
          icon: 'pi pi-chart-line',
          command: () => {
              this.ChangeChartType({value : {value : 'line'}},element.chart,idx);
              this.ReloadChart(element);
          }
        },
        {
          label: 'Bar',
          icon: 'pi pi-chart-bar',
          command: () => {
              this.ChangeChartType({value : {value : 'bar'}},element.chart,idx);
              this.ReloadChart(element);
          }
        },
        {
          label: 'Scatter',
          icon: 'pi pi-chart-scatter',
          command: () => {
              this.ChangeChartType({value : {value : 'scatter'}},element.chart,idx);
              this.ReloadChart(element);
          }
        },
        {
          label: 'Radar',
          icon: 'pi pi-chevron-circle-down',
          command: () => {
              this.ChangeChartType({value : {value : 'radar'}},element.chart,idx);
              this.ReloadChart(element);
          }
        },
        {
          label: 'Pie',
          icon: 'pi pi-chart-pie',
          command: () => {
              this.ChangeChartType({value : {value : 'pie'}},element.chart,idx);
              this.ReloadChart(element);
          }
        },
        {
          label: 'Doughnut',
          icon: 'pi pi-bullseye',
          command: () => {
              this.ChangeChartType({value : {value : 'doughnut'}},element.chart,idx);
              this.ReloadChart(element);
          }
        },
        {
          label: 'PolarArea',
          icon: 'pi pi-arrows-alt',
          command: () => {
              this.ChangeChartType({value : {value : 'polarArea'}},element.chart,idx);
              this.ReloadChart(element);
          }
        }
    ];
      this.chartmenuitems.push(menuitems);
      idx++;
    });
  }


  CreateNew(){
      this.ref = this.dialogService.open(ObjecttypeCreateComponent,  { data: {}, header: 'Create new table', resizable: true});
      this.ref.onClose.subscribe(x => {
        this.authService.newlogin.emit(true);
      });
  }

  RemoveChart(element : any){
this.apiService.RemoveFromDashboardChart(element.chartInput.id).subscribe(x => {
  const index = this.data.indexOf(element);
  if (index > -1) { 
    this.data.splice(index, 1); 
    this.CreateMenu();
  }

})
  }

  scrollLeft(){
    if(this.widgetsContent)
    this.widgetsContent.nativeElement.scrollLeft -= 150;
  }

  scrollRight(){
    if(this.widgetsContent)
    this.widgetsContent.nativeElement.scrollLeft += 150;
  console.log('df');
  }

  scrollLeft2(){
    if(this.widgetsContent2)
    this.widgetsContent2.nativeElement.scrollLeft -= 150;
  }

  scrollRight2(){
    if(this.widgetsContent2)
    this.widgetsContent2.nativeElement.scrollLeft += 150;
    console.log('df');

  }

  
  scrollLeft3(){
    if(this.selectedindex > 0){
      this.selectedindex--;
    }
    else{
      this.selectedindex = this.data.filter(x => x.type == 'table').length - 1;
    }

  }

  scrollRight3(){
    if(this.selectedindex < this.data.filter(x => x.type == 'table').length - 1){
      this.selectedindex++;
    }
    else{
      this.selectedindex = 0;
    }
  }


  RefreshChart(event : any, table : any){
    let tableconections = this.connections.filter(x => x.table == table.objectType.dashboardTableId);
    tableconections.forEach(element => {
          this.data.find(x => x.chartInput.id == element.chart).chartInput.filter = event;
          this.data.find(x => x.chartInput.id == element.chart).chartInput.liveMode = true;
    this.apiService.CreateCalculatedChart(this.data.find(x => x.chartInput.id == element.chart).chartInput).subscribe(x => {
      this.data.find(x => x.chartInput.id == element.chart).chart = x;
    })
    });



  }

  ReloadChart(element : any){
    var dset = element.chart;
    element.chart = undefined;
    this.changeDetector.detectChanges();
    element.chart = dset;
    this.changeDetector.detectChanges();
  }

  ReorderChart(old_index : number, new_index : number) {

    var olddata = this.data[old_index];
    var newdata = this.data[new_index];
    this.data[old_index] = newdata;
    this.data[new_index] = olddata;

    console.log(this.data);

    this.CreateMenu();

};

SaveDashboard(){
  this.data.forEach(element => {
    console.log(element);
    if(element.type == 'table'){
      this.apiService.SaveDashboard(element.objectType.dashboardTableId,element.size,element.position).subscribe(x => {

      });
    }
    else if(element.type == 'chart'){
      this.apiService.SaveChartDashboard(element.chartInput.id,element.size,element.position).subscribe(x => {});
    }
  })
}


  dragchart : any | undefined;
  connections: any[] = [];
dragStart(chart: any) {
  console.log(this.dragchart);

    this.dragchart = chart;
}

drop(table : any) {
       let chartfilter : RootFilter = this.dragchart.chartInput.filter;
       let tablefilter : RootFilter = JSON.parse(table.filter);
       if(chartfilter.filters.filter(x => x.field == 'ObjectTypeId')[0].value != tablefilter.filters.filter(x => x.field == 'ObjectTypeId')[0].value ){
        this.connecterrorvisible = true;
       }
       else{
        if(!table.connectedcharts){
          table.connectedcharts = [];
         }
         
         table.connectedcharts.push(this.dragchart);
        //  this.leader(this.dragchart.chartInput.id,table.objectType.dashboardTableId)
         this.connections.push({chart: this.dragchart.chartInput.id, table : table.objectType.dashboardTableId});
       }

    
}

droptoChart(tochart : any) {

  if(this.dragchart == tochart){
    return;
  }
  this.dragchart.chart.datasets.forEach((element2: any) => {

    let dset = {
      data: element2.data,
      label: element2.label,
      backgroundColor: element2.backgroundColor,
      borderColor: element2.borderColor,
      borderWidth: element2.borderWidth,
      type: this.dragchart.chartInput.type
    }
    tochart.chart.datasets.push(dset);   
    console.log(element2);
          
});
  // tochart.chart.datasets.push(this.dragchart.chart.datasets[0]);
  if(tochart.chartInput.type == 'pie' || tochart.chartInput.type == 'doughnut' || tochart.chartInput.type == 'polarArea'){
    tochart.chartInput.combined = true;
    console.log(tochart.chartInput);
  }
  this.ReloadChart(tochart);
}

dragEnd() {

}

selectedindex: number = 0;
IsSelectedData(element: any){
  var index = this.data.indexOf(element);
  if(this.data.filter(x => x.type == 'table').length > 0){
    if(this.data.filter(x => x.type == 'table')[this.selectedindex].objectType.dashboardTableId == element.objectType.dashboardTableId){
      return true;
    }
    else{
      return false;
    }

  }
  else{
    return false;
  }
}

chartsize : number = 9;
datasize : number = 3;

MouseOver(){
  this.chartsize = 3;
  this.datasize = 9;


}

MouseLeave(){
  this.chartsize = 9;
  this.datasize = 3;

}

// leader(id : any, id2 : any) {
//   let startEl = document.getElementById(id);
//       let endEl = document.getElementById(id2);


//   return new LeaderLine(
//     LeaderLine.mouseHoverAnchor(startEl),
//     endEl, {
//       endPlugOutline: false,
//       animOptions: { duration: 3000, timing: 'linear' }
//     }
//   );
// }



ChangeChartType(event: any,chart: any, i: number,child : boolean = false){

  if(event.value.value == 'treemap'){
      chart.type = event.value.value;   

  }
  else{
      if(!chart.label){
          chart.label = '';
      }
      chart.type = event.value.value;
      this.optionsnum.scales = {x: this.optionsnum.scales.x, y: this.optionsnum.scales.y};
      this.optionsopt.scales = {x: this.optionsnum.scales.x, y: this.optionsnum.scales.y};       
  if(chart.datasets?.length > 0){
      chart.datasets.forEach((element: any) => {
          console.log(element);
          console.log(event);
          element.type = event.value.value;
      });
  }
  chart.type = event.value.value;
     
  }
}

hidestuff : boolean = true;
HideLabels(){
  this.hidestuff = !this.hidestuff;
  this.optionsnum.plugins.legend.display = !this.optionsnum.plugins.legend.display;
  this.optionsopt.plugins.legend.display = !this.optionsopt.plugins.legend.display;
  this.data.filter(x => x.type == 'chart').forEach((element: any) => {
    this.ReloadChart(element);
  });


}

RefreshChartByChart(event : any, table : any){
  console.log(event);
   var ch = this.data.find(x => x.chartInput.id == event.chartInput.id).chart = event.chart;




}

ChartClickOption(event: {element:{ datasetIndex: number, index : number}},chart: any, i: number){


  var filter = chart.chartInput.filter;

  let tableconections = this.connections.filter(x => x.chart == chart.chartInput.id);

  tableconections.forEach(element => {
        console.log(filter);
        console.log(chart.chart.privateobject)

        var valueflter = chart.chart.labels[event.element.index].toString();
        var list = this.dataobjectlist?.find(x => x.data.dashboardTableId == element.table);
        if(list){
          let tableconections = this.connections.filter(x => x.table == element.table).map(x => x.chart);

          list?.DashboardRefresh(chart.chartInput,valueflter,tableconections);
        }
  });
  

  // if(tableconections.length == 0){
  //   localStorage.setItem('filter', JSON.stringify(filter));


  //   localStorage.setItem('filterfull', JSON.stringify(filter));
  
  //   if(chart.chart.privateobject){
  //     localStorage.setItem('private', chart.chart.privateobject.toString());
  // }
  //   this.ref = this.dialogService.open(DataobjectListComponent,  { data: {filter: filter}});
  //   this.ref.onClose.subscribe(x => {
  //     localStorage.removeItem('filterfull');
  //   });
  // }

 
}

ChartClickNum(event: any,chart: any, i: number){
console.log(event);
console.log(chart);

  var filter = chart.chartInput.filter;

  let tableconections = this.connections.filter(x => x.chart == chart.chartInput.id);

  tableconections.forEach(element => {
        console.log(filter);
        console.log(chart.chart.privateobject)

        var valueflter = chart.chart.datasets[event.element.datasetIndex].data[event.element.index].toString();
        console.log(valueflter);
        var list = this.dataobjectlist?.find(x => x.data.dashboardTableId == element.table);
        if(list){
          let tableconections = this.connections.filter(x => x.table == element.table).map(x => x.chart);

          list?.DashboardRefresh(chart.chartInput,valueflter,tableconections,chart.chart.datasets[event.element.datasetIndex].label);
        }
  });
  

  // if(tableconections.length == 0){
  //   localStorage.setItem('filter', JSON.stringify(filter));


  //   localStorage.setItem('filterfull', JSON.stringify(filter));
  
  //   if(chart.chart.privateobject){
  //     localStorage.setItem('private', chart.chart.privateobject.toString());
  // }
  //   this.ref = this.dialogService.open(DataobjectListComponent,  { data: {filter: filter}});
  //   this.ref.onClose.subscribe(x => {
  //     localStorage.removeItem('filterfull');
  //   });
  // }

 
}


Stacked(){
  this.optionsnum.scales = {x: this.optionsnum.scales.x, y: this.optionsnum.scales.y};
  this.optionsnum.scales.y.stacked = !this.optionsnum.scales.y.stacked;
  this.optionsnum.scales.x.stacked = !this.optionsnum.scales.x.stacked;
  this.data.filter(x => x.type == 'chart' && (x.chartInput.type == 'line' || x.chartInput.type == 'bar' || x.chartInput.type == 'scatter')).forEach((element: any) => {
    this.ReloadChart(element);
  });
  
}


  optionsopt = {
    scales: {

    },
    plugins: {
        legend: {
          display: true,
            labels: {
                usePointStyle: true,
                color: this.textColor
            }
        }
    }
};

public optionsoptcombined = {
  legend:{
      display: true
  },
  plugins: {
      legend: {
        labels: {
          generateLabels: function(chart : any) {
            console.log(chart);
            // Get the default label list
            const original = Chart.overrides.pie.plugins.legend.labels.generateLabels;
            const labelsOriginal : any = original.call(this, chart);

            // Build an array of colors used in the datasets of the chart
            let datasetColors = chart.data.datasets.map(function(e : any) {
              return e.backgroundColor;
            });
            datasetColors = datasetColors.flat();
            var datacounts = chart.data.datasets.map(function(e : any) {
              return e.data.length;
            });
            // Modify the color and hide state of each label
            var idx = 0;	
            labelsOriginal.forEach((label: { datasetIndex: number; index: number; hidden: boolean; fillStyle: any; }) => {
              var labelidx = 0;
              var countidx = 0;
              var result = 0;
             datacounts.forEach((element: any) => {
              for(let i = 0; i < element; i++){
                  if(labelidx == label.index){
                      result = countidx;
                  }
                  labelidx++;

              }
              countidx++;
             });
              
              // There are twice as many labels as there are datasets. This converts the label index into the corresponding dataset index
              label.datasetIndex = result;
              

              // The hidden state must match the dataset's hidden state
              label.hidden = !chart.isDatasetVisible(label.datasetIndex);

              // Change the color to match the dataset
              label.fillStyle = datasetColors[label.index];
              idx++;
            });
            
            return labelsOriginal;
          }
        },
        onClick: function(mouseEvent : any, legendItem : any, legend : any) {
          // toggle the visibility of the dataset from what it currently is
          legend.chart.getDatasetMeta(
            legendItem.datasetIndex
          ).hidden = legend.chart.isDatasetVisible(legendItem.datasetIndex);
          legend.chart.update();
        }
      },
      tooltip: {
          callbacks: {
            label: function(context : any) {
              var datacounts = context.chart.data.datasets.map(function(e : any) {
                  return e.data.length;
                });
                var idx = context.dataIndex;
                if(context.datasetIndex > 0){
                  for(let i = 0; i < context.datasetIndex; i++){
                      idx += datacounts[i];
                  }
                }
               
              const labelIndex = idx;
              return context.chart.data.labels[labelIndex] + ': ' + context.formattedValue;
            },
            title: function(context : any) {
              return '';
            }
          }
        }
    },
    scales:{
      
    }
};



  public optionsnum = {
    maintainAspectRatio: false,
    aspectRatio: 0.6,
    plugins: {
        legend: {
          display: true,
            labels: {
               // color: this.textColor
            }
        }
    },
    scales: {
        x: {
          position : 'bottom',
          stacked : false,
            ticks: {
                color: this.textColorSecondary
            },
            grid: {
                color: this.surfaceBorder,
                drawBorder: false
            }
        },
        y: {
          
          position : 'left',
          stacked : false,
            ticks: {
                color: this.textColorSecondary
            },
            grid: {
                color: this.surfaceBorder,
                drawBorder: false
            }
        }
    }
  };
}

