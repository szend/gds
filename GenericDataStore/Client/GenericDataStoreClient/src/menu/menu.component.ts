import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { MenubarModule } from 'primeng/menubar';
import { ApiService } from '../app/Services/api.service';
import { AuthService } from '../app/Services/auth.service';
import { ActivatedRoute, Router } from '@angular/router';
import { AvatarModule } from 'primeng/avatar';
import { AvatarGroupModule } from 'primeng/avatargroup';
import { SidebarModule } from 'primeng/sidebar';
import { MeterGroupModule, MeterItem } from 'primeng/metergroup';
import { get } from 'http';
import { InputTextModule } from 'primeng/inputtext';
import { KeyFilterModule } from 'primeng/keyfilter';
import { FormsModule } from '@angular/forms';
import { ProgressSpinnerModule } from 'primeng/progressspinner';



@Component({
  selector: 'app-menu',
  standalone: true,
  imports: [MenubarModule, ButtonModule,CommonModule,AvatarModule,AvatarGroupModule,SidebarModule,MeterGroupModule,InputTextModule, KeyFilterModule, FormsModule, ProgressSpinnerModule],
  templateUrl: './menu.component.html',
  styleUrl: './menu.component.css'
})
export class MenuComponent {

  public items : MenuItem[] = [];
  public username : string  | undefined = '';
  public sidebarVisible: boolean = false;
  public listspace : MeterItem[] = [];
  public dataspace: MeterItem[] = [];
  public extdataspace: MeterItem[] = [];
  public maxspace : number | undefined;
  public currentpace : number | undefined;
  public maxlist : number | undefined;
  public currentlist : number | undefined;
  public maxexterndata : number | undefined;
  public currentexdata : number | undefined;

  public intdata : number | undefined;
  public extdata : number | undefined;
  public limitsaved : boolean = false;


  public loggedIn : boolean | undefined = undefined;

  constructor(protected route: ActivatedRoute,public authService: AuthService,private router: Router,public apiService: ApiService) 
  {
  }
  ngOnInit()
  {
    this.loggedIn = this.authService.isLoggedIn();
    this.authService.newlogin.subscribe(x => {
      this.loggedIn = x;
      this.Refresh();
    });
   this.Refresh();
  }

  Refresh(){
    //this.loggedIn = this.authService.isLoggedIn();
    this.username = localStorage?.getItem("username") ?? "";
    this.items = [];

    this.apiService.GetTypeByFilter(undefined,true).subscribe(x =>{
      let privatmenu : {label: string, icon: string, routerLink: string, items: any[]} =  { label: 'My lists', icon: 'pi pi-align-justify', routerLink: '/mylists', items:[]};
      x.forEach((y: any)  => {
        let icon = y.private == true ? 'pi pi-lock' : 'pi pi-lock-open'
        let link = y.private == true ? 'private/' +y.objectTypeId+ '/'+ y.name : '/' +y.objectTypeId + '/' + y.name
        let listitem =  { label: y.name, icon: icon, routerLink: link};
        privatmenu.items.push(listitem)
      });
      this.items = [
        { label: 'Dashboard', icon: 'pi pi-home', routerLink: '/dashboard'},
        { label: 'Price', icon: 'pi pi-euro', routerLink: '/price'},
        { label: 'Contact', icon: 'pi pi-comment', routerLink: '/contact'},
        { label: 'Features', icon: 'pi pi-info-circle', routerLink: '/features'},
        { label: 'MyDatabases',icon: 'pi pi-database', routerLink: '/connection/databases'},
        { label: 'Messages',icon: 'pi pi-comments', routerLink: '/messages'},
      ];
      this.items.push(privatmenu);
      // this.getCategories();
      this.loggedIn = true;
      
    }, () =>{
      this.items = [
        { label: 'Home', icon: 'pi pi-home', routerLink: '/home'},
        { label: 'Price', icon: 'pi pi-euro', routerLink: '/price'},
        { label: 'Contact', icon: 'pi pi-comment', routerLink: '/contact'},
        { label: 'Features', icon: 'pi pi-info-circle', routerLink: '/features'},
        { label: 'Login', icon: 'pi pi-sign-in', routerLink: '/login'},
        { label: 'Register', icon: 'pi pi-user-plus', routerLink: '/register'},
      ];
      // this.getCategories();
      // this.items.push(        { label: 'Login', icon: 'pi pi-sign-in', routerLink: '/login'},
      //   { label: 'Register', icon: 'pi pi-user-plus', routerLink: '/register'});
    })

   
  
  }

  // getCategories(){
  //   this.apiService.GetAllCategory().subscribe(x => {
  //     if(this.items.length == 2 || this.items[2].label != 'Categories'){
  //     let categorymenu : {label: string, icon: string, routerLink: string, items: any[]} =  { label: 'Categories', icon: 'pi pi-tags', routerLink: '/home', items:[]};
  //     x.forEach((y: any)  => {
  //       let listitem =  { label: y.cat, icon: 'pi pi-tag', routerLink: '/list/'+ y.cat};
  //       categorymenu.items.push(listitem)
  //     });
      
  //     this.items.push(categorymenu);
  //     this.items = [...this.items];
  //   }
  //   });
  
  // }

  Logout(){
    this.apiService.Logout().subscribe(x => {
      this.authService.logout();
      this.router.navigateByUrl('login');
      this.Refresh();
    });
  }

  Login(){
    this.router.navigateByUrl('login');
    this.Refresh();
  }
  Register(){
    this.router.navigateByUrl('register');
  }

  Messages(){
    this.router.navigateByUrl('messages');
  }

  Subscriptions(){
    window.open('https://billing.stripe.com/p/login/bIYbKS8HgdiNazKfYY', '_blank');
  }

  Settings(){
    this.sidebarVisible = true;
  }

  limitsloadnig : boolean = false;
  Limits(){
    this.limitsloadnig = true;
    this.apiService.GetSettingData().subscribe(x =>{
      let newlistspace : MeterItem[] = [];
      let newdataspace : MeterItem[] = [];
      let newdataexternspace : MeterItem[] = [];

      newlistspace.push({label: 'List space', value: (x.currentlist / x.maxlist)* 100, color: '#77B6EA'});
      newdataspace.push({label: 'Data space', value: (x.currentdata / x.maxdata) * 100, color: '#97B67A'});
      newdataexternspace.push({label: 'External data space', value: (x.currentexdata / x.maxexterndata) * 100, color: '#27B931'});

      this.dataspace = newdataspace;
      this.listspace = newlistspace;
      this.extdataspace = newdataexternspace
      this.maxspace = x.maxdata;
      this.currentpace = x.currentdata;
      this.maxlist = x.maxlist;
      this.currentlist = x.currentlist;
      this.maxexterndata = x.maxexterndata;
      this.currentexdata = x.currentexdata;
      this.limitsloadnig = false;
    });
  }
  Route(route : string){
    this.router.navigateByUrl('/'+route);
  }

  Search(event : any, element : any){
    var str = element.value;
    element.value = "";
    this.router.navigateByUrl('/search/'+str);
  }
}
