
using Trump_It_.ViewModels;

namespace Trump_It_.Pages;

public partial class GameContent : ContentPage
{
    public GameContentViewModel ViewModel { get; }
    public GameContent()
    {
        InitializeComponent();
        ViewModel = new GameContentViewModel();
        BindingContext = ViewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        ViewModel.CanPlayCard = false;

        // Open and Close game area
        ViewModel.PropertyChanged += async (s, e) =>
        {
            if (e.PropertyName == nameof(ViewModel.IsGameAreaOpen))
            {
                if (ViewModel.IsGameAreaOpen)
                {
                    await GamePlayAreaStack.ScaleYTo(1, 300, Easing.CubicOut);
                }
                else
                {
                    await GamePlayAreaStack.ScaleYTo(0, 300, Easing.CubicIn);
                }
            }
        };
        
        // End the game when number of rounds is zero
        ViewModel.PropertyChanged += async (s, e) =>
        {
            if (e.PropertyName == nameof(ViewModel.RoundsPlayable))
            {
                if (ViewModel.RoundsPlayable == 0)
                {
                    await ViewModel.EndGame();
                    await Navigation.PopModalAsync();
                    return;
                }
            }
        };

        ViewModel.SetRoundsEnabled = true;
        ViewModel.IsGameAreaOpen = true;
    }
    private async void HomeButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}