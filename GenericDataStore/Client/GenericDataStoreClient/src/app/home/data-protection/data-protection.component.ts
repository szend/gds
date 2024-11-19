import { Component, OnInit } from '@angular/core';
import { Title, Meta } from '@angular/platform-browser';

@Component({
  selector: 'app-data-protection',
  standalone: true,
  imports: [],
  templateUrl: './data-protection.component.html',
  styleUrl: './data-protection.component.css'
})
export class DataProtectionComponent implements OnInit{


  constructor(private titleService: Title,private meta: Meta ) 
  {
  }
  ngOnInit(): void {
    this.titleService.setTitle("DataProtection"); 
    this.meta.updateTag({ name: 'description', content: "DataProtection" });
    this.meta.updateTag({ name: 'keywords', content: "DataProtection" });
    }

}
