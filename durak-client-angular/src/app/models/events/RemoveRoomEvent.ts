export class RemoveRoomEvent {
    connectionId: string;

    constructor(connectionId: string) {
        this.connectionId = connectionId;
    }
}