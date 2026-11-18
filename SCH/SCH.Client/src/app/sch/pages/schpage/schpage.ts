import { Component, computed, ChangeDetectionStrategy } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SCHHeader } from '../../selectors/sch-header/sch-header';
import { SCHSideNav } from '../../selectors/sch-sidenav/sch-sidenav';
import { SidenavService } from '../../services/sidenav.service';

@Component({
  selector: 'sch-schpage',
  imports: [RouterOutlet, SCHHeader, SCHSideNav],
  templateUrl: './schpage.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './schpage.scss',
})
export class SCHPage {
  isCollapsed = computed(() => this.sidenavService.isCollapsed());

  constructor(private readonly sidenavService: SidenavService) {}

}
