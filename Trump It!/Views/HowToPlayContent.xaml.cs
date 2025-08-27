using Trump_It_.ViewModels;
namespace Trump_It_.Pages;

public partial class HowToPlayContent : ContentPage
{
    public HowToPlayContent()
	{
		InitializeComponent();
        BindingContext = new HowToPlayViewModel();
    }
    private async void HomeButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}