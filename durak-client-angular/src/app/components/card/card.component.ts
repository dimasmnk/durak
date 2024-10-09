import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Suit } from '../../models/enums/suit';
import { Rank } from '../../models/enums/rank';

@Component({
  selector: 'app-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './card.component.html',
  styleUrl: './card.component.scss'
})
export class CardComponent implements OnInit{
  @Input() suit!: Suit;
  @Input() rank!: Rank;
  imagePath!: string;

  ngOnInit(): void {
    this.imagePath = `./assets/cards/${Rank[this.rank]}-${Suit[this.suit]}.png`
  }
}
