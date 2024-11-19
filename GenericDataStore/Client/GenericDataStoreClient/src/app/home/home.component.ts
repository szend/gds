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
import { ScrollTopModule } from 'primeng/scrolltop';
import { ContactComponent } from './contact/contact.component';
import { DataobjectListComponent } from '../Components/dataobject-list/dataobject-list.component';
import { Meta, Title } from '@angular/platform-browser';




@Component({
  selector: 'app-home',
  standalone: true,
  imports: [ButtonModule, DialogModule, DynamicDialogModule, CommonModule,CardModule,DataViewModule, ListViewComponent,TagModule,AnimateOnScrollModule,FooterComponent,ScrollTopModule,
    ContactComponent,DataobjectListComponent
  ],
  providers: [DialogService],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {

  constructor(private titleService: Title,private meta: Meta,public apiService: ApiService, protected router: Router, protected changeDetector: ChangeDetectorRef, protected dialogService: DialogService,public authService: AuthService) 
  {
  }

  ref: DynamicDialogRef | undefined;
  public types : any[] = [];
  public loggedin : boolean = true;
  @ViewChild('list', {static: false}) list: ListViewComponent | undefined;


  key : string = "generic, data, store, genericdatastore, generic data store, database editor, without sql, edit database, edit database without sql, database, database tool, store data online, online data, manage database, connect database, analyse data, data analysis, database chart, create ai, ai data analysis, online database editor, online database editor without sql, store data online free, edit data online free, connect database online, edit database online, create relation between tables with different database. create relation between database tables with different database online"

  desc : string = "Database editor tool. Manage and analyse your data online without sql. Connnect your databases, edit, create, delete data and tables, see chart, create calculated field and coloring, use built in AI tools."


  repidx = 1;
  withidx = 1;
  ngOnInit() {
    this.titleService.setTitle("GenericDataStore"); 
    this.meta.updateTag({ name: 'description', content: this.desc });
    this.meta.updateTag({ name: 'keywords', content: this.key });
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
      const exp = gsap.timeline()
      exp.to('.experience-middle', {
        '--progress1': 1,
        duration: 5,
        smoothOrigin: true,
        scrollTrigger: {
          trigger: '.experience',
          start: 'top top',
          end: '+=5000',
          scrub: true,
          markers: false,
          pin: '.experience',
        },
      });

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


// gsap.from("#zoom-out h2", {
//   scale: 930, stagger: 0.25, duration: 3,
//   scrollTrigger: {
//       trigger: "#zoom-out",
//       pin: true,
//       end: `+=${innerHeight * 1.3}`,
//       scrub: 3,
//       markers: false
//   }
// });

// zoom-in
gsap.to("#zoom-in h2", {
  scale: 600, stagger: 0.25, duration: 3,
  scrollTrigger: {
      trigger: "#zoom-in",
      pin: true,
      end: `+=${innerHeight * 1.3}`,
      scrub: 3,
      markers: false
  }
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
