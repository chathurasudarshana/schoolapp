import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { SCHPage } from './schpage';
import { SidenavService } from '../../services/sidenav.service';

describe('SCHPage', () => {
  let component: SCHPage;
  let fixture: ComponentFixture<SCHPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SCHPage],
      providers: [
        provideRouter([]),
        SidenavService
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SCHPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
