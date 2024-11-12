import { Component } from '@angular/core';
import { AccordionModule } from 'primeng/accordion';
import { ProgramDescriptionComponent } from '../program-description/program-description.component';
import { FooterComponent } from '../footer/footer.component';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-features',
  standalone: true,
  imports: [AccordionModule, ProgramDescriptionComponent,FooterComponent,CommonModule, FormsModule],
  templateUrl: './features.component.html',
  styleUrl: './features.component.css'
})
export class FeaturesComponent {

  title :string = "About the program" 

  Select(str : string){
    this.title = str;
  }

}
