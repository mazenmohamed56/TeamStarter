import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';

/** Prefixes relative URLs with /api — services call `http.get('/tasks')`. */
export const apiBaseUrlInterceptor: HttpInterceptorFn = (req, next) =>
  next(req.url.startsWith('http') ? req : req.clone({ url: `/api${req.url}` }));

/** Turns an API error (ProblemDetails / validation errors) into a readable message. */
export function errorMessage(error: unknown): string {
  if (error instanceof HttpErrorResponse && error.error) {
    const data = error.error as {
      title?: string;
      detail?: string;
      errors?: Record<string, string[]>;
    };
    if (data.errors) return Object.values(data.errors).flat().join(' ');
    return data.detail ?? data.title ?? error.message;
  }
  return error instanceof Error ? error.message : 'Unexpected error';
}
