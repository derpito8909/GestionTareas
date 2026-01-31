import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';
import { ApiErrorResponse } from '../models/api-error.model';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe(
    catchError((err: unknown) => {
      if (err instanceof HttpErrorResponse && err.status === 0) {
        const connectionErr: ApiErrorResponse = {
          traceId: '',
          code: 'connection_error',
          message: 'No se pudo conectar al API. Verifica que el backend esté encendido y que el proxy esté configurado.'
        };
        return throwError(() => connectionErr);
      }

      if (err instanceof HttpErrorResponse) {
        const apiErr = err.error as ApiErrorResponse;

        if (apiErr?.message) {
          return throwError(() => apiErr);
        }

        const generic: ApiErrorResponse = {
          traceId: '',
          code: 'http_error',
          message: `Error HTTP ${err.status}. Revisa la respuesta del servidor.`
        };
        return throwError(() => generic);
      }

      const fallback: ApiErrorResponse = {
        traceId: '',
        code: 'unexpected_error',
        message: 'Ocurrió un error inesperado. Intenta de nuevo.'
      };
      return throwError(() => fallback);
    })
  );
};
