import { RoomPlayer } from "./room-player";
import { RoomSettings } from "./room-settings";

export class RoomState {
    constructor(public players: RoomPlayer[], public isGameStarted: boolean, public roomSettings: RoomSettings) { }
}