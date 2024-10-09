import { CardAttack } from "./card-attack";

export class Turn {
    constructor(
        public attackerId: number,
        public defenderId: number,
        public tableCards: CardAttack[]
    ) { }
}