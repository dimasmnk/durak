import { GameState } from "../game-state";

export class SyncGameStateEvent {
    constructor(public gameState: GameState) {
    }
}