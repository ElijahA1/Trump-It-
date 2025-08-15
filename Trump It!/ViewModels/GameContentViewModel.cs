using Card_Game;
using SkiaSharp.Extended.UI.Controls;
using System.ComponentModel;

namespace Trump_It_.ViewModels
{
    public class GameContentViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new(propertyName));

        public GameLogic Logic { get; } = new();

        private bool isShuffling;
        private bool canPlayCard;
        private int playerBid;
        private int rounds;
        public int Rounds
        {
            get => rounds;
            set
            {
                rounds = value;
                OnPropertyChanged(nameof(Rounds));
            }
        }
        public bool IsShuffling
        {
            get => isShuffling;
            set
            {
                isShuffling = value;
                OnPropertyChanged(nameof(IsShuffling));
            }
        }
        public int PlayerBid
        {
            get => playerBid;
            set
            {
                playerBid = value;
                OnPropertyChanged(nameof(PlayerBid));
            }
        }
        public bool CanPlayCard
        {
            get => canPlayCard;
            set
            {
                if (canPlayCard != value)
                {
                    canPlayCard = value;
                    OnPropertyChanged(nameof(CanPlayCard));
                }
            }
        }


        // -- New Game Flow --
        public async Task ShuffleCards()
        {
            Logic.PushCardsToDeck();
            Logic.ShuffleDeck();

            IsShuffling = true;
            await Task.Delay(3000);
            IsShuffling = false;
        }
        public void DealCards() 
        {
            if (Player.Hand == null)
                Logic.DealCards(rounds);
            else
            { 
                Player.Hand.Clear();
                Logic.DealCards(rounds);
            }
        }
        public void PlayHand(string filePath) 
        {
            for (int i = 0; i < Player.Hand.Count; i++)
            {
                var card = Player.Hand[i];
                if (card.ImagePath == filePath)
                {
                    Player.CardInPlay = card;
                    break;
                }
            }
            Logic.PlayerTurn(Player.CardInPlay);
            Logic.DealersTurn();
        }
        public async Task RevealCard(SKLottieView animation, Image cardImage) 
        {
            animation.IsVisible = true;
            animation.IsAnimationEnabled = true;
            await Task.Delay(300);

            animation.IsAnimationEnabled = false;
            animation.IsVisible = false;
            cardImage.IsVisible = true;
            await Task.Delay(200);
        }
    }
}