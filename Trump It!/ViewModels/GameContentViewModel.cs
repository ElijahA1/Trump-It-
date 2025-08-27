using Card_Game;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.Maui.Audio;
using System.Collections.ObjectModel;

namespace Trump_It_.ViewModels
{
    public partial class GameContentViewModel : ObservableObject
    {
        Game Logic = new();
        public ObservableCollection<CardViewModel> PlayersHand { get; } = new();
        public ObservableCollection<CardViewModel> TrumpCard { get; } = new();
        public ObservableCollection<CardViewModel> PlayerCard { get; } = new();
        public ObservableCollection<CardViewModel> DealerCard { get; } = new();

        #region ObservableProperties

        [ObservableProperty]
        private int roundsPlayable;

        [ObservableProperty]
        private bool shufflingEnabled;
        [ObservableProperty]
        private bool setRoundsEnabled;
        [ObservableProperty]
        private bool biddingEnabled;
        [ObservableProperty]
        private bool canPlayCard;
        [ObservableProperty]
        private bool winningStatementEnabled;
        [ObservableProperty]
        private bool isGameAreaOpen;
        [ObservableProperty]
        private bool newGameEnabled;

        [ObservableProperty]
        private int playerBid;
        [ObservableProperty]
        private int playerTricks;
        [ObservableProperty]
        private int playerPoints;

        [ObservableProperty]
        private int dealerBid;
        [ObservableProperty]
        private int dealerTricks;
        [ObservableProperty]
        private int dealerPoints;

        [ObservableProperty]
        private string? winningStatement;
        #endregion

        #region RelayCommands
        [RelayCommand]
        public async Task ConfirmRounds()
        {
            SetRoundsEnabled = false;
            IsGameAreaOpen = false;

            await ShuffleCards();
            await StartRound();
        }

        [RelayCommand]
        public void ConfirmPlayerBid()
        {
            BiddingEnabled = false;

            // Set player's and dealer's current bid
            Logic.Player.CurrentBid = PlayerBid;
            Logic.SetDealersBid(RoundsPlayable);
            DealerBid = Logic.Dealer.CurrentBid;

            IsGameAreaOpen = false;
            CanPlayCard = true;
        }

        [RelayCommand]
        public async Task PlayCard(CardViewModel selectedCard)
        {
            if (selectedCard == null || Logic.Player.CardsInHand == null)
                return;

            // Stage game area
            CanPlayCard = false;
            IsGameAreaOpen = true;

            // Set player's chosen card
            PlayerCard.Add(selectedCard);
            PlayersHand.Remove(selectedCard);
            Logic.Player.CardInPlay = selectedCard.Card;
            Logic.Player.CardsInHand.Remove(selectedCard.Card);

            // Trigger dealer's move
            Logic.DealerPlaysCard();

            if (Logic.Dealer.CardInPlay == null)
                return;

            var dealersCard = new CardViewModel(Logic.Dealer.CardInPlay);
            DealerCard.Add(dealersCard);
            await RevealCardAnimation(dealersCard);

            // Remove Cards
            await Task.Delay(3000);
            PlayerCard.Clear();
            DealerCard.Clear();

            // Show winner
            await ShowHandWinner();

            if (PlayersHand.Count == 0)
                await EndRound();
        }

        [RelayCommand]
        public void StartNewGame() 
        {
            NewGameEnabled = false;
            CanPlayCard = false;
            RoundsPlayable = 1;
            PlayerPoints = PlayerTricks = PlayerBid = 0;
            DealerPoints = DealerTricks = DealerBid = 0;
            SetRoundsEnabled = true;
            IsGameAreaOpen = true;
        }
        #endregion

        #region GameActions
        public async Task ShuffleCards()
        {
            // Set or Reset bids and tricks to default
            PlayerBid = PlayerTricks = DealerBid = DealerTricks = 0;
            TrumpCard.Clear();

            Logic.AddCardsToDeck();
            Logic.ShuffleCardsInDeck();

            ShufflingEnabled = true;
            await Task.Delay(3000);
            ShufflingEnabled = false;
        }
        public async Task StartRound()
        {
            // Deal Cards logically
            Logic.DealCardsForRound(RoundsPlayable);

            // Check for null values
            if (Logic.Player.CardsInHand == null || Logic.TrumpCard == null)
                return;

            // Bind/Reveal Players Cards to Player Hand
            foreach (var card in Logic.Player.CardsInHand)
            {
                var cardVM = new CardViewModel(card);

                PlayersHand.Add(cardVM);
                await RevealCardAnimation(cardVM);
            }

            // Bind/Reveal Trump Card
            var trump = new CardViewModel(Logic.TrumpCard);
            TrumpCard.Add(trump);
            await RevealCardAnimation(trump);

            // Open Bidding
            BiddingEnabled = true;
            IsGameAreaOpen = true;
        }
        public async Task ShowHandWinner()
        {
            // Set winning statement Label text
            WinningStatementEnabled = true;
            WinningStatement = Logic.PlayerWonHand() switch
            {
                true => "Player won this hand. Tricks + 1",
                false => "Dealer won this hand. Dealer Tricks + 1"
            };

            PlayerTricks = Logic.Player.TricksWon;
            DealerTricks = Logic.Dealer.TricksWon;

            // Continue game
            await Task.Delay(3000);
            WinningStatementEnabled = false;
            IsGameAreaOpen = false;
            CanPlayCard = true;
        }
        public async Task EndRound()
        {
            // Add Points
            Logic.AddBonusPoints();
            PlayerPoints = Logic.Player.TotalPoints;
            DealerPoints = Logic.Dealer.TotalPoints;

            // Start next round or end the game
            RoundsPlayable--;

            if (RoundsPlayable == 0)
                await EndGame();
            else
            {
                await ShuffleCards();
                await StartRound();
            }
        }
        public async Task EndGame()
        {
            WinningStatement = PlayerPoints switch
            {
                var points when points > DealerPoints => "Congrats! You won the game",
                var points when points == DealerPoints => "You tied the game",
                _ => "Dealer won the game"
            };

            WinningStatementEnabled = true;
            IsGameAreaOpen = true;
            await Task.Delay(2000);

            // Request to Start New Game
            IsGameAreaOpen = false;
            WinningStatementEnabled = false;
            NewGameEnabled = true;
            IsGameAreaOpen = true;
        } 
        #endregion
        public async Task RevealCardAnimation(CardViewModel cardVM)
        {
            // Flip card
            cardVM.FlipAnimationEnabled = true;
            await Task.Delay(300);

            // Show card face
            cardVM.FlipAnimationEnabled = false;
            cardVM.IsFlipped = true;
        }
    }
}