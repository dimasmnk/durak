import { Rank } from "./enums/rank";
import { Suit } from "./enums/suit";

export class Card{
    constructor(public suit: Suit, public rank: Rank) {}
}