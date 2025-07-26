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
    private async void OnCardTapped(object sender, EventArgs e)
    {
        cardFaceImage.IsVisible = false; // Hide PNG at start
        card_flip_anim.IsAnimationEnabled = true;// Run flip animation

        await Task.Delay(200);           // Midpoint delay (adjust for your animation)

        // After flip midpoint, show card face PNG
        cardFaceImage.Source = ImageSource.FromFile("ace_star.png");
        cardFaceImage.IsVisible = true;
    }
}  