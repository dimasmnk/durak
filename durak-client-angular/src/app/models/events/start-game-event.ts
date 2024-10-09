import { GameState } from "../game-state";

export class StartGameEvent {
    constructor(public gameState: GameState) {
    }
}