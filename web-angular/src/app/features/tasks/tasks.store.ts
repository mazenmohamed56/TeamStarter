import { Injectable, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { errorMessage } from '../../core/api';
import { Task, TasksApi } from './tasks.api';

/** Signal-based state for the tasks feature. */
@Injectable({ providedIn: 'root' })
export class TasksStore {
  private readonly api = inject(TasksApi);

  readonly items = signal<Task[]>([]);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  load = () =>
    this.run(async () => this.items.set(await firstValueFrom(this.api.getAll())));

  add = (title: string) =>
    this.run(async () => {
      const created = await firstValueFrom(this.api.create({ title }));
      this.items.update((items) => [created, ...items]);
    });

  toggle = (task: Task) =>
    this.run(async () => {
      const updated = await firstValueFrom(this.api.update(task.id, { ...task, isDone: !task.isDone }));
      this.items.update((items) => items.map((t) => (t.id === updated.id ? updated : t)));
    });

  remove = (id: number) =>
    this.run(async () => {
      await firstValueFrom(this.api.remove(id));
      this.items.update((items) => items.filter((t) => t.id !== id));
    });

  /** Runs an API call with shared loading/error handling. */
  private async run(action: () => Promise<void>) {
    this.loading.set(true);
    this.error.set(null);
    try {
      await action();
    } catch (e) {
      this.error.set(errorMessage(e));
    } finally {
      this.loading.set(false);
    }
  }
}
