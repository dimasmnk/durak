import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faCoins, faKaaba } from '@fortawesome/free-solid-svg-icons';
import { Router } from '@angular/router';
import { RouterModule } from '@angular/router';
import { HttpClientModule, provideHttpClient, withInterceptors } from '@angular/common/http';
import { TgService } from '../../services/tg.service';
import { RoomService } from '../../services/room.service';
import { Bet } from '../../models/enums/bet';
import { FormsModule } from '@angular/forms';
import { environment } from '../../../environments/environment';
import { CreateRoomRequest } from '../../models/requests/create-room-request';


@Component({
  selector: 'app-room-creator',
  standalone: true,
  imports: [
    CommonModule,
    FontAwesomeModule,
    RouterModule,
    HttpClientModule,
    FormsModule
  ],
  templateUrl: './room-creator.component.html',
  styleUrl: './room-creator.component.scss'
})
export class RoomCreatorComponent implements OnInit, OnDestroy{
  onBackButtonClick = () => this.router.navigate(['/']);
  onMainButtonClick = () => this.createRoomAndJoin();

  betOptions = Object.values(Bet).filter(value => typeof value === 'number' && value != 0);
  accessOptions = [{value: false, text: 'Private'}, {value: true, text: 'Public'}];
  selectedBet: number = 10;
  selectedAccess: boolean = false;
  faCoins = faCoins;
  faKaaba = faKaaba;
  
  constructor(private router: Router, private roomService: RoomService, private tgService: TgService) {}

  ngOnInit(): void {    
    this.tgService.tg.backButton.show();
    this.tgService.tg.backButton.on('click', this.onBackButtonClick);
  }

  public async createRoomAndJoin() {
    const createRoomRequest = new CreateRoomRequest(this.selectedBet, this.selectedAccess, 2);
    const roomId = await this.roomService.createRoom(createRoomRequest);
    console.log(roomId);
    await this.roomService.joinRoom(roomId);
    // this.router.navigate([`/room/${roomId}`]);
  }

  ngOnDestroy(): void {
    this.tgService.tg.backButton.off('click', this.onBackButtonClick);
  }
}