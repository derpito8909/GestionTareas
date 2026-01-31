import { inject } from '@angular/core';
import { HttpInterceptorFn } from '@angular/common/http';
import { API_BASE_URL } from '../config/app.tokens';

export const apiBaseUrlInterceptor: HttpInterceptorFn = (req, next) => {
  const baseUrl = inject(API_BASE_URL);

  if (/^https?:\/\//i.test(req.url)) return next(req);

  const url = `${baseUrl}${req.url}`;
  return next(req.clone({ url }));
};
