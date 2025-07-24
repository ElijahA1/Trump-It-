namespace Trump_It_.Pages;

public partial class GameContent : ContentPage
{
	public GameContent()
	{
		InitializeComponent();
	}

	private async void homeBtnClicked(object sender, EventArgs e)
	{
		await Navigation.PopModalAsync();
	}
}  