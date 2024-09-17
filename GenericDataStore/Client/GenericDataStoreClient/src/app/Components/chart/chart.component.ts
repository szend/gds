import { AfterViewInit, ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ConfirmationService, MessageService, TreeNode } from 'primeng/api';
import { DynamicDialogConfig, DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { ApiService } from '../../Services/api.service';
import { TabViewModule } from 'primeng/tabview';
import { ChartModule } from 'primeng/chart';
import { CommonModule } from '@angular/common';
import { CardModule } from 'primeng/card';
import { OrganizationChartModule } from 'primeng/organizationchart';
import { SliderModule } from 'primeng/slider';
import { FormsModule } from '@angular/forms';
import { CheckboxModule } from 'primeng/checkbox';
import { ButtonModule } from 'primeng/button';
import { Chart, scales } from 'chart.js';
import { DropdownModule } from 'primeng/dropdown';
import { ColorPickerModule } from 'primeng/colorpicker';
import regression from 'regression';
import { DataobjectCreateComponent } from '../dataobject-create/dataobject-create.component';
import { DataobjectListComponent } from '../dataobject-list/dataobject-list.component';
import { NgxEchartsModule, provideEcharts,NgxEchartsDirective } from 'ngx-echarts';
import { EChartsOption } from 'echarts';
import { ChartCreateComponent } from '../chart-create/chart-create.component';

@Component({
  selector: 'app-chart',
  standalone: true,
  imports: [NgxEchartsModule,NgxEchartsDirective,TabViewModule,ChartModule, CommonModule,CardModule,OrganizationChartModule,SliderModule,FormsModule, CheckboxModule, ButtonModule,DropdownModule,ColorPickerModule
    ],
    providers: [provideEcharts()],

  templateUrl: './chart.component.html',
  styleUrl: './chart.component.css'
})
export class ChartComponent implements OnInit, AfterViewInit{
   documentStyle = getComputedStyle(document.documentElement);
   textColor = this.documentStyle.getPropertyValue('--text-color');
   textColorSecondary = this.documentStyle.getPropertyValue('--text-color-secondary');
   surfaceBorder = this.documentStyle.getPropertyValue('--surface-border');
   selectedNodes!: TreeNode[];

  constructor( private confirmationService: ConfirmationService, public apiService: ApiService, protected changeDetector: ChangeDetectorRef, protected config: DynamicDialogConfig, private messageService: MessageService, protected dialogService: DialogService) {
  }



 


  ref: DynamicDialogRef | undefined;

  color: string[] = [];

  filloptions = [
    {label: 'Off', value: false},
    {label: 'Origin', value: 'origin'},
    {label: 'Start', value: 'start'},
    {label: 'End', value: 'end'}
  ];

  fillmoreoptions = [
    {label: 'Off', value: false},
    {label: 'Origin', value: 'origin'},
    {label: 'Start', value: 'start'},
    {label: 'End', value: 'end'},
    {label: '+1', value: '+1'},
    {label: '-1', value: '-1'},
    {label: '+2', value: '+2'},
    {label: '-2', value: '-2'}
  ];

  decioptions = [
    {label: 'Off', value: 1},
    {label: '50%', value: 2},
    {label: '33%', value: 3},
    {label: '25%', value: 4},	
    {label: '20%', value: 5},	
    {label: '10%', value: 10},	
    {label: '5%', value: 20},
    {label: '2%', value: 50},
    {label: '1%', value: 100},
    {label: '0.1%', value: 1000},
  ];

  regoptions = [
    {label: 'Off', value: false},
    {label: 'Linear', value: 'linear'},
    {label: 'Polynomial', value: 'polynomial'},
    {label: 'Exponential', value: 'exponential'},
    {label: 'Power', value: 'power'},
    {label: 'Logarithmic', value: 'logarithmic'}
  ];

  typeoptions = [
    {label: 'Line', value: 'line'},
    {label: 'Bar', value: 'bar'},
    {label: 'Scatter', value: 'scatter'},
    {label: 'Radar', value: 'radar'}
  ];

  typeoptionslogical = [
    {label: 'Pie', value: 'pie'},
    {label: 'Doughnut', value: 'doughnut'},
    {label: 'Bar', value: 'bar'},
    {label: 'Radar', value: 'radar'},
    {label: 'PolarArea', value: 'polarArea'}
  ];

  steppedoptions = [
    {label: 'Off', value: false},
    {label: 'Before', value: true},
    {label: 'After', value: 'after'},
    {label: 'Middle', value: 'middle'}
  ];



  
  
  optionsopt = {
    scales: {

    },
    plugins: {
        legend: {
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

size : number = 2;
name : string | undefined;
id : string | undefined;
private : boolean | undefined;


public optionsnum = {
  maintainAspectRatio: false,
  aspectRatio: 0.6,
  plugins: {
      legend: {
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

  public dataRec: any;
  public fields: any;

  ngAfterViewInit(): void {
    }



  ngOnInit(): void {
    if(this.config.data.dataRec){
        this.dataRec = this.config.data.dataRec;
        this.orgchart = this.CreateChartOrg();
      this.dataRec.numbers.forEach((element: any) => {
        element.datasets.forEach((element2: any) => {
            this.color.push(element2.backgroundColor[0]);
            if(element2.borderColor == undefined){
                element2.borderColor = this.color;
            }
            if(element2.backgroundColor == undefined){
                element2.backgroundColor = this.color;
            }
        });
      });
      if(this.config.data.name){
        this.name = this.config.data.name;
      }
      if(this.config.data.id){
        this.id = this.config.data.id;
      }
      if(this.config.data.private){
        this.name = this.config.data.private;
      }
      if(this.config.data.fields){
        this.fields = this.config.data.fields;
      }
    }
    

  } 

  orgchart : EChartsOption | undefined;
  onChartEvent(event: any) {
    if(event.data.name){
        localStorage.setItem('name', event.data.name);
    }
    if(event.data.objId){
        localStorage.setItem('id', event.data.objId);
    }
    if(event.data.private){
        localStorage.setItem('private', event.data.private.toString());
    }
    this.ref = this.dialogService.open(DataobjectListComponent,  { data: {filter: null}});
    this.ref.onClose.subscribe(x => {
      localStorage.removeItem('filter');
        localStorage.removeItem('name');
        localStorage.removeItem('id');
        localStorage.removeItem('private');

    });
   
}
CreateChartOrg() : EChartsOption {
   let orgoptions: EChartsOption = {
        title: {
          text: 'Connections',
        },
        tooltip: {},
        animationDurationUpdate: 1500,
        animationEasingUpdate: 'quinticInOut',
        series: [
          {
            type: 'graph',
            layout: 'none',
            symbolSize: 60,
            roam: true,
            label: {
              show: true,
            },
            edgeSymbol: ['circle', 'arrow'],
            edgeSymbolSize: [4, 10],
            edgeLabel: {
              fontSize: 20,
            },
            data: this?.dataRec.organisation.data,

            links: this.dataRec.organisation.links,
            lineStyle: {
              opacity: 0.9,
              width: 2,
              curveness: 0,
            },
          },
        ],
      };
        this.orgchart = orgoptions;
    return orgoptions;
       
    }



CreateChart(){
    let count = this.dataRec.numbers.filter((x : any) => x.checked == true).length;
    if(count >= 1){
        let first = this.dataRec.numbers.filter((x : any) => x.checked == true)[0];
        let newchart :{name: string, labels: string[], datasets: any[]} = {name: "New Chart", labels: first.labels, datasets: []};
        this.dataRec.numbers.filter((x : any) => x.checked == true).filter((x : any) => 
            (x.datasets[0].type == 'scatter')
    ).forEach((element: { datasets: any[], labels : string[], type: string }) => {
            if(element.labels.length > newchart.labels.length){
                newchart.labels = element.labels;
            }
            element.datasets.forEach((element2: any) => {
                element2.type = element.type;
                newchart.datasets.push(element2);           
            });
           });

        this.dataRec.numbers.filter((x : any) => x.checked == true).filter((x : any) => 
            (x.datasets[0].type == 'line' ||  x.datasets[0].type == undefined) && (x.datasets[0].fill == false || x.datasets[0].fill == undefined)
    ).forEach((element: { datasets: any[], labels : string[], type: string }) => {
            if(element.labels.length > newchart.labels.length){
                newchart.labels = element.labels;
            }
            element.datasets.forEach((element2: any) => {
                element2.type = element.type;
                newchart.datasets.push(element2);           
            });
           });

           this.dataRec.numbers.filter((x : any) => x.checked == true).filter((x : any) => 
            (x.datasets[0].type == 'bar')
    ).forEach((element: { datasets: any[], labels : string[], type: string }) => {
            if(element.labels.length > newchart.labels.length){
                newchart.labels = element.labels;
            }
            element.datasets.forEach((element2: any) => {
                element2.type = element.type;
                newchart.datasets.push(element2);           
            });
           });

           this.dataRec.numbers.filter((x : any) => x.checked == true).filter((x : any) => 
            (x.datasets[0].type == 'line' ||  x.datasets[0].type == undefined) && (x.datasets[0].fill != false && x.datasets[0].fill != undefined)
    ).forEach((element: { datasets: any[], labels : string[], type: string }) => {
            if(element.labels.length > newchart.labels.length){
                newchart.labels = element.labels;
            }
            element.datasets.forEach((element2: any) => {
                element2.type = element.type;
                newchart.datasets.push(element2);           
            });
           });

           this.dataRec.numbers.filter((x : any) => x.checked == true).filter((x : any) => 
            (x.datasets[0].type == 'radar') && (x.datasets[0].fill == false || x.datasets[0].fill == undefined)
    ).forEach((element: { datasets: any[], labels : string[], type: string }) => {
            if(element.labels.length > newchart.labels.length){
                newchart.labels = element.labels;
            }
            element.datasets.forEach((element2: any) => {
                element2.type = element.type;
                newchart.datasets.push(element2);           
            });
           });

           this.dataRec.numbers.filter((x : any) => x.checked == true).filter((x : any) => 
            (x.datasets[0].type == 'radar') && (x.datasets[0].fill != false && x.datasets[0].fill != undefined)
    ).forEach((element: { datasets: any[], labels : string[], type: string }) => {
            if(element.labels.length > newchart.labels.length){
                newchart.labels = element.labels;
            }
            element.datasets.forEach((element2: any) => {
                element2.type = element.type;
                newchart.datasets.push(element2);           
            });
           });
           this.color = [''].concat(this.color);
           this.dataRec.numbers = [newchart].concat(this.dataRec.numbers);
    }
}

CreateChartChategory(){
    let count = this.dataRec.options.filter((x : any) => x.checked == true).length;
    if(count >= 1){
 
        let newchart :{type: string | undefined, name: string, labels: string[], datasets: any[]} = {name: "New Chart", labels: [], datasets: [], type: undefined};

        this.dataRec.options.filter((x : any) => x.checked == true).filter((x : any) => 
            (x.datasets[0].type == 'pie' ||  x.datasets[0].type == undefined || x.datasets[0].type == 'polarArea')
    ).forEach((element: { datasets: any[], labels : string[], type: string }) => {
        
        element.labels.forEach((element2: any) => {
            newchart.labels.push(element2);
        });
            element.datasets.forEach((element2: any) => {
                element2.type = element.type;
                newchart.datasets.push(element2);           
            });
           });


           this.dataRec.options.filter((x : any) => x.checked == true).filter((x : any) => 
            (x.datasets[0].type == 'doughnut')
    ).forEach((element: { datasets: any[], labels : string[], type: string }) => {
        
        element.labels.forEach((element2: any) => {
            newchart.labels.push(element2);
        });
            element.datasets.forEach((element2: any) => {
                element2.type = element.type;
                newchart.datasets.push(element2);           
            });
           });



           this.dataRec.options.filter((x : any) => x.checked == true).filter((x : any) => 
            (x.datasets[0].type == 'bar' ||  x.datasets[0].type == 'radar')
    ).forEach((element: { datasets: any[], labels : string[], type: string }) => {
        
        element.labels.forEach((element2: any) => {
            newchart.labels.push(element2);
        });
            element.datasets.forEach((element2: any) => {
                element2.type = 'pie';
                newchart.datasets.push(element2);           
            });
           });

         newchart.type = 'pie';
        this.dataRec.options = [newchart].concat(this.dataRec.options);
    }
}

CreateChartBool(){
    let count = this.dataRec.booleans.filter((x : any) => x.checked == true).length;
    if(count >= 1){
 
        let newchart :{type: string | undefined, name: string, labels: string[], datasets: any[]} = {name: "New Chart", labels: [], datasets: [], type: undefined};



           this.dataRec.booleans.filter((x : any) => x.checked == true).filter((x : any) => 
            (x.datasets[0].type == 'pie' ||  x.datasets[0].type == undefined || x.datasets[0].type == 'doughnut' || x.datasets[0].type == 'polarArea')
    ).forEach((element: { datasets: any[], labels : string[], type: string }) => {
        
        element.labels.forEach((element2: any) => {
            newchart.labels.push(element2);
        });
            element.datasets.forEach((element2: any) => {
                element2.type = element.type;
                newchart.datasets.push(element2);           
            });
           });

           this.dataRec.booleans.filter((x : any) => x.checked == true).filter((x : any) => 
            (x.datasets[0].type == 'bar' ||  x.datasets[0].type == 'radar')
    ).forEach((element: { datasets: any[], labels : string[], type: string }) => {
        
        element.labels.forEach((element2: any) => {
            newchart.labels.push(element2);
        });
            element.datasets.forEach((element2: any) => {
                element2.type = 'pie';
                newchart.datasets.push(element2);           
            });
           });

         newchart.type = 'pie';
        this.dataRec.booleans = [newchart].concat(this.dataRec.booleans);
    }
}




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
                element.type = this.ChangeChartType(event,element,i);
            });
        }
        chart.type = event.value.value;   
        }      
}

FillChart(event: any,chart: {datasets : any[]}, i: number){
    let datasets : any[] = [];
    chart.datasets.forEach((element: any) => {    
            element.fill = event.value.value;        
        datasets.push(element);
    });
    this.dataRec.numbers[i] = [];
    this.changeDetector.detectChanges();
    this.dataRec.numbers[i] = chart;
    this.changeDetector.detectChanges();

}


FillChartMore(chart: {datasets : any[]}, i: number){
    let datasets : any[] = [];
    chart.datasets.forEach((element: any) => {
        if(element.fill == false || element.fill == undefined || element.fill == 'origin' || element.fill == 'start' || element.fill == 'end'){
            element.fill = '+1';
        }
        else if(element.fill == '+1'){
            element.fill = '-1';
        }
        else if(element.fill == '-1'){
            element.fill = '+1';
        
        }
        else{
            element.fill = '+1';
        }
       
        datasets.push(element);
    });
    this.dataRec.numbers[i] = [];
    this.changeDetector.detectChanges();
    this.dataRec.numbers[i] = chart;
    this.changeDetector.detectChanges();

}

Stacked(chart: {datasets : any[]}, i: number){
    this.optionsnum.scales = {x: this.optionsnum.scales.x, y: this.optionsnum.scales.y};
    this.optionsnum.scales.y.stacked = !this.optionsnum.scales.y.stacked;
    this.optionsnum.scales.x.stacked = !this.optionsnum.scales.x.stacked;
    this.dataRec.numbers[i] = [];
    this.changeDetector.detectChanges();
    this.dataRec.numbers[i] = chart;
    this.changeDetector.detectChanges();
    
}

// DownloadChart(id: string){
//     var pchart = document.getElementById(id);
//       var canvas =pchart?.getElementsByTagName('canvas')[0] as any as HTMLCanvasElement;

//     var link = document.createElement('a');
//     link.href =  canvas.toDataURL();
//     link.download = id+'.png';
//     window.location.href =  canvas.toDataURL();



//     link.click();
   
// }

SteppedChart(event: any,chart: {datasets : any[]}, i: number){
    chart.datasets.forEach(element => {
        element.stepped = event.value.value;
    });
    this.dataRec.numbers[i] = [];
    this.changeDetector.detectChanges();
    this.dataRec.numbers[i] = chart;
    this.changeDetector.detectChanges();
}

ChangeColor(event: any,chart: {datasets : any[]}, i: number){
    this.optionsnum.scales = {x: this.optionsnum.scales.x, y: this.optionsnum.scales.y};

    chart.datasets.forEach(element => {
        element.backgroundColor = event.value;
        element.borderColor = event.value;

    });
    this.dataRec.numbers[i] = [];
    this.changeDetector.detectChanges();
    this.dataRec.numbers[i] = chart;
    this.changeDetector.detectChanges();    

}

Scale(chart: {datasets : any[]}, i: number){
    this.optionsnum.scales = {x: this.optionsnum.scales.x, y: this.optionsnum.scales.y};

    if(this.optionsnum.scales.x.position == 'center'){
        this.optionsnum.scales.x.position = 'bottom';
        this.optionsnum.scales.y.position = 'left';
    }
    else{
        this.optionsnum.scales.x.position = 'center';
        this.optionsnum.scales.y.position = 'center';
    }


    this.dataRec.numbers[i] = [];
    this.changeDetector.detectChanges();
    this.dataRec.numbers[i] = chart;
    this.changeDetector.detectChanges();
}

Decimation(event: any,chart: {datasets : any[], labels: any[],labels2: any[]}, i: number){
    

    chart.datasets.forEach(element => {
        if(element.data2 == undefined){
            element.data2 = element.data;
        }
        element.data = [];
        element.labels = [];
        element.data2.forEach((element2: any, index: number) => {
            if(index % event.value.value == 0){
                element.data.push(element2);
            }
        });
        
    });
    if(chart.labels2 == undefined){
        chart.labels2 = chart.labels;
    }
    chart.labels = [];
    chart.labels2.forEach((element: any, index: number) => {
        if(index % event.value.value == 0){
            chart.labels.push(element);
        }
    });
    this.dataRec.numbers[i] = [];
    this.changeDetector.detectChanges();
    this.dataRec.numbers[i] = chart;
    this.changeDetector.detectChanges();  
} 

Regression(event: any,chart: {datasets : any[], regresult: string | undefined}, i: number){
    if(event.value.value == false){

        chart.datasets = chart.datasets.filter((element: any) => !element.label.includes('Regression'));
        chart.regresult = undefined;
    }	
    else{
        var datasets : any[] = [];
        chart.datasets = chart.datasets.filter((element: any) => !element.label.includes('Regression'));
        chart.datasets.forEach(element => {
            var my_regression;
            if(event.value.value == 'linear'){
                my_regression = regression.linear(
                    element.data.map((element2: any, index: number) => {
                        return [index, element2];
                    })
                  );
            }
            else if(event.value.value == 'polynomial'){
                my_regression = regression.polynomial(
                    element.data.map((element2: any, index: number) => {
                        return [index, element2];
                    })
                  );
            }
            else if(event.value.value == 'exponential'){
                my_regression = regression.exponential(
                    element.data.map((element2: any, index: number) => {
                        return [index, element2];
                    })
                  );
            }
            else if(event.value.value == 'power'){
                my_regression = regression.power(
                    element.data.map((element2: any, index: number) => {
                        return [index, element2];
                    })
                  );
            }
            else if(event.value.value == 'logarithmic'){
                my_regression = regression.logarithmic(
                    element.data.map((element2: any, index: number) => {
                        return [index, element2];
                    })
                  );
            }
              const useful_points = my_regression?.points.map(([x, y]) => {
                return y;
            })
            chart.regresult = my_regression?.string;
            let newdata ={
                data: useful_points,
                label: 'Regression'+element.label,
                borderColor: 'red',
                backgroundColor: 'red',
                fill: element.fill,
                stepped: element.stepped,
                type: 'line'
            };
            datasets.push(newdata);
    
              
        });
        chart.datasets = datasets.concat(chart.datasets);
    }

    this.dataRec.numbers[i] = [];
    this.changeDetector.detectChanges();
    this.dataRec.numbers[i] = chart;
    this.changeDetector.detectChanges();  
}

ChartClick(event: {element:{ datasetIndex: number, index : number}},chart: {name: string, datasets : any[], regresult: string | undefined}, i: number){
    if(chart.datasets[event.element.datasetIndex].label.includes('Regression')){
        return;
    }
    var filter = { field: chart.name, operator: 'equals', value: chart.datasets[event.element.datasetIndex].data[event.element.index].toString() };

    localStorage.setItem('filter', JSON.stringify(filter));
    if(this.name){
        localStorage.setItem('name', this.name);
    }
    if(this.id){
        localStorage.setItem('id', this.id);
    }
    if(this.private){
        localStorage.setItem('private', this.private.toString());
    }

    this.ref = this.dialogService.open(DataobjectListComponent,  { data: {filter: filter}});
    this.ref.onClose.subscribe(x => {
      localStorage.removeItem('filter');
        localStorage.removeItem('name');
        localStorage.removeItem('id');
        localStorage.removeItem('private');

    });
    
}

ChartClickOption(event: {element:{ datasetIndex: number, index : number}},chart: {name: string, datasets : any[], labels: any[]}, i: number){

    var filter = { field: chart.name, operator: 'equals', value: chart.labels[event.element.index].toString() };
    localStorage.setItem('filter', JSON.stringify(filter));
    if(this.name){
        localStorage.setItem('name', this.name);
    }
    if(this.id){
        localStorage.setItem('id', this.id);
    }
    if(this.private){
        localStorage.setItem('private', this.private.toString());
    }

    this.ref = this.dialogService.open(DataobjectListComponent,  { data: {filter: filter}});
    this.ref.onClose.subscribe(x => {
      localStorage.removeItem('filter');
        localStorage.removeItem('name');
        localStorage.removeItem('id');
        localStorage.removeItem('private');

    });
}

AddNewChart(type: string){
    if(type == "numeric"){
        this.ref = this.dialogService.open(ChartCreateComponent,  { data: {fields: this.fields ,charttype: 'numeric',id: this.id, filter: this.config.data.filter, options: this.optionsnum, typeoptions: this.typeoptions}, header: 'Create Chart'});
        this.ref.onClose.subscribe(x => {
            this.dataRec.numbers = [x.chartdata].concat(this.dataRec.numbers);
        });
    }
    else if(type == "text"){
        this.ref = this.dialogService.open(ChartCreateComponent,  { data: {fields: this.fields ,charttype: 'text',id: this.id, filter: this.config.data.filter, options: this.optionsopt, typeoptions: this.typeoptionslogical}, header: 'Create Chart'});
        this.ref.onClose.subscribe(x => {
            this.dataRec.options = [x.chartdata].concat(this.dataRec.options);
        });
    }
    else if(type == "logical"){
        this.ref = this.dialogService.open(ChartCreateComponent,  { data: {fields: this.fields ,charttype: 'logical',id: this.id, filter: this.config.data.filter, options: this.optionsopt, typeoptions: this.typeoptionslogical}, header: 'Create Chart'});
        this.ref.onClose.subscribe(x => {
            this.dataRec.booleans = [x.chartdata].concat(this.dataRec.booleans);
        });
    }
}

}
