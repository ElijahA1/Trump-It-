using Card_Game;
using SkiaSharp.Extended.UI.Controls;
using Trump_It_.ViewModels;

namespace Trump_It_.Pages;

public partial class GameContent : ContentPage
{
    private GameContentViewModel ViewModel => BindingContext as GameContentViewModel;

    public GameContent()
    {
        InitializeComponent();
    }
    private TaskCompletionSource<bool> roundConfirmedTcs;
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await SetRounds();
        await StartGame();
    }
    private async Task StartGame() 
    {
        await ViewModel.ShuffleCards();
        ViewModel.DealCards();
        await ShowPlayerCards();
        await ShowTrumpCard();
        await Task.Delay(800);
        await StartBidding();
    }
    #region Repeatable Methods and Data Binding
    private SKLottieView CreateFlipAnimation() => new SKLottieView
    {
        IsAnimationEnabled = false,
        HeightRequest = 200,
        WidthRequest = 200,
        IsVisible = false,
        Source = (SKLottieImageSource?)SKFileLottieImageSource.FromFile("card_flip.json")
    };
    private Image CreateCardImage(string imagePath) => new Image
    {
        HeightRequest = 200,
        WidthRequest = 200,
        IsVisible = false,
        Aspect = Aspect.AspectFit,
        Source = ImageSource.FromFile(imagePath)
    };
    private void EnsureColumnExists(Grid grid, int index)
    {
        if (grid.ColumnDefinitions.Count <= index)
            grid.ColumnDefinitions.Add(new ColumnDefinition());
    }
    private void AddToGrid(View view, Grid grid, int column, int columnSpan, bool clearGrid)
    {
        if (clearGrid)
            grid.Clear();

        grid.Children.Add(view);
        grid.SetColumn(view, column);
        grid.SetColumnSpan(view, columnSpan);
    }
    private void AddTapGesture(View view, Action<object, EventArgs> action)
    {
        var gesture = new TapGestureRecognizer();
        gesture.Tapped += (s, e) => action(s, e);
        view.GestureRecognizers.Add(gesture);
    }
    private async Task SetRounds()
    {
        roundConfirmedTcs = new TaskCompletionSource<bool>();

        await OpenGameArea();

        RoundCounter.SetBinding(Label.TextProperty, nameof(ViewModel.Rounds));
        RoundSlider.SetBinding(Slider.ValueProperty, nameof(ViewModel.Rounds));

        AddTapGesture(RoundConfirmButton, async (s, e) =>
        {
            if (!roundConfirmedTcs.Task.IsCompleted)
            {
                await CloseGameArea();
                roundConfirmedTcs.TrySetResult(true);
            }
        });

        await roundConfirmedTcs.Task;
        BiddingGrid.Clear();
    }
    private void SetBids() // async removed not tested
    {
        Player.Bid = ViewModel.PlayerBid;
        ViewModel.Logic.DealerBid(ViewModel.Rounds);

        playerBidCount.Text = Player.Bid.ToString();
        dealerBidCount.Text = Dealer.Bid.ToString();
    }
    #endregion

    #region Interface
    private async Task ShowPlayerCards()
    {
        for (int i = 0; i < ViewModel.Rounds; i++)
        {
            Card card = Player.Hand[i];

            var cardFlip = CreateFlipAnimation();
            var cardImage = CreateCardImage(card.ImagePath);
            AddTapGesture(cardImage, CardTapped);

            EnsureColumnExists(PlayerHandGrid, i);
            AddToGrid(cardFlip, PlayerHandGrid, i, 1, false);
            AddToGrid(cardImage, PlayerHandGrid, i, 1, false);

            ViewModel.RevealCard(cardFlip, cardImage);
        }
    }
    private async Task ShowTrumpCard()
    {
        trump_card_face.Source = ImageSource.FromFile(ViewModel.Logic.TrumpCard.ImagePath);
        ViewModel.RevealCard(flip_trump_card, trump_card_face);
    }
    private async Task OpenGameArea()
    {
        ViewModel.CanPlayCard = false;
        await Task.WhenAll(BiddingStack.ScaleYTo(1, 300, Easing.CubicOut));
    }
    private async Task CloseGameArea()
    {
        ViewModel.CanPlayCard = true;
        await Task.WhenAll(BiddingStack.ScaleYTo(0, 300, Easing.CubicOut));
    }
    private async Task StartBidding()
    {
        await OpenGameArea();

        Button betButton = new()
        {
            Text = "BET TRICKS",
            TextColor = Colors.WhiteSmoke,
            FontSize = 20,
            WidthRequest = 150,
            HeightRequest = 70,
            BackgroundColor = Colors.DarkRed,
        };
        Button passButton = new()
        {
            Text = "PASS",
            TextColor = Colors.WhiteSmoke,
            FontSize = 20,
            WidthRequest = 150,
            HeightRequest = 70,
            BackgroundColor = Colors.DarkGreen,
        };

        AddTapGesture(betButton, BetButtonClicked);
        AddTapGesture(passButton, PassButtonClicked);

        AddToGrid(betButton, BiddingGrid, 0, 1, true);
        AddToGrid(passButton, BiddingGrid, 2, 1, false);
    }
    private async Task RoundWinner()
    {
        if (ViewModel.Logic.PlayerWon())
        {
            Label label = new Label
            { Text = "Player has won the hand", TextColor = Colors.Black, FontSize = 25, FontAttributes = FontAttributes.Bold, WidthRequest = 200, HeightRequest = 200 };

            AddToGrid(label, BiddingGrid, 0, 2, true);
            UpdateScoreBoard();
        }
        else
        {
            Label label = new Label
            { Text = "Dealer has won the hand", TextColor = Colors.Black, FontSize = 25, FontAttributes = FontAttributes.Bold, WidthRequest = 200, HeightRequest = 200 };

            AddToGrid(label, BiddingGrid, 0, 2, true);
            UpdateScoreBoard();
        }
    }
    private void UpdateScoreBoard()
    {
        playerTrickCount.Text = Player.Tricks.ToString();
        dealerTrickCount.Text = Dealer.Tricks.ToString();
    }

    #endregion

    #region User Interactions
    private async void BetButtonClicked(object sender, EventArgs e)
    {
        Label counter = new()
        {
            Text = "",
            TextColor = Colors.Black,
            FontSize = 18,
            WidthRequest = 100,
            HeightRequest = 100,
            BackgroundColor = Colors.BlanchedAlmond,
        };
        Slider bidSlider = new()
        {
            Minimum = 0,
            Maximum = ViewModel.Rounds,
            ThumbColor = Colors.Black,
            MinimumTrackColor = Colors.DarkGoldenrod,
            MaximumTrackColor = Colors.LightGray,
            WidthRequest = 200,
            HeightRequest = 50,
        };
        Button confirmButton = new()
        {
            Text = "BID IT!",
            TextColor = Colors.WhiteSmoke,
            FontSize = 18,
            WidthRequest = 150,
            HeightRequest = 70,
            BackgroundColor = Colors.Black,
        };

        AddTapGesture(confirmButton, (s, e) => CloseGameArea());
        AddTapGesture(confirmButton, (s, e) => SetBids());

        counter.SetBinding(Label.TextProperty, nameof(ViewModel.PlayerBid));
        bidSlider.SetBinding(Slider.ValueProperty, nameof(ViewModel.PlayerBid));

        EnsureColumnExists(BiddingGrid, 3);
        AddToGrid(counter, BiddingGrid, 0, 1, true);
        AddToGrid(bidSlider, BiddingGrid, 1, 1, false);
        AddToGrid(confirmButton, BiddingGrid, 2, 1, false);
    }
    private async void PassButtonClicked(object sender, EventArgs e) 
    {
        ViewModel.PlayerBid = 0;
        CloseGameArea();
        SetBids();
    }
    private async void CardTapped(object sender, EventArgs e)
    {
        if (!ViewModel.CanPlayCard) return;

        var tappedImage = sender as Image;
        var fileSource = tappedImage.Source as FileImageSource;
        ViewModel.PlayHand(fileSource.File);
        PlayerHandGrid.Children.Remove(tappedImage);

        // Create flip animation to apply to dealer card
        SKLottieView flipAnimation = CreateFlipAnimation();
        flipAnimation.HeightRequest = 170;

        // Create Dealer card
        Image dealerCard = CreateCardImage(Dealer.CardInPlay.ImagePath);
        dealerCard.HeightRequest = 170;

        // Create Player Card
        tappedImage = CreateCardImage(Player.CardInPlay.ImagePath);
        tappedImage.HeightRequest = 170;
        tappedImage.IsVisible = true;

        // Append all items to Grid
        AddToGrid(tappedImage, BiddingGrid, 0, 1, true);
        AddToGrid(flipAnimation, BiddingGrid, 2, 1, false);
        AddToGrid(dealerCard, BiddingGrid, 2, 1, false);

        // Game Logic
        await OpenGameArea();
        await ViewModel.RevealCard(flipAnimation, dealerCard);
        await Task.Delay(2500);
        await RoundWinner();
        await Task.Delay(2000);
        CloseGameArea();
    }
    #endregion

    #region Navigation and Button Events
    private async void homeBtnClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
    #endregion
}