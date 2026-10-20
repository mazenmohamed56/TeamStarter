import axios, { AxiosError } from 'axios'

/** Shared axios instance — every API module uses this. */
export const http = axios.create({ baseURL: '/api' })

// TODO (Week 3): request interceptor that attaches `Authorization: Bearer <token>`
// TODO (Week 3): response interceptor that refreshes the token on 401

/** Turns an API error (ProblemDetails / validation errors) into a readable message. */
export function errorMessage(error: unknown): string {
  if (error instanceof AxiosError && error.response?.data) {
    const data = error.response.data as {
      title?: string
      detail?: string
      errors?: Record<string, string[]>
    }
    if (data.errors) return Object.values(data.errors).flat().join(' ')
    return data.detail ?? data.title ?? error.message
  }
  return error instanceof Error ? error.message : 'Unexpected error'
}
