import { Component, OnInit } from '@angular/core';
import { Title, Meta } from '@angular/platform-browser';

@Component({
  selector: 'app-terms-use',
  standalone: true,
  imports: [],
  templateUrl: './terms-use.component.html',
  styleUrl: './terms-use.component.css'
})
export class TermsUseComponent implements OnInit{


  constructor(private titleService: Title,private meta: Meta ) 
  {
  }
  ngOnInit(): void {
    this.titleService.setTitle("TermsUse"); 
    this.meta.updateTag({ name: 'description', content: "TermsUse" });
    this.meta.updateTag({ name: 'keywords', content: "TermsUse" });
    }

}