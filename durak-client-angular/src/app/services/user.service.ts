import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { lastValueFrom } from 'rxjs';
import { TgService } from './tg.service';
import { User } from '../models/user';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private http: HttpClient, private tgService: TgService) { }

  async getUser(): Promise<User> {
    return lastValueFrom<User>(this.http.get<User>(`${environment.baseUrl}/users/me`));
  }

  async sync(): Promise<void> {
    const username = this.tgService.tg.initData?.user?.firstName!;
    if(localStorage.getItem('sync') === username) return;
    await lastValueFrom(this.http.post(`${environment.baseUrl}/users/sync`, {}));
    localStorage.setItem('sync', username);
  }
}
