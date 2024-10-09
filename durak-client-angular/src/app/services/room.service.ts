import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { lastValueFrom } from 'rxjs';
import { Room } from '../models/Room';
import { GameState } from '../models/game-state';
import { RoomState } from '../models/room-state';
import { Card } from '../models/card';
import { DefendPositionCard } from '../models/requests/defend-position-card';
import { CreateRoomRequest } from '../models/requests/create-room-request';

@Injectable({
  providedIn: 'root'
})
export class RoomService {

  constructor(private http: HttpClient) { }

  async generateRoomId(bet: number, isPrivate: boolean ): Promise<string> {
    return lastValueFrom<string>(this.http.get(`${environment.baseUrl}/rooms/create`, { params: new HttpParams().set('bet', bet).set('isPrivate', isPrivate), responseType: 'text' }));
  }

  async joinRoomObsolete(roomId: string): Promise<boolean> {
    return lastValueFrom<boolean>(this.http.post<boolean>(`${environment.baseUrl}/rooms/${roomId}`, {}));
  }

  async joinRoom(roomId: string): Promise<void> {
    await lastValueFrom(this.http.post(`${environment.baseUrl}/rooms/${roomId}`, {}));
  }

  async getRandomRoomConnectionId(): Promise<string> {
    return lastValueFrom<string>(this.http.get(`${environment.baseUrl}/rooms/random`, { responseType: 'text' }));
  }

  async leaveRoom() : Promise<boolean> {
    return lastValueFrom<boolean>(this.http.post<boolean>(`${environment.baseUrl}/rooms/leave`, {}));
  }

  async getRooms(): Promise<Room[]> {
    return lastValueFrom<Room[]>(this.http.get<Room[]>(`${environment.baseUrl}/rooms`));
  }

  async getRoomState(): Promise<RoomState> {
    return lastValueFrom<RoomState>(this.http.get<RoomState>(`${environment.baseUrl}/rooms/state`));
  }

  async getGameState(): Promise<GameState> {
    return lastValueFrom<GameState>(this.http.get<GameState>(`${environment.baseUrl}/rooms/game`));
  }

  async setReady(): Promise<boolean> {
    return lastValueFrom<boolean>(this.http.post<boolean>(`${environment.baseUrl}/rooms/ready`, {}));
  }

  async attack(card: Card): Promise<void> {
    await lastValueFrom(this.http.post(`${environment.baseUrl}/rooms/attack`, card));
  }

  async defend(defendPositionCard: DefendPositionCard): Promise<void> {
    await lastValueFrom(this.http.post(`${environment.baseUrl}/rooms/defend`, defendPositionCard));
  }

  async setPass(): Promise<void> {
    await lastValueFrom(this.http.post(`${environment.baseUrl}/rooms/pass`, {}));
  }

  async setWantToTake(): Promise<void> {
    await lastValueFrom(this.http.post(`${environment.baseUrl}/rooms/take`, {}));
  }

  async getCurrentRoom(): Promise<string> {
    return lastValueFrom<string>(this.http.get(`${environment.baseUrl}/rooms/current`, { responseType: 'text' }));
  }

  async createRoom(createRoomRequest: CreateRoomRequest): Promise<string> {
    return lastValueFrom<string>(this.http.post<string>(`${environment.baseUrl}/rooms`, createRoomRequest));
  }
}
