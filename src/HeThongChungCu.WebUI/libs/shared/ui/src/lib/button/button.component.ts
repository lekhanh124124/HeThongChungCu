import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-button',
  template: `<button class="example-btn">{{ label }}</button>`,
  styles: [`.example-btn { padding: 0.5rem 1rem; }`]
})
export class ButtonComponent {
  @Input() label = 'Click';
}
