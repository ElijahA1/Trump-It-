using Class_Practice;
using SkiaSharp.Extended.UI.Controls;
using System.Threading.Tasks;

namespace Trump_It_.Pages;

public partial class GameContent : ContentPage
{
    Game game = new Game();
    public GameContent()
	{
		InitializeComponent();

        int rounds = 5;

        Image[] cardFaces = new[]
        { card_face_0, card_face_1, card_face_2, card_face_3, card_face_4 };

        SKLottieView[] flipAnims = new[]
        { flip_card_anim_0, flip_card_anim_1, flip_card_anim_2, flip_card_anim_3, flip_card_anim_4 };

        for (int i = 1; i <= rounds + 1; i++)
        {
            game.pushCardsToDeck();
            game.shuffleDeck();
            shuffleCardsAnim();
            game.dealCards(i);
            dealCardAnimations(i, game.getPlayerCards());
        }

        void dealCardAnimations(int rounds, List<Card> playerHand)
        {
            for (int i = 0; i < cardFaces.Length; i++)
            {
                cardFaces[i].IsVisible = false;
                flipAnims[i].IsVisible = false;
            }

            // Populate only the first `rounds` slots
            for (int i = 0; i < rounds && i < cardFaces.Length; i++)
            {
                var card = playerHand[i];

                // Show the static face image
                cardFaces[i].Source = ImageSource.FromFile(card.ImagePath);
                cardFaces[i].IsVisible = true;

                // Keep the flip animation hidden until you tap to flip
                flipAnims[i].IsVisible = false;
            }
        }
    }
	private async void homeBtnClicked(object sender, EventArgs e)
	{
		await Navigation.PopModalAsync();
	}

    private async Task shuffleCardsAnim() 
    {
        deck_shuffle_anim.IsAnimationEnabled=true;
        await Task.Delay(4000);
        deck_shuffle_anim.IsAnimationEnabled=false;
    }
}  