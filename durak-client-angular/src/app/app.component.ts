import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterOutlet } from '@angular/router';
import { routes } from './app.routes';
import { TgService } from './services/tg.service';
import { UserService } from './services/user.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})

export class AppComponent {
  isLoaded = false;

  constructor(private router: Router, private tgService: TgService, private userService: UserService) {
    this.tgService.init().then(() => {
      const path = window.location.pathname;
      this.router.resetConfig(routes);
      this.userService.sync();
      this.isLoaded = true;
      if(tgService.tg.initData?.startParam) {
        this.router.navigate([`/room/${tgService.tg.initData?.startParam}`]);
      }
      else {
        this.router.navigateByUrl(path);
      }
    });
  }
}
