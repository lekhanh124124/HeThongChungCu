import { Component, Input, OnChanges, SimpleChanges, OnDestroy } from '@angular/core';

@Component({
  selector: 'app-alert',
  templateUrl: './alert.component.html',
  styleUrls: ['./alert.component.scss']
})
export class AlertComponent {
  @Input() messages: string[] = [];
  @Input() nzType: 'success' | 'info' | 'warning' | 'error' = 'error';
  @Input() duration = 3000;

  messagesInternal: Array<{ id: number; text: string }> = [];
  private nextId = 1;
  private timers = new Map<number, any>();

  ngOnChanges(changes: SimpleChanges) {
    if (changes.messages && Array.isArray(this.messages)) {
      for (const m of this.messages) {
        if (!this.messagesInternal.find(x => x.text === m)) {
          const id = this.nextId++;
          this.messagesInternal.push({ id, text: m });
          if (this.duration > 0) {
            const t = setTimeout(() => this.removeById(id), this.duration);
            this.timers.set(id, t);
          }
        }
      }
    }
  }

  removeById(id: number) {
    this.messagesInternal = this.messagesInternal.filter(x => x.id !== id);
    const t = this.timers.get(id);
    if (t) {
      clearTimeout(t);
      this.timers.delete(id);
    }
  }

  ngOnDestroy() {
    for (const t of this.timers.values()) clearTimeout(t);
    this.timers.clear();
  }
}
