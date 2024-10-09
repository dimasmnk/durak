export class PlayerConnectionStatusEvent {
    constructor(public playerId: number, public isConnected: boolean) {
    }
}