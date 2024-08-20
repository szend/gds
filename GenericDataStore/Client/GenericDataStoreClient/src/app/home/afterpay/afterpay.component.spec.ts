import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AfterpayComponent } from './afterpay.component';

describe('AfterpayComponent', () => {
  let component: AfterpayComponent;
  let fixture: ComponentFixture<AfterpayComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AfterpayComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(AfterpayComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
