using Card_Game;
using SkiaSharp.Extended.UI.Controls;
using System.Threading.Tasks;
using Trump_It_.ViewModels;

namespace Trump_It_.Pages;

public partial class GameContent : ContentPage
{
    private GameContentViewModel ViewModel => BindingContext as GameContentViewModel;

    public GameContent()
    {
        InitializeComponent();
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        await ViewModel.ShuffleCards();
        ViewModel.DealCards();
        await ShowPlayerCards(ViewModel.rounds);
        await ShowTrumpCard();
        await Task.Delay(800);
        await OpenBiddingArea();
    }
    #region Card Display Methods
    private async Task ShowPlayerCards(int rounds)
    {
        for (int i = 0; i < rounds; i++)
        {
            Card card = Player.Hand[i];

            SKLottieView flipAnimation = new SKLottieView()
            { IsAnimationEnabled = false, HeightRequest = 200, WidthRequest = 200, IsVisible = false, Source = (SKLottieImageSource?)SKFileLottieImageSource.FromFile("card_flip.json") };

            Image cardImage = new Image()
            { HeightRequest = 200, WidthRequest = 200, IsVisible = false, Aspect = Aspect.AspectFit, Source = ImageSource.FromFile(card.ImagePath) };

            TapGestureRecognizer tapGesture = new();
            tapGesture.Tapped += CardTapped;
            cardImage.GestureRecognizers.Add(tapGesture);

            playerHandGrid.Children.Add(flipAnimation);
            playerHandGrid.Children.Add(cardImage);
            if (playerHandGrid.ColumnDefinitions.Count <= i)
                playerHandGrid.ColumnDefinitions.Add(new ColumnDefinition());
            playerHandGrid.SetColumn(flipAnimation, i);
            playerHandGrid.SetColumn(cardImage, i);

            ViewModel.RevealCard(flipAnimation, cardImage);

            await Task.Delay(200);
        }
    }
    private async Task ShowTrumpCard()
    {
        trump_card_face.Source = ImageSource.FromFile(ViewModel.Logic.TrumpCard.ImagePath);
        ViewModel.RevealCard(flip_trump_card, trump_card_face);
    }
    #endregion

    #region Binding UI and Interaction
    private async Task OpenBiddingArea()
    {
        ViewModel.CanPlayCard = false;
        await Task.WhenAll(BiddingStack.ScaleYTo(1, 300, Easing.CubicOut));
    }
    private async void CloseBiddingArea(object sender, EventArgs e)
    {
        ViewModel.CanPlayCard = true;
        await Task.WhenAll(BiddingStack.ScaleYTo(0, 300, Easing.CubicOut));
    }
    private async void BidClicked(object sender, EventArgs e)
    {
        Label counter = new()
        {
            Text = "0",
            TextColor = Colors.Black,
            FontSize = 18,
            WidthRequest = 100,
            HeightRequest = 100,
            BackgroundColor = Colors.BlanchedAlmond,
        };
        Slider bidSlider = new()
        {
            Minimum = 0,
            Maximum = ViewModel.rounds,
            ThumbColor = Colors.Black,
            MinimumTrackColor = Colors.DarkGoldenrod,
            MaximumTrackColor = Colors.LightGray,
            WidthRequest = 300,
            HeightRequest = 50,
        };
        Button confirm = new()
        {
            Text = "BID IT!",
            TextColor = Colors.BlanchedAlmond,
            FontSize = 18,
            WidthRequest = 200,
            HeightRequest = 70,
            BackgroundColor = Colors.Black,
        };

        TapGestureRecognizer tapGesture = new();
        tapGesture.Tapped += CloseBiddingArea;
        tapGesture.Tapped += SetBids;
        confirm.GestureRecognizers.Add(tapGesture);

        counter.SetBinding(Label.TextProperty, nameof(ViewModel.PlayerBid));
        bidSlider.SetBinding(Slider.ValueProperty, nameof(ViewModel.PlayerBid));

        BiddingGrid.Children.Clear();
        BiddingGrid.Children.Add(counter);
        BiddingGrid.Children.Add(bidSlider);
        BiddingGrid.Children.Add(confirm);
        if (BiddingGrid.ColumnDefinitions.Count < 3)
            BiddingGrid.ColumnDefinitions.Add(new ColumnDefinition());
        BiddingGrid.SetColumn(counter, 0);
        BiddingGrid.SetColumn(bidSlider, 1);
        BiddingGrid.SetColumn(confirm, 2);
    }
    private async void PassClicked(object sender, EventArgs e) 
    {
        ViewModel.PlayerBid = 0;
        CloseBiddingArea(sender, e);
        SetBids(sender ,e);
    }
    private async void SetBids(object sender, EventArgs e)
    {
        Player.Bid = ViewModel.PlayerBid;
        ViewModel.Logic.DealerBid(ViewModel.rounds);

        playerBidCount.Text = Player.Bid.ToString();
        dealerBidCount.Text = Dealer.Bid.ToString();
    }
    #endregion

