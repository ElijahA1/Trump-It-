using Class_Practice;
using System.Threading.Tasks;

namespace Trump_It_.Pages;

public partial class GameContent : ContentPage
{
	public GameContent()
	{
		InitializeComponent();

        int rounds = 5;
        for (int i = 1; i <= rounds + 1; i++)
        {
            game.pushCardsToDeck();
            game.shuffleDeck();
            shuffleCardsAnim();
            //game.dealCards(i);
            //game.countBids(i);
            //for (int j = 0; j < i; j++)
            //{
            //    game.playersTurn();
            //    game.dealersTurn();
            //    game.handWinner();
            //}
        }
        //game.addBonusPoints();

    }

    Game game = new Game();

	private async void homeBtnClicked(object sender, EventArgs e)
	{
		await Navigation.PopModalAsync();
	}
    private async void OnCardTapped(object sender, EventArgs e)
    {
        cardFaceImageLeft.IsVisible = false; // Hide PNG at start
        card_flip_left.IsAnimationEnabled = true;// Run flip animation
        cardFaceImageMiddle.IsVisible = false; // Hide PNG at start
        card_flip_middle.IsAnimationEnabled = true;// Run flip animation
        cardFaceImageRight.IsVisible = false; // Hide PNG at start
        card_flip_right.IsAnimationEnabled = true;// Run flip animation

        await Task.Delay(200);           // Midpoint delay (adjust for your animation)

        // After flip midpoint, show card face PNG
        cardFaceImageLeft.Source = ImageSource.FromFile("ace_heart.png");
        cardFaceImageLeft.IsVisible = true;
        cardFaceImageMiddle.Source = ImageSource.FromFile("ace_star.png");
        cardFaceImageMiddle.IsVisible = true;
        cardFaceImageRight.Source = ImageSource.FromFile("ace_club.png");
        cardFaceImageRight.IsVisible = true;
    }
    private async Task shuffleCardsAnim() 
    {
        deck_shuffle_anim.IsAnimationEnabled=true;
        await Task.Delay(4000);
        deck_shuffle_anim.IsAnimationEnabled=false;
    }
}  