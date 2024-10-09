export class PlayerState {
    constructor(
        public id: number, 
        public isPassed: boolean, 
        public isWantToTake: boolean, 
        public cardCount: number) { }
}