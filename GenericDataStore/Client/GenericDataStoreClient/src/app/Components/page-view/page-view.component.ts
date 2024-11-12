import { CommonModule, NgFor } from '@angular/common';
import { AfterViewInit, Compiler, Component, ComponentRef, Directive, inject, Input, ModuleWithComponentFactories, NgModule, OnChanges, OnInit, Type, ViewChild, viewChild, ViewContainerRef ,ɵcompileComponent} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { compileComponentFromMetadata } from '@angular/compiler';
import { ActivatedRoute } from '@angular/router';
import { ApiService } from '../../Services/api.service';
import { RootFilter } from '../../Models/Parameters';

@Component({
  selector: 'app-page-view',
  standalone: true,
  imports: [],
  templateUrl: './page-view.component.html',
  styleUrl: './page-view.component.css'
})
export class PageViewComponent implements OnInit {

  viewRef = inject(ViewContainerRef);
  name : string | null = null;
  id : string | null = null;
  data: any
  page : string | null = null;
  public rootFilter : RootFilter = {parentValueSortingParams: [],parentValueFilters: [], valueFilters: [], valueSortingParams: [], filters: [], logic: 'and', sortingParams: [], skip: 0, take:0, valueSkip: 0, valueTake:0};

  constructor(public apiService: ApiService,protected route: ActivatedRoute) { }
  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {  
      var queryparams : any = {};

     this.route.queryParamMap.subscribe(qparams => {
      qparams.keys.forEach(key => {
        queryparams[key] = qparams.get(key);
      });
     });

    this.name = params.get("name");
    this.id = params.get("id");
    this.page = params.get("page");
    this.rootFilter.filters.push({ field: 'Name', operator: 'equals', value: this.name });

      this.apiService.GetPage(this.rootFilter,this.id ?? "",this.page ?? "").subscribe((data: any) => {
        let formeddata : any[] = [];
        data.data.forEach((element : {name: string, valueString: string}[]) => {
          let dataitem : any = {};
            element.forEach(element2 => {
              dataitem[element2.name] = element2.valueString;
            });

          formeddata.push(dataitem);
        });
        let template = data.page.html.replace(/<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>/gi, '')
        .replace(/<style\b[^<]*(?:(?!<\/style>)<[^<]*)*<\/style>/gi, '')
        .replace(/<link\b[^<]*(?:(?!<\/link>)<[^<]*)*<\/link>/gi, '')
        .replace(/<meta\b[^<]*(?:(?!<\/meta>)<[^<]*)*<\/meta>/gi, '')
        .replace(/<title\b[^<]*(?:(?!<\/title>)<[^<]*)*<\/title>/gi, '')
        .replace(/<head\b[^<]*(?:(?!<\/head>)<[^<]*)*<\/head>/gi, '')
        .replace(/<body\b[^<]*(?:(?!<\/body>)<[^<]*)*<\/body>/gi, '')
        .replace(/<html\b[^<]*(?:(?!<\/html>)<[^<]*)*<\/html>/gi, '')
        .replace(/<base\b[^<]*(?:(?!<\/base>)<[^<]*)*<\/base>/gi, '')
        .replace(/<form\b[^<]*(?:(?!<\/form>)<[^<]*)*<\/form>/gi, '')
        .replace(/<iframe\b[^<]*(?:(?!<\/iframe>)<[^<]*)*<\/iframe>/gi, '')
        .replace(/<object\b[^<]*(?:(?!<\/object>)<[^<]*)*<\/object>/gi, '')
        .replace(/<embed\b[^<]*(?:(?!<\/embed>)<[^<]*)*<\/embed>/gi, '')
        .replace(/<param\b[^<]*(?:(?!<\/param>)<[^<]*)*<\/param>/gi, '')
        .replace(/<source\b[^<]*(?:(?!<\/source>)<[^<]*)*<\/source>/gi, '')
        .replace(/<track\b[^<]*(?:(?!<\/track>)<[^<]*)*<\/track>/gi, '')
        ;
        let css =data.page.css ?? '';
        const component = getComponentFromTemplate(template,css);
        const componentRef = this.viewRef.createComponent(component);
    componentRef.instance.data = formeddata;
    componentRef.instance.params = queryparams;
      });
    
    });
  }

  Refresh(){

  }

}

@Component({
  template: '',
  
}) class MyCustomComponent {
  @Input('data') data: any[] | undefined;
  @Input('params') params: any | undefined;
}

function getComponentFromTemplate(template: string, style: string = ''): Type<any> {
  ɵcompileComponent(MyCustomComponent, {
    template,
    styles: [style],
    standalone: true,
    imports: [NgFor],

  });

  return MyCustomComponent;
}

