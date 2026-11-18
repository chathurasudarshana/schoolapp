import { Component, computed, ChangeDetectionStrategy } from '@angular/core';

import { RouterModule } from '@angular/router';
import { SidenavService } from '../../services/sidenav.service';
@Component({
  selector: 'sch-sidenav',
  imports: [RouterModule],
  templateUrl: './sch-sidenav.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './sch-sidenav.scss',
})
export class SCHSideNav {
  isCollapsed = computed(() => this.sidenavService.isCollapsed());
  
  navigationItems = [
    { label: 'Dashboard', route: '/sch/dashboard', icon: 'bi-speedometer2' },
    { label: 'Students', route: '/sch/student', icon: 'bi-people-fill' },
    { label: 'Teachers', route: '/sch/teacher', icon: 'bi-person-workspace' },
    { label: 'Courses', route: '/sch/course', icon: 'bi-book-half' },
  ];

  constructor(private readonly sidenavService: SidenavService) {}


  toggleSidebar() {
    this.sidenavService.toggleSidebar();
  }
}
