namespace Trump_It_.Pages;

public partial class HowToPlayContent : ContentPage
{
	public HowToPlayContent()
	{
		InitializeComponent();
	}
    private async void homeBtnClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}