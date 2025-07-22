namespace Trump_It_
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }
        private async void homeCardTapped(object sender, TappedEventArgs e)
        {
            if (sender is Image tappedImage)
            {
                await tappedImage.ScaleTo(1.2, 150, Easing.CubicOut);
                await tappedImage.ScaleTo(1.0, 150, Easing.CubicIn);
            }
        }
    }
}
