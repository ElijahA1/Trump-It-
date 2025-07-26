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

        // GamePlay flow
        for (int i = 1; i <= rounds + 1; i++)
        {
            game.pushCardsToDeck();
            game.shuffleDeck();
            shuffleCardsAnim();
            game.dealCards(i);
            dealCardAnimations(i, game.getPlayerCards());
            trumpCardAnimation(game.trumpCard);
        }

        // Assign player cards from list player.hand to images
        async Task dealCardAnimations(int rounds, List<Card> playerHand)
        {
            for (int i = 0; i < rounds-1; i++)
            {
                var card = playerHand[i];

                cardFaces[i].IsVisible = false;
                cardFaces[i].Source = ImageSource.FromFile(card.ImagePath);

                flipAnims[i].IsVisible = true;
                flipAnims[i].IsAnimationEnabled = true;
                await Task.Delay(200);

                flipAnims[i].IsAnimationEnabled = false;
                flipAnims[i].IsVisible=false;
                cardFaces[i].IsVisible = true;
            }
        }
        async Task trumpCardAnimation(Card trumpCard) 
        {
           trump_card_face.IsVisible = false;
           trump_card_face.Source = ImageSource.FromFile(trumpCard.ImagePath);

           flip_trump_card.IsVisible = true;
           flip_trump_card.IsAnimationEnabled = true;
           await Task.Delay(200);
           
           flip_trump_card.IsVisible=false;
           trump_card_face.IsVisible = true;
        }
    }
    private async Task shuffleCardsAnim() 
    {
        deck_shuffle_anim.IsAnimationEnabled=true;
        await Task.Delay(4000);
        deck_shuffle_anim.IsAnimationEnabled=false;
    }
    private async void homeBtnClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}  