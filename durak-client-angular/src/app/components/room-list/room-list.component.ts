import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Room } from '../../models/Room';
import { RoomService } from '../../services/room.service';
import { Router } from '@angular/router';
import { TgService } from '../../services/tg.service';
import { AuthService } from '../../services/auth.service';
import { environment } from '../../../environments/environment';
import * as signalR from "@microsoft/signalr";
import { AddRoomEvent } from '../../models/events/AddRoomEvent';
import { RemoveRoomEvent } from '../../models/events/RemoveRoomEvent';
import { UpdateRoomPlayerCountEvent } from '../../models/events/UpdateRoomPlayerCountEvent';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faGhost, faCoins, faUserGroup } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-room-list',
  standalone: true,
  imports: [CommonModule, FontAwesomeModule],
  templateUrl: './room-list.component.html',
  styleUrl: './room-list.component.css'
})
export class RoomListComponent implements OnInit{
  faGhost = faGhost;
  faCoins = faCoins;
  faUserGroup = faUserGroup;

  rooms: Room[] = [];

  constructor(public roomService: RoomService, private router: Router, private authService: AuthService) {}

  ngOnInit(): void {
    this.getRooms();
    
    const connection = new signalR.HubConnectionBuilder()
    .withUrl(environment.baseUrl + 'hubs/room-list', { accessTokenFactory: () => this.authService.getToken()!})
    .build();

    connection.start();

    connection.on("AddRoom", data => {
      this.addRoom(data);
    });

    connection.on("RemoveRoom", data => {
      this.removeRoom(data);
    });

    connection.on("UpdateRoomPlayerCount", data => {
      this.updateRoom(data);
    });
  }

  addRoom(addRoomEvent: AddRoomEvent) {
    const room = new Room(addRoomEvent.connectionId, addRoomEvent.roomId, addRoomEvent.bet, addRoomEvent.playerCount);
    this.rooms.push(room);
  }

  removeRoom(removeRoomEvent: RemoveRoomEvent) {
    this.rooms = this.rooms.filter(room => room.connectionId !== removeRoomEvent.connectionId);
  }

  updateRoom(updateRoomPlayerCountEvent: UpdateRoomPlayerCountEvent) {
    console.log(updateRoomPlayerCountEvent);
    const room = this.rooms.find(room => room.connectionId === updateRoomPlayerCountEvent.connectionId);
    if (room) {
      room.playerCount = updateRoomPlayerCountEvent.playerCount;
    }
  }

  joinRoom(connectionId: string) {
    this.router.navigate([`/room/${connectionId}`]);
  }

  async getRooms() {
    this.rooms = await this.roomService.getRooms();
  }
}
