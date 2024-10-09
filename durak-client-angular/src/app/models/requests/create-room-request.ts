export class CreateRoomRequest {
    bet: number;
    isPublic: boolean;
    maxPlayerCount: number;
    
    constructor(bet: number, isPublic: boolean, maxPlayerCount: number) {
        this.bet = bet;
        this.isPublic = isPublic;
        this.maxPlayerCount = maxPlayerCount;
    }
}