import { Component, OnInit } from '@angular/core';
import { AccordionModule } from 'primeng/accordion';
import { ProgramDescriptionComponent } from '../program-description/program-description.component';
import { FooterComponent } from '../footer/footer.component';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Title, Meta } from '@angular/platform-browser';

@Component({
  selector: 'app-features',
  standalone: true,
  imports: [AccordionModule, ProgramDescriptionComponent,FooterComponent,CommonModule, FormsModule],
  templateUrl: './features.component.html',
  styleUrl: './features.component.css'
})
export class FeaturesComponent  implements OnInit{

  constructor(private titleService: Title,private meta: Meta){

  }

  ngOnInit(): void {

    this.titleService.setTitle("Features"); 
    this.meta.updateTag({ name: 'description', content: "Features" });
    this.meta.updateTag({ name: 'keywords', content: "Features" });  }

  title :string = "About the program" 

  Select(str : string){
    this.title = str;
  }

}
