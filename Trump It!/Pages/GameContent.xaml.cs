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

        //GamePlay flow
            biddingGrid.Children.Clear();
            game.pushCardsToDeck();
            game.shuffleDeck();
            shuffleCardsAnim();

            // Deal cards plus animations
            game.dealCards(rounds);
            dealCardAnimations(rounds, Player.Hand);
            trumpCardAnimation(game.trumpCard);
            generateBidButtons(rounds);
            game.dealerBid(Player.Bid, rounds);

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
    private async Task generateBidButtons(int rounds) 
    {
        for (int i = 0; i <= rounds; i++)
        {
            Button button = new Button
            { Text = $"{i} tricks", WidthRequest = 150, HeightRequest = 60, Background = new SolidColorBrush(Colors.SkyBlue)};
            if (biddingGrid.ColumnDefinitions.Count <= i)
                biddingGrid.ColumnDefinitions.Add(new ColumnDefinition());
            biddingGrid.Children.Add(button);
            biddingGrid.SetColumn(button, i);
            button.Clicked += playerBid;
        }
    }
    private async void playerBid(object sender, EventArgs e) 
    {
        string myString = (sender as Button).Text.ToString();
        string[] subs = myString.Split(' ');
        Player.Bid = int.Parse(subs[0]);
        biddingGrid.Children.Clear();
        Label label = new Label { Text=$"You bid {Player.Bid}", FontSize=20, };
        biddingGrid.Children.Add(label);
    }
    private async void playerCardSelected(object sender, EventArgs e)
    {
        if (sender is Image tappedImage && tappedImage.Source is FileImageSource fileSource)
        {
            string tappedPath = fileSource.File;

            for (int i = 0; i < Player.Hand.Count; i++)
            {
                var card = Player.Hand[i];
                if (card.ImagePath == tappedPath)
                {
                    Player.CardInPlay = card;
                    game.playersTurn(card);
                    playerHandGrid.Children.Remove(tappedImage);
                    biddingGrid.Children.Add(tappedImage);
                    break;
                }
            }
        }
        game.dealersTurn();
        Image dealerCardImage = new Image
        { Source = ImageSource.FromFile(Dealer.CardInPlay.ImagePath), WidthRequest = 200, HeightRequest = 200};

        // Add to grid
        biddingGrid.Children.Add(dealerCardImage);

        await Task.Delay(3000);
        roundWinner();
    }
    private async void roundWinner() 
    {
        if (game.handWinner())
        {
            Label label = new Label
            {Text="Player has won the hand", FontSize=50, WidthRequest=400, HeightRequest=200};
            biddingGrid.Clear();
            biddingGrid.Children.Add(label);
        } else
        {
            Label label = new Label
            { Text = "Player has lost the hand", FontSize = 50, WidthRequest = 400, HeightRequest = 200 };
            biddingGrid.Clear();
            biddingGrid.Children.Add(label);
        }
    }
    private async void homeBtnClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}  