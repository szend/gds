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

  constructor(public apiService: ApiService, protected router: Router, protected changeDetector: ChangeDetectorRef, protected dialogService: DialogService,public authService: AuthService) 
  {
  }

  ref: DynamicDialogRef | undefined;
  public types : any[] = [];
  public loggedin : boolean = true;
  @ViewChild('list', {static: false}) list: ListViewComponent | undefined;

  replacetexts : any[] = [
    {text : "Replace your unreadable API response", src: "assets/demoimg/apiresponse.png"},
    {text : "Replace your Excel file", src: "assets/demoimg/Excel.png"},
    {text : "Replace your complex SQL query", src: "assets/demoimg/sql.png"}
  ];

  withtexts : any[] = [
    {text : "Manageable list", src: "assets/demoimg/table.gif"},
    {text : "Live dashboard", src: "assets/demoimg/dashboard.gif"},
    {text : "Auto generated and custom chart", src: "assets/demoimg/charts.gif"}
  ]

  replacetext : string = this.replacetexts[0].text;
  withtext : string = this.withtexts[0].text;
  replacesrc : string = this.replacetexts[0].src;
  withsrc : string = this.withtexts[0].src;

  repidx = 1;
  withidx = 1;
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
