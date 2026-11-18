import { http } from '@/api/http'

// Mirrors the backend DTOs (Features/Services/ServiceDtos.cs)
export interface Service {
  id: number
  name: string
  description: string | null
  durationMinutes: number
  price: number
  isActive: boolean
  imageUrl: string | null
  categoryId: number
  categoryName: string
  createdAtUtc: string
}

export interface CreateServiceRequest {
  name: string
  description?: string | null
  durationMinutes: number
  price: number
  isActive: boolean
  imageUrl?: string | null
  categoryId: number
}

export interface UpdateServiceRequest {
  name: string
  description?: string | null
  durationMinutes: number
  price: number
  isActive: boolean
  imageUrl?: string | null
  categoryId: number
}

export const servicesApi = {
  getAll: () => http.get<Service[]>('/services').then((r) => r.data),
  getById: (id: number) => http.get<Service>(`/services/${id}`).then((r) => r.data),
  create: (body: CreateServiceRequest) => http.post<Service>('/services', body).then((r) => r.data),
  update: (id: number, body: UpdateServiceRequest) =>
    http.put<Service>(`/services/${id}`, body).then((r) => r.data),
  remove: (id: number) => http.delete(`/services/${id}`),
}
