using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Trump_It_.ViewModels
{
    public partial class HowToPlayViewModel : ObservableObject
    {
        private int imageNumber = 1;

        [ObservableProperty]
        private string topImage = "top_image_1.png";
        [ObservableProperty]
        private string bottomImage = "bottom_image_1.png";
        [ObservableProperty]
        private string description = "To start the game select the number of rounds to play. The round will indicate the amount of cards dealt for that round. After each round the number of cards" +
            " in your hand will decrease until the final round. After each round, your points and the dealer's points are tallied. The player with the most points, after the final round, wins the game.";

        [RelayCommand]
        public void SlideLeft()
        {
            if (imageNumber == 1)
                return;

            imageNumber--;
            Description = ChooseDescription(imageNumber);

            if (imageNumber == 4)
            {
                TopImage = $"top_image_{imageNumber}.png";
                BottomImage = "bottom_image_2.png";
                return;
            }

            TopImage = $"top_image_{imageNumber}.png";
            BottomImage = $"bottom_image_{imageNumber}.png";
        }

        [RelayCommand]
        public void SlideRight()
        {
            if(imageNumber == 5)
                return;

            imageNumber++;
            Description = ChooseDescription(imageNumber);

            if (imageNumber == 4)
            {
                TopImage = $"top_image_{imageNumber}.png";
                BottomImage = "bottom_image_2.png";
                return;
            }
            TopImage = $"top_image_{imageNumber}.png";
            BottomImage = $"bottom_image_{imageNumber}.png";
        }

        private static string ChooseDescription(int slideNumber) 
        {
            switch (slideNumber) 
            {
                case 1:
                    return "To start the game select the number of rounds to play. The round will indicate the amount of cards dealt for that round. After each round the number of cards in your hand will decrease until the final round. After each round, your points and the dealer's points are tallied. The player with the most points, after the final round, wins the game.";
                case 2:
                    return "At the start of each round, you will bid(bet) on how many tricks(hands) you think you will win. When you play a card the dealer will also play a card of the same suit, if the dealer has one. If the card you play is stronger than the card the dealer plays, you will win that trick(hand). Once then final hand is played for that round, and you have no more cards, points will be tallied. If your tricks(hands) won matches your bid(bet) you get 5 bonus points.";
                case 3:
                    return "Trump cards are cards that match the suit of the trump card dealt at the beginning of the round on the board. When a trump card is played it will beat all other cards played regardless of their value. Only a trump card of a higher value can beat another trump card. When cards of the same suit are played, the card with the higher value wins that trick. The ace is the highest value for a card.";
                case 4:
                    return "If you play an off-suit card(card that is not trump) and the dealer does not have that suit, the dealer can play a card of any suit. If the dealer's card is not a trump card, or is not the same suit, you will win that trick(hand) regardless of the values of cards played.";
                default:
                    return "Points are tallied at the end of each round after the final cards are played. You get one point for each trick you won. If your tricks match your bid you get 5 bonus points.";
            }
        }
    }
}