    #region Navigation and Button Events
    private async void homeBtnClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
    #endregion

    private async void CardTapped(object sender, EventArgs e)
    {
        if (!ViewModel.CanPlayCard) return;

        BiddingGrid.Children.Clear();

        SKLottieView flipAnimation = new SKLottieView()
        { IsAnimationEnabled = false, HeightRequest = 170, WidthRequest = 200, IsVisible = false, Source = (SKLottieImageSource?)SKFileLottieImageSource.FromFile("card_flip.json") };
        Image dealerCard = new()
        { WidthRequest = 200, HeightRequest = 170, IsVisible = false };
        Image playerCard = new()
        { WidthRequest = 200, HeightRequest = 170, IsVisible = true };


        if (sender is Image tappedImage && tappedImage.Source is FileImageSource fileSource)
        {
            playerHandGrid.Children.Remove(tappedImage);

            ViewModel.PlayHand(fileSource.File);

            playerCard.Source = tappedImage.Source;
            dealerCard.Source = ImageSource.FromFile(Dealer.CardInPlay.ImagePath);
        }
       
        BiddingGrid.Children.Add(dealerCard);
        BiddingGrid.Children.Add(playerCard);
        BiddingGrid.Children.Add(flipAnimation);

        BiddingGrid.SetColumn(flipAnimation, 1);
        BiddingGrid.SetColumn(dealerCard, 1);
        BiddingGrid.SetColumn(playerCard, 0);

        await OpenBiddingArea();
        await ViewModel.RevealCard(flipAnimation, dealerCard);
        await Task.Delay(2500);
        await RoundWinner();
        await Task.Delay(2000);
        CloseBiddingArea(sender,e);
    }
    private async Task RoundWinner()
    {
        if (ViewModel.Logic.PlayerWon())
        {
            Label label = new Label
            { Text = "Player has won the hand", TextColor = Colors.Black, FontSize = 25, FontAttributes= FontAttributes.Bold, WidthRequest = 200, HeightRequest = 200 };
            BiddingGrid.Clear();
            BiddingGrid.Children.Add(label);
            BiddingGrid.SetColumn(label, 0);
            BiddingGrid.SetColumnSpan(label, 2);
            UpdateScoreBoard();
        }
        else
        {
            Label label = new Label
            { Text = "Dealer has won the hand", TextColor = Colors.Black, FontSize = 25, FontAttributes = FontAttributes.Bold, WidthRequest = 200, HeightRequest = 200 };
            BiddingGrid.Clear();
            BiddingGrid.Children.Add(label);
            BiddingGrid.SetColumn(label, 0);
            BiddingGrid.SetColumnSpan(label, 2);
            UpdateScoreBoard();
        }
    }
    private void UpdateScoreBoard()
    {
        playerTrickCount.Text = Player.Tricks.ToString();
        dealerTrickCount.Text = Dealer.Tricks.ToString();
    }
}