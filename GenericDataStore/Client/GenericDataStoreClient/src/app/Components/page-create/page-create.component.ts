
import { CommonModule, NgFor } from '@angular/common';
import { AfterViewInit, Compiler, Component, ComponentRef, Directive, forwardRef, inject, Input, ModuleWithComponentFactories, NgModule, OnChanges, OnInit, Type, ViewChild, viewChild, ViewContainerRef ,ɵcompileComponent} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { compileComponentFromMetadata } from '@angular/compiler';
import { ActivatedRoute } from '@angular/router';
import { ApiService } from '../../Services/api.service';
import { RootFilter } from '../../Models/Parameters';
import { DynamicDialogConfig } from 'primeng/dynamicdialog';
import { ButtonModule } from 'primeng/button';
import { CalendarModule } from 'primeng/calendar';
import { CheckboxModule } from 'primeng/checkbox';
import { ChipModule } from 'primeng/chip';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DropdownModule } from 'primeng/dropdown';
import { FileUploadModule } from 'primeng/fileupload';
import { ImageModule } from 'primeng/image';
import { InputGroupModule } from 'primeng/inputgroup';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextModule } from 'primeng/inputtext';
import { KeyFilterModule } from 'primeng/keyfilter';
import { MessagesModule } from 'primeng/messages';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { TabViewModule } from 'primeng/tabview';
import { ToastModule } from 'primeng/toast';
import { TriStateCheckboxModule } from 'primeng/tristatecheckbox';
import { DataobjectListComponent } from '../dataobject-list/dataobject-list.component';
import { MessageService } from 'primeng/api';


@Component({
  selector: 'app-page-create',
  standalone: true,
  imports: [InputGroupModule, FormsModule,CommonModule,TriStateCheckboxModule,CalendarModule,ConfirmDialogModule,DropdownModule,TabViewModule,ImageModule,ChipModule,
    InputTextModule,InputNumberModule,ButtonModule,MessagesModule,CheckboxModule,forwardRef(() => DataobjectListComponent),FileUploadModule,KeyFilterModule,
    ProgressSpinnerModule,ToastModule,MessagesModule,ToastModule
  ],
  providers: [MessageService],
  templateUrl: './page-create.component.html',
  styleUrl: './page-create.component.css'
})
export class PageCreateComponent implements OnInit {

  name : string | undefined;
  id : string | undefined;
  html: string | undefined;
  css: string | undefined;
  params : string[] = [];	

  constructor(public apiService: ApiService,protected route: ActivatedRoute,  protected config: DynamicDialogConfig,private messageService: MessageService) { }
  ngOnInit(): void {
    if(this.config.data.id){
      this.id = this.config.data.id;
    }
  }

  Save(){
    let page = {
      html : this.html,
      css : this.css,
      name : this.name,
      objectTypeId : this.id
    }
    this.apiService.CreatePage(page).subscribe((data: any) => {
      this.messageService.add({severity:'success', summary: 'Success', detail: 'Page Created'});
    },(error: any) => {
      this.messageService.add({severity:'error', summary: 'Error', detail: 'Error Creating Page'});
    });
  }

  AddParam(){
    this.params.push('');
    console.log(this.params);

  }
  RemoveParam(){
    this.params.pop();

  }

  onReject() {
    this.messageService.clear();
  }
   
}



