import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TgService } from '../../services/tg.service';
import { faCoins } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { User } from '../../models/user';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-player-info',
  standalone: true,
  imports: [CommonModule, FontAwesomeModule],
  templateUrl: './player-info.component.html',
  styleUrl: './player-info.component.css'
})
export class PlayerInfoComponent implements OnInit {
  user: User | null = null;
  faCoins = faCoins;

  constructor(public userService: UserService, public tgService: TgService) {}

  ngOnInit(): void {
    this.userService.getUser().then(user => {
      this.user = user;
    }).catch(err => console.error(err));
  }
}
