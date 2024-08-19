import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { FileUpload, FileUploadModule } from 'primeng/fileupload';
import { ApiService } from '../../Services/api.service';
import { OverlayPanel, OverlayPanelModule } from 'primeng/overlaypanel';
import { ImageModule } from 'primeng/image';

@Component({
  selector: 'app-savefile',
  standalone: true,
  imports: [],
  templateUrl: './savefile.component.html',
  styleUrl: './savefile.component.css'
})
export class SavefileComponent implements OnInit {
  constructor(public apiService: ApiService
  ) 
{
}

@ViewChild('fileUploader', {static: false}) fileUploader: FileUpload | undefined;
@ViewChild('overlayPanel', {static: false}) overlayPanel: OverlayPanel | undefined;
@ViewChild('buttonClicker') buttonClicker: ElementRef<HTMLElement> | undefined;
acceptedFilesForUpload: string = ".xls, .xlsx, .txt, .rtf, .doc, .docx, .pdf, .jpg, .jpeg, .png, .tiff .img";
public src : string | undefined;
public srctype : string | undefined;
@Input() versionid: number | undefined;

public comment : string | undefined;

public files : any[] = [];

ngOnInit(): void {
 this.Refresh()
}

Refresh(){
//  if (this.versionid != undefined){
//    this.apiService.GetFileMetaByParent(this.versionid).subscribe(x => {
//      this.files = x;
//    });
//  }

}

UploadFile(event : any)
{
//   var file : File = event.files[0];
//   const formData = new FormData();
//   formData.append('file', file, file.name);
//   event.files = null;
//  if(this.versionid != undefined){
//    this.apiService.SaveFile(formData, this.versionid, this.comment).subscribe(x => {
//      this.fileUploader?.clear();
//      this.Refresh();
//    });
//  }

}

DeleteFile(rowIndex : number){
//  this.apiService.DeleteFileMetaById(this.files[rowIndex].fileMetaId).subscribe(x =>{
//    this.Refresh();
//  });
}

Download(rowIndex : number) {
//  this.apiService.DownloadFile(this.files[rowIndex].fileMetaId).subscribe(x => {
//   const downloadedFile = x;
//   const a = document.createElement('a');
//   document.body.appendChild(a);
//   a.download = this.files[rowIndex].name;
//   a.href = URL.createObjectURL(downloadedFile);
//   a.target = '_blank';
//   a.click();
//   document.body.removeChild(a);
//  })
}

SeeFile(event : any, rowIndex : number ){
//  this.apiService.DownloadFile(this.files[rowIndex].fileMetaId).subscribe(x => {
//    this.srctype = x.type;
//    this.src = URL.createObjectURL(x);
//    if (this.srctype == 'application/pdf'){
//      window.open(this.src, '_blank');
//    }
//    else {
//      this.overlayPanel?.toggle(event);
//    }

//   })

}

}
