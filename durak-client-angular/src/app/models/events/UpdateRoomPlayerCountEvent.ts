export class UpdateRoomPlayerCountEvent {
    connectionId: string;
    playerCount: number;

    constructor(connectionId: string, playerCount: number) {
        this.connectionId = connectionId;
        this.playerCount = playerCount;
    }
}