import { Component, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { RoomService } from '../../services/room.service';
import { TgService } from '../../services/tg.service';
import * as signalR from "@microsoft/signalr";
import { environment } from '../../../environments/environment';
import { AuthService } from '../../services/auth.service';
import { RoomPlayer } from '../../models/room-player';
import { faCoins, faCrown, faRug, faLayerGroup, faHandPointUp, faCheck } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { Card } from '../../models/card';
import { CardComponent } from '../../components/card/card.component';
import { Rank } from '../../models/enums/rank';
import { Suit } from '../../models/enums/suit';
import { GameState } from '../../models/game-state';
import { Turn } from '../../models/turn';
import { PlayerState } from '../../models/player-state';
import { RoomSettings } from '../../models/room-settings';
import { Bet } from '../../models/enums/bet';
import interact from 'interactjs';
import { DefendPositionCard } from '../../models/requests/defend-position-card';

@Component({ 
  selector: 'app-room',
  standalone: true,
  imports: [
    CommonModule, 
    FontAwesomeModule, 
    CardComponent],
  templateUrl: './room.component.html',
  styleUrl: './room.component.scss'
})
export class RoomComponent {
  onMainButtonReadyClick = async () => { await this.setReady(); this.setMainButton()};
  onMainButtonActionClick = () => this.onActionClick();

  mainButtonColor: string = '';
  disabledMainButtonColor: string = 'd1d5db';

  faCoins = faCoins;
  faRug = faRug;
  faCrown = faCrown;
  faLayerGroup = faLayerGroup;
  faHandPointUp = faHandPointUp;
  faCheck = faCheck;

  roomConnectionId: string = '';
  isConnected: boolean | null = null;
  isGameStarted: boolean = false;
  roomSettings: RoomSettings = new RoomSettings("", Bet.Bet10, false);
  trumpSuit: Suit = Suit.clubs;
  deckCardCount: number = 0;

  trumpSuitIcon: string = '';

  playerId: number = this.tgService.tg.initData?.user?.id!;
  roomPlayers: RoomPlayer[] = [];
  get roomOpponent(): RoomPlayer { return this.roomPlayers.find(x => x.id != this.playerId)!}
  get roomMe(): RoomPlayer { return this.roomPlayers.find(x => x.id == this.playerId)!};

  gamePlayers: PlayerState[] = [];
  get gameOpponent(): PlayerState { return this.gamePlayers.find(x => x.id != this.playerId)!}
  get gameMe(): PlayerState { return this.gamePlayers.find(x => x.id == this.playerId)!};

  currentTurn: Turn = null!;
  get isMyTurn(): boolean { return this.currentTurn?.attackerId == this.playerId; }

  cards: Card[] = [];

  get cardCount() {
    return this.cards.length;
  }

  constructor(
    private route: ActivatedRoute, 
    private roomService: RoomService, 
    private router: Router, 
    private tgService: TgService,
    private authService: AuthService,
    private el: ElementRef) {
      this.mainButtonColor = this.tgService.tg.themeParams.buttonColor!;
    }

  ngOnInit(): void {
    this.tgService.tg.backButton.on('click', () => { this.leaveRoom() })
    this.tgService.tg.backButton.show();
    this.roomConnectionId = this.route.snapshot.params['id'];

    this.roomService.getCurrentRoom().then(roomId => {
      if(roomId) {
        if(roomId != this.roomConnectionId) {
          this.router.navigate([`/room/${roomId}`]);
        }
        else {
            const connection = new signalR.HubConnectionBuilder()
            .withUrl(environment.baseUrl + '/room', { accessTokenFactory: () => this.authService.getToken()!})
            .build();
        
            this.roomService.getRoomState().then((roomState) => {
              this.roomPlayers = roomState.players;
              this.roomSettings = roomState.roomSettings;
    
              if(roomState.isGameStarted) {
                this.roomService.getGameState().then((gameState) => {
                  this.isGameStarted = true;
                  this.applyGameState(gameState);
                });
              }
              else {
                this.setMainButton();
              }
            });
    
            connection.on("JoinPlayer", data => {
              this.roomPlayers.push(data.roomPlayer);
            });
    
            connection.on("LeavePlayer", data => {
              this.roomPlayers = this.roomPlayers.filter(x => x.id != data.playerId);
            });
    
            connection.on("SetReady", data => {
              const player = this.roomPlayers.find(x => x.id == data.playerId);
              if(player) {
                player.isReady = true;
              }
            });
    
            connection.on("StartGame", data => {
              this.isGameStarted = true;
              this.applyGameState(data.gameState);
            });
    
            connection.on("SyncGameState", data => {
              this.applyGameState(data.gameState);
            });
    
            connection.on("GameEnd", data => {
              this.isGameStarted = false;
              this.roomService.getRoomState().then((roomState) => {
                this.roomPlayers = roomState.players;
                this.roomSettings = roomState.roomSettings;
                this.setMainButton();
      
                if(roomState.isGameStarted) {
                  this.roomService.getGameState().then((gameState) => {
                    this.isGameStarted = true;
                    this.applyGameState(gameState);
                  });
                }
              });
            });
    
            connection.on("PlayerConnectionStatus", data => {
              if(data.playerId == this.roomOpponent.id) {
                this.roomOpponent.isConnected = data.isConnected;
              }
            });
    
            connection.start();

        }
      }
      else {
        this.roomService.joinRoomObsolete(this.roomConnectionId).then((joinResult) => {
          this.isConnected = joinResult;
          if(this.isConnected) {
            const connection = new signalR.HubConnectionBuilder()
            .withUrl(environment.baseUrl + '/room', { accessTokenFactory: () => this.authService.getToken()!})
            .build();
        
            this.roomService.getRoomState().then((roomState) => {
              this.roomPlayers = roomState.players;
              this.roomSettings = roomState.roomSettings;
    
              if(roomState.isGameStarted) {
                this.roomService.getGameState().then((gameState) => {
                  this.isGameStarted = true;
                  this.applyGameState(gameState);
                });
              }
              else {
                this.setMainButton();
              }
            });
    
            connection.on("JoinPlayer", data => {
              this.roomPlayers.push(data.roomPlayer);
            });
    
            connection.on("LeavePlayer", data => {
              this.roomPlayers = this.roomPlayers.filter(x => x.id != data.playerId);
            });
    
            connection.on("SetReady", data => {
              const player = this.roomPlayers.find(x => x.id == data.playerId);
              if(player) {
                player.isReady = true;
              }
            });
    
            connection.on("StartGame", data => {
              this.isGameStarted = true;
              this.applyGameState(data.gameState);
            });
    
            connection.on("SyncGameState", data => {
              this.applyGameState(data.gameState);
            });
    
            connection.on("GameEnd", data => {
              this.isGameStarted = false;
              this.roomService.getRoomState().then((roomState) => {
                this.roomPlayers = roomState.players;
                this.roomSettings = roomState.roomSettings;
      
                if(roomState.isGameStarted) {
                  this.roomService.getGameState().then((gameState) => {
                    this.isGameStarted = true;
                    this.applyGameState(gameState);
                  });
                }
              });
            });
    
            connection.on("PlayerConnectionStatus", data => {
              if(data.playerId == this.roomOpponent.id) {
                this.roomOpponent.isConnected = data.isConnected;
              }
            });
    
            connection.start();
          }
        });
      }
  
    });
  }

  ngAfterViewInit() {
    interact('.draggable').draggable({
      listeners: {
        move: this.dragMoveListener,
        start (event) {
          event.target.classList.add('zoom-in');
        },
        end (event) {
          if(!event.relatedTarget){
            event.target.style.transform = 'translate(0px, 0px)'
            event.target.setAttribute('data-x', 0)
            event.target.setAttribute('data-y', 0)
          }

          event.target.classList.remove('zoom-in');
        }
      },
      autoScroll: false,
    });

    interact('.attack-dropzone').dropzone({
      // Require a 75% element overlap for a drop to be possible
      overlap: 0.75,
    
      // listen for drop related events:
      ondragenter: function (event) {
        const dropzoneElement = event.target
        dropzoneElement.classList.add('dropzone-active')
      },
      ondragleave: function (event) {
        // remove the drop feedback style
        event.target.classList.remove('dropzone-active')
      },
      ondrop: this.onDropAttack,
    })

    interact('.defend-dropzone').dropzone({
      overlap: 0.2,
    
      ondragenter: function (event) {
        const dropzoneElement = event.target
        dropzoneElement.children[0].classList.add('card-wrapper')
        dropzoneElement.classList.add('defend-dropzone-active')
      },
      ondragleave: function (event) {
        const dropzoneElement = event.target
        dropzoneElement.children[0].classList.remove('card-wrapper')
        event.target.classList.remove('defend-dropzone-active')
      },
      ondrop: this.onDropDefend,
    })
  }

  leaveRoom() {
    this.roomService.leaveRoom().then(
      (result) => {
        this.router.navigate(['/']);
      }
    );
  }

  async setReady(): Promise<void> {
    await this.roomService.setReady();
  }

  applyGameState(gameState: GameState) {
    this.trumpSuit = gameState.trump;
    gameState.cards.sort((a, b) => this.cardCompare(a, b));
    this.cards = gameState.cards;
    this.currentTurn = gameState.currentTurn;
    this.gamePlayers = gameState.playerStates;
    this.deckCardCount = gameState.deckCardCount;

    this.setTrumpIcon();
    this.setMainButton();
  }

  getCardId(card: Card): number {
    return card.suit * 100 + card.rank;
  };

  getTrumpIcon(): string {
    switch(this.trumpSuit) {
      case Suit.hearts:
        return 'bi-suit-heart-fill'
      case Suit.diamonds:
        return 'bi-suit-diamond-fill'
      case Suit.clubs:
        return 'bi-suit-club-fill'
      case Suit.spades:
        return 'bi-suit-spade-fill'
    }
  }
  
  getTrumpColor(): string {
    switch(this.trumpSuit) {
      case Suit.hearts:
      case Suit.diamonds:
        return 'text-red-600';
      case Suit.clubs:
      case Suit.spades:
        return 'text-black';
    }
  }

  setTrumpIcon() {
    this.trumpSuitIcon = this.getTrumpIcon() + ' ' + this.getTrumpColor();
  }

  async attack(card: Card): Promise<boolean> {
    if(this.isMyTurn && (this.currentTurn.tableCards.length === 0 || this.isTableCardContainsCard(card))){
      try{
        await this.roomService.attack(card);
        return true;
      }
      catch(e) {
        return false;
      }
    }

    return false;
  }

  async defend(attackCardId: number, defendCard: Card): Promise<boolean> {
    if(!this.isMyTurn && this.currentTurn.tableCards.length > 0 && this.isCardRankHigher(defendCard, this.currentTurn.tableCards[attackCardId].attackCard)) {
      try{
        await this.roomService.defend(new DefendPositionCard(attackCardId, defendCard));
        return true;
      }
      catch(e) {
        return false;
      }
    }

    return false;
  }
  
  isTableCardContainsCard(card: Card): boolean {
    const isAttackCardContains = this.currentTurn.tableCards.find(x => x.attackCard != null && x.attackCard.rank == card.rank) != null;
    const isDefenceCardContains = this.currentTurn.tableCards.find(x => x.defendCard != null && x.defendCard.rank == card.rank) != null;
    return isAttackCardContains || isDefenceCardContains;
  }

  isCardRankHigher(card: Card, otherCard: Card): boolean {
    if(card.suit == this.trumpSuit && otherCard.suit != this.trumpSuit) {
      return true;
    }

    if(card.suit != this.trumpSuit && otherCard.suit == this.trumpSuit) {
      return false;
    }

    if(card.suit == this.trumpSuit && otherCard.suit == this.trumpSuit) {
      return card.rank > otherCard.rank;
    }

    if(card.suit != this.trumpSuit && otherCard.suit != this.trumpSuit) {
      if(card.suit == otherCard.suit) {
        return card.rank > otherCard.rank;
      }
    }

    return false;
  }

  cardCompare(a: Card, b: Card): number {
    if(a.suit == this.trumpSuit && b.suit != this.trumpSuit) {
      return 1;
    }

    if(a.suit != this.trumpSuit && b.suit == this.trumpSuit) {
      return -1;
    }

    return a.rank > b.rank ? 1 : -1;
  }

  onActionClick() {
    if(this.isMyTurn) {
      this.roomService.setPass();
    }
    else {
      this.roomService.setWantToTake();
    }
  }

  isAllCardsBeaten(): boolean {
    return this.currentTurn.tableCards.find(x => x.defendCard == null) == null;
  }

  onShareLinkClick() {
    this.tgService.tg.utils.openTelegramLink(`https://t.me/share/url?url=https://t.me/${environment.botUsername}/game?startapp=${this.roomConnectionId}`);
  }

  setMainButton() {
    this.tgService.tg.mainButton.off('click', this.onMainButtonReadyClick);
    this.tgService.tg.mainButton.off('click', this.onMainButtonActionClick);

    if(!this.tgService.tg.mainButton.isEnabled) {
      this.tgService.tg.mainButton.enable();
      this.tgService.tg.mainButton.setBackgroundColor(`#${this.mainButtonColor}`);
    }

    if(this.isGameStarted) {
      if(this.isMyTurn && this.currentTurn.tableCards.length > 0 && !this.gameMe.isPassed) {
        this.tgService.tg.mainButton.setText('Pass');
        this.tgService.tg.mainButton.on('click', this.onMainButtonActionClick);
      }

      if(this.isMyTurn && (this.currentTurn.tableCards.length < 0 || this.gameMe.isPassed)) {
        this.tgService.tg.mainButton.disable();
        this.tgService.tg.mainButton.setBackgroundColor(`#${this.disabledMainButtonColor}`);
      }

      if(!this.isMyTurn && this.currentTurn.tableCards.length > 0 && !this.isAllCardsBeaten() && !this.gameMe.isWantToTake) {
        this.tgService.tg.mainButton.setText('Take');
        this.tgService.tg.mainButton.on('click', this.onMainButtonActionClick);
      }

      if(!this.isMyTurn && (this.isAllCardsBeaten() || this.gameMe.isWantToTake)) {
        this.tgService.tg.mainButton.disable();
        this.tgService.tg.mainButton.setBackgroundColor(`#${this.disabledMainButtonColor}`);
      }

      if(this.currentTurn.tableCards.length === 0) {
        this.tgService.tg.mainButton.hide();
      }
      else {
        this.tgService.tg.mainButton.show();
      }
    }
    else {
      if(!this.tgService.tg.mainButton.isVisible) {
        this.tgService.tg.mainButton.show();
      }

      if(this.roomMe.isReady) {
        this.tgService.tg.mainButton.setText('Waiting for the opponent...');
        this.tgService.tg.mainButton.disable();
        this.tgService.tg.mainButton.setBackgroundColor(`#${this.disabledMainButtonColor}`);
      }
      else {
        this.tgService.tg.mainButton.setText('Ready');
        this.tgService.tg.mainButton.on('click', this.onMainButtonReadyClick);
      }
    }

  }

  ngOnDestroy(): void {
    this.tgService.tg.mainButton.off('click', this.onMainButtonReadyClick);
    this.tgService.tg.mainButton.off('click', this.onMainButtonActionClick);
  }

  onDropAttack = async (event: any) => {
    const dropzoneElement = event.target
    dropzoneElement.classList.remove('dropzone-active')

    const target = event.relatedTarget;
    this.attack(new Card(Number(target.getAttribute('suit')), Number(target.getAttribute('rank')))).then((result) => {
      if(!result) {
        target.style.transform = 'translate(0px, 0px)'
        target.setAttribute('data-x', 0)
        target.setAttribute('data-y', 0)
      }
    });
  }

  onDropDefend = async (event: any) => {
    const dropzoneElement = event.target
    dropzoneElement.classList.remove('defend-dropzone-active')
    dropzoneElement.children[0].classList.remove('card-wrapper')

    const target = event.relatedTarget;
    this.defend(Number(dropzoneElement.children[0].getAttribute('id')), new Card(Number(target.getAttribute('suit')), Number(target.getAttribute('rank')))).then((result) => {
      if(!result) {
        target.style.transform = 'translate(0px, 0px)'
        target.setAttribute('data-x', 0)
        target.setAttribute('data-y', 0)
      }
    });
  }

  dragMoveListener (event: any) {
    const target = event.target
    // keep the dragged position in the data-x/data-y attributes
    var x = (parseFloat(target.getAttribute('data-x')) || 0) + event.dx
    var y = (parseFloat(target.getAttribute('data-y')) || 0) + event.dy
  
    // translate the element
    target.style.transform = 'translate(' + x + 'px, ' + y + 'px)'
  
    // update the posiion attributes
    target.setAttribute('data-x', x)
    target.setAttribute('data-y', y)
  }
}
