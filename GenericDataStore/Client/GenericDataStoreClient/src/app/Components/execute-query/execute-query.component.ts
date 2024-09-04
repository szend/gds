import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ConfirmationService, MessageService } from 'primeng/api';
import { DynamicDialogConfig, DialogService } from 'primeng/dynamicdialog';
import { ApiService } from '../../Services/api.service';
import { error } from 'console';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-execute-query',
  standalone: true,
  imports: [CommonModule,FormsModule,ProgressSpinnerModule,InputTextareaModule,ButtonModule],
  templateUrl: './execute-query.component.html',
  styleUrl: './execute-query.component.css'
})
export class ExecuteQueryComponent  implements OnInit {

  constructor( public apiService: ApiService, protected changeDetector: ChangeDetectorRef,  protected config: DynamicDialogConfig,private messageService: MessageService
    , protected dialogService: DialogService
  ) 
  {
  }
  public loading: boolean = false;
  public query: string = "";
  public result: string = "";
  public id: string = "";


  ngOnInit(): void {
    if(this.config.data.id){
      this.id = this.config.data.id;
    } 
  }


  Execute(){
    this.loading = true
    this.apiService.ExecuteQuery(this.id,this.query).subscribe(x =>{
      this.result = x;
      this.loading = false;
    }, error => {
      this.messageService.add({severity:'error', summary: 'Error', detail: 'Error'});
      this.loading = false;
    })
  }


}
