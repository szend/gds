import { Component } from '@angular/core';
import { AccordionModule } from 'primeng/accordion';
import { ProgramDescriptionComponent } from '../program-description/program-description.component';
import { FooterComponent } from '../footer/footer.component';

@Component({
  selector: 'app-features',
  standalone: true,
  imports: [AccordionModule, ProgramDescriptionComponent,FooterComponent],
  templateUrl: './features.component.html',
  styleUrl: './features.component.css'
})
export class FeaturesComponent {

}
