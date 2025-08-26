using System;
using System.Threading.Tasks;
using SkiaSharp.Extended.UI.Controls;    // for SKLottieView
using Microsoft.Maui.Controls;
using Trump_It_.Pages;

namespace Trump_It_
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void homeCardTapped(object sender, TappedEventArgs e)
        {
            if (sender is Image tappedImage)
            {
                await tappedImage.TranslateTo(0, -50, 300);
                await tappedImage.TranslateTo(0, 0, 300);

                trumpetAnim.IsAnimationEnabled = false;    // stop current run
                trumpetAnim.Progress = TimeSpan.Zero;      // rewind to frame 0
                await Task.Delay(20);                      // allow control to update
                trumpetAnim.IsAnimationEnabled = true;     // start playing
            }
        }
        private async void playBtnClicked(object sender, EventArgs e) 
        {
            await Navigation.PushModalAsync(new GameContent());
        }
        private async void howToPlayBtnClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new HowToPlayContent());
        }
    }
}