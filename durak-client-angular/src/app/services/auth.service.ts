import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { lastValueFrom } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import * as jwt_decode from 'jwt-decode';
import { TelegramToken } from '../models/requests/TelegramToken';
import { TgService } from './tg.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  token: string = '';

  constructor(private httpClient: HttpClient, private tgService: TgService) { }

  async getToken(): Promise<string> {
    if(this.token == '' || this.isJwtExpiringSoon(this.token)) {
      const initDataRaw = this.tgService.tg?.initDataRaw!;
      this.token = await lastValueFrom<string>(this.httpClient.post(`${environment.baseUrl}/auth`, new TelegramToken(initDataRaw), {responseType: 'text'}));
    }

    return this.token;
  }

  isJwtExpiringSoon(jwt: string): boolean {
  const decodedToken: any = jwt_decode.jwtDecode(jwt);
  const currentTime: number = Math.floor(Date.now() / 1000);
  const expirationTime: number = decodedToken.exp;
  const remainingTime: number = expirationTime - currentTime;
  return remainingTime <= 300;
}

}
