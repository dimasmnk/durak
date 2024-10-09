export class Room {
    connectionId: string;
    roomId: string;
    playerCount: number;
    bet: number;

    constructor(connectionId: string, roomId: string, bet: number, playerCount: number) {
        this.connectionId = connectionId;
        this.roomId = roomId;
        this.playerCount = playerCount;
        this.bet = bet;
    }
}