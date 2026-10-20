import { http } from '@/api/http'

// Mirrors the backend DTOs (Features/Categories/CategoryDtos.cs)
export interface Category {
  id: number
  name: string
  description: string | null
  createdAtUtc: string
}

export interface CreateCategoryRequest {
  name: string
  description?: string | null
}

export interface UpdateCategoryRequest {
  name: string
  description?: string | null
}

export const categoriesApi = {
  getAll: () => http.get<Category[]>('/categories').then((r) => r.data),
  getById: (id: number) => http.get<Category>(`/categories/${id}`).then((r) => r.data),
  create: (body: CreateCategoryRequest) =>
    http.post<Category>('/categories', body).then((r) => r.data),
  update: (id: number, body: UpdateCategoryRequest) =>
    http.put<Category>(`/categories/${id}`, body).then((r) => r.data),
  remove: (id: number) => http.delete(`/categories/${id}`),
}
