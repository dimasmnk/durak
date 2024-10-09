import { Routes } from '@angular/router';
import { MainMenuComponent } from './pages/main-menu/main-menu.component';
import { RoomCreatorComponent } from './pages/room-creator/room-creator.component';
import { RoomComponent } from './pages/room/room.component';

export const routes: Routes = [
    { path: '', component: MainMenuComponent },
    { path: 'room-creator', component: RoomCreatorComponent },
    { path: 'room/:id', component: RoomComponent }
];
