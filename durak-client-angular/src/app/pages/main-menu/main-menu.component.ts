import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faCoins } from '@fortawesome/free-solid-svg-icons';
import { faGem } from '@fortawesome/free-solid-svg-icons';
import { faBolt } from '@fortawesome/free-solid-svg-icons';
import { Router, RouterModule } from '@angular/router';
import { TgService } from '../../services/tg.service';
import { RoomService } from '../../services/room.service';
import { PlayerInfoComponent } from "../../components/player-info/player-info.component";
import { RoomListComponent } from "../../components/room-list/room-list.component";

@Component({
    selector: 'app-main-menu',
    standalone: true,
    templateUrl: './main-menu.component.html',
    styleUrl: './main-menu.component.scss',
    imports: [
        CommonModule,
        FontAwesomeModule,
        RouterModule,
        PlayerInfoComponent,
        RoomListComponent
    ]
})
export class MainMenuComponent implements OnInit{
  faCoins = faCoins;
  faGem = faGem;
  faBolt = faBolt;

  // public profile: Profile = new Profile(0, 0, 0, 0);

  constructor(private roomService: RoomService, public tgService: TgService, private router: Router) {}

  ngOnInit(): void {
    // this.tgService.resetMainButtonColor();

    // this.roomService.getCurrentRoom().then(roomId => {
    //   if (roomId) {
    //     this.router.navigate([`/room/${roomId}`]);
    //   }
    // }).catch(err => console.error(err));

    // this.profileService.getProfile().then(profile => {
    //   this.profile = profile;
    // }).catch(err => console.error(err));
  }

  onMainButtonClick = () => this.router.navigate(['/room-creator']);
}
