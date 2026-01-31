export interface ApiFieldError {
  field: string;
  code: string;
  message: string;
}

export interface ApiErrorResponse {
  traceId: string;
  code: string;
  message: string;
  errors?: ApiFieldError[];
}
