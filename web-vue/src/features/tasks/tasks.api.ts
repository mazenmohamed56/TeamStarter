import { http } from '@/api/http'

// Mirrors the backend DTOs (Features/Tasks/TaskDtos.cs)
export interface Task {
  id: number
  title: string
  description: string | null
  isDone: boolean
  createdAtUtc: string
}

export interface CreateTaskRequest {
  title: string
  description?: string | null
}

export interface UpdateTaskRequest extends CreateTaskRequest {
  isDone: boolean
}

export const tasksApi = {
  getAll: () => http.get<Task[]>('/tasks').then((r) => r.data),
  create: (body: CreateTaskRequest) => http.post<Task>('/tasks', body).then((r) => r.data),
  update: (id: number, body: UpdateTaskRequest) =>
    http.put<Task>(`/tasks/${id}`, body).then((r) => r.data),
  remove: (id: number) => http.delete(`/tasks/${id}`),
}
