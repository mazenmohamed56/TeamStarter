import { Component, OnInit, inject, signal } from '@angular/core';
import { TasksStore } from './tasks.store';

@Component({
  selector: 'app-tasks-page',
  template: `
    <h1>Tasks</h1>

    <form class="row" (submit)="submit($event)">
      <input
        [value]="newTitle()"
        (input)="newTitle.set($any($event.target).value)"
        placeholder="What needs to be done?"
        maxlength="200"
      />
      <button type="submit" [disabled]="store.loading()">Add</button>
    </form>

    @if (store.error(); as error) {
      <p class="error">{{ error }}</p>
    }
    @if (store.loading() && !store.items().length) {
      <p class="muted">Loading…</p>
    } @else if (!store.items().length) {
      <p class="muted">No tasks yet — add the first one.</p>
    }

    <ul class="list">
      @for (task of store.items(); track task.id) {
        <li [class.done]="task.isDone">
          <label>
            <input type="checkbox" [checked]="task.isDone" (change)="store.toggle(task)" />
            {{ task.title }}
          </label>
          <button class="link" (click)="store.remove(task.id)">Delete</button>
        </li>
      }
    </ul>
  `,
})
export class TasksPage implements OnInit {
  protected readonly store = inject(TasksStore);
  protected readonly newTitle = signal('');

  ngOnInit() {
    this.store.load();
  }

  async submit(event: Event) {
    event.preventDefault();
    const title = this.newTitle().trim();
    if (!title) return;
    await this.store.add(title);
    if (!this.store.error()) this.newTitle.set('');
  }
}
