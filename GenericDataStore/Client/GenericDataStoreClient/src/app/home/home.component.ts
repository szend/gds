import { ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { ApiService } from '../Services/api.service';
import { Router } from '@angular/router';
import { Dialog, DialogModule  } from 'primeng/dialog';
import { DialogService, DynamicDialogModule, DynamicDialogRef } from 'primeng/dynamicdialog';
import { ObjecttypeCreateComponent } from '../Components/objecttype-create/objecttype-create.component';
import { CommonModule } from '@angular/common';
import { CardModule } from 'primeng/card';
import { AuthService } from '../Services/auth.service';
import { DataViewModule } from 'primeng/dataview';
import { ListViewComponent } from './list-view/list-view.component';
import { TagModule } from 'primeng/tag';
import { AnimateOnScrollModule } from 'primeng/animateonscroll';
import { FooterComponent } from './footer/footer.component';
import { gsap, Power2, Elastic  } from 'gsap';
import { ScrollTrigger } from "gsap/ScrollTrigger";
import { ScrollToPlugin } from "gsap/ScrollToPlugin";





@Component({
  selector: 'app-home',
  standalone: true,
  imports: [ButtonModule, DialogModule, DynamicDialogModule, CommonModule,CardModule,DataViewModule, ListViewComponent,TagModule,AnimateOnScrollModule,FooterComponent],
  providers: [DialogService],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {

  constructor(public apiService: ApiService, protected router: Router, protected changeDetector: ChangeDetectorRef, protected dialogService: DialogService,public authService: AuthService) 
  {
  }

  ref: DynamicDialogRef | undefined;
  public types : any[] = [];
  public loggedin : boolean = true;
  @ViewChild('list', {static: false}) list: ListViewComponent | undefined;


  ngOnInit() {
    gsap.registerPlugin(ScrollTrigger,ScrollToPlugin);
    
    let items = gsap.utils.toArray(".items"),
    pageWrapper = document.querySelector(".page-wrapper");

items.forEach((container : any, i) => {
  let localItems = container.querySelectorAll(".item"),
      distance = () => {
        let lastItemBounds = localItems[localItems.length-1].getBoundingClientRect(),
            containerBounds = container.getBoundingClientRect();
        return Math.max(0, lastItemBounds.right - containerBounds.right);
      };
  gsap.to(container, {
    x: () => -distance(), // make sure it dynamically calculates things so that it adjusts to resizes
    ease: "none",
    scrollTrigger: {
      trigger: container,
      start: "top top",
      pinnedContainer: pageWrapper,
      end: () => "+=" + distance(),
      pin: pageWrapper,
      scrub: true,
      invalidateOnRefresh: true // will recalculate any function-based tween values on resize/refresh (making it responsive)
    }
  })
});
    
    this.Refresh()
  }

  setActive(link : any, links: any) {
    links.forEach((el : any) => el.classList.remove("active"));
    link.classList.add("active");
  }

  Refresh(){
    this.list?.Refresh();
    this.loggedin = this.authService.isLoggedIn()

      
  }

  CreateNew(){
      this.ref = this.dialogService.open(ObjecttypeCreateComponent,  { data: {}, header: 'Create Object Definition', resizable: true});
      this.ref.onClose.subscribe(x => {
        this.Refresh();
      });
  }

  Contact(){
    this.router.navigateByUrl('/contact');
  }

  GoToType(name : string){
    this.router.navigateByUrl('/'+ name);
  }

  DemoList(){
    this.router.navigateByUrl('/demo');
  }

  Route(route : string){
    this.router.navigateByUrl('/'+route);
  }
}
