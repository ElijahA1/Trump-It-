using Card_Game;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkiaSharp.Extended.UI.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

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
        int roundsPlayable = 1;

        [ObservableProperty]
        bool shufflingEnabled;
        [ObservableProperty]
        bool setRoundsEnabled;
        [ObservableProperty]
        bool biddingEnabled;
        [ObservableProperty]
        bool canPlayCard;
        [ObservableProperty]
        bool winningStatementEnabled;
        [ObservableProperty]
        bool isGameAreaOpen;

        [ObservableProperty]
        int playerBid;
        [ObservableProperty]
        int playerTricks;
        [ObservableProperty]
        int playerPoints;

        [ObservableProperty]
        int dealerBid;
        [ObservableProperty]
        int dealerTricks;
        [ObservableProperty]
        int dealerPoints;

        [ObservableProperty]
        string winningStatement;
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
            if (selectedCard == null)
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
        #endregion

        #region GameFlow
        public async Task ShuffleCards()
        {
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

            // Bind/Reveal Players Cards
            foreach (var card in Logic.Player.CardsInHand)
            {
                var cardVM = new CardViewModel(card);

                PlayersHand.Add(cardVM);
                await RevealCardAnimation(cardVM);
            }

            // Bind/Reveal Trump Card
            TrumpCard.Clear();
            var trump = new CardViewModel(Logic.TrumpCard);
            TrumpCard.Add(trump);
            await RevealCardAnimation(trump);

            // Open Bidding
            BiddingEnabled = true;
            IsGameAreaOpen = true;
        }
        public async Task ShowHandWinner()
        {
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
            Logic.AddBonusPoints();
            PlayerPoints = Logic.Player.TotalPoints;
            DealerPoints = Logic.Dealer.TotalPoints;

            RoundsPlayable--;
            await ShuffleCards();
            await StartRound();
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