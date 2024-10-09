import { Bet } from "./enums/bet";

export class RoomSettings {
    constructor(public id: string, public bet: Bet, public isPrivate: boolean) {
    }
}