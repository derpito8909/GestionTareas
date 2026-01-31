import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CreateUserRequest, UserResponse } from '../domain/user.model';

@Injectable({ providedIn: 'root' })
export class UsersApiService {
  private readonly http = inject(HttpClient);

  create(req: CreateUserRequest) {
    return this.http.post<UserResponse>('/api/users', req);
  }

  list() {
    return this.http.get<UserResponse[]>('/api/users');
  }
}
