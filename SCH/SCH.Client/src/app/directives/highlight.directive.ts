import { Directive, ElementRef, HostListener, inject, input, output } from '@angular/core';

@Directive({
  selector: '[highlight]',
  standalone: true
})
export class HighlightDirective {
  highlight = input<string>('yellow');
  highlighted = output<boolean>();

  private readonly el = inject(ElementRef);

  @HostListener('mouseenter')
  onEnter(): void {
    this.el.nativeElement.style.background = this.highlight();
    this.highlighted.emit(true);
  }

  @HostListener('mouseleave')
  onLeave(): void {
    this.el.nativeElement.style.background = '';
    this.highlighted.emit(false);
  }
}
