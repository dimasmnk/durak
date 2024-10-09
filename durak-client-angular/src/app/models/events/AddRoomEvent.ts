export class AddRoomEvent {
    connectionId: string;
    roomId: string;
    bet: number;
    playerCount: number;

    constructor(connectionId: string, roomId: string, bet: number, playerCount: number) {
        this.connectionId = connectionId;
        this.roomId = roomId;
        this.bet = bet;
        this.playerCount = playerCount;
    }
}