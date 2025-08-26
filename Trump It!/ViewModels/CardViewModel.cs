using Card_Game;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Trump_It_.ViewModels
{
    public partial class CardViewModel : ObservableObject
    {
        public Card Card { get; }
        public string ImagePath => Card.ImagePath;

        [ObservableProperty]
        bool isFlipped;
        [ObservableProperty]
        bool flipAnimationEnabled;

        public CardViewModel(Card card)
        {
            Card = card;
        }
    }
}