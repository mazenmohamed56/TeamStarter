import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';

// Mirrors the backend DTOs (Features/Tasks/TaskDtos.cs)
export interface Task {
  id: number;
  title: string;
  description: string | null;
  isDone: boolean;
  createdAtUtc: string;
}

export interface CreateTaskRequest {
  title: string;
  description?: string | null;
}

export interface UpdateTaskRequest extends CreateTaskRequest {
  isDone: boolean;
}

@Injectable({ providedIn: 'root' })
export class TasksApi {
  private readonly http = inject(HttpClient);

  getAll = () => this.http.get<Task[]>('/tasks');
  create = (body: CreateTaskRequest) => this.http.post<Task>('/tasks', body);
  update = (id: number, body: UpdateTaskRequest) => this.http.put<Task>(`/tasks/${id}`, body);
  remove = (id: number) => this.http.delete<void>(`/tasks/${id}`);
}
