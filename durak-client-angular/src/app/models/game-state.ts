import { Card } from "./card";
import { Suit } from "./enums/suit";
import { PlayerState } from "./player-state";
import { Turn } from "./turn";

export class GameState {
    constructor(
        public playerStates: PlayerState[], 
        public deckCardCount: number,
        public trump: Suit,
        public trumpCard: Card,
        public currentTurn: Turn,
        public cards: Card[]) { }
}