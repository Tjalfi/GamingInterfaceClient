using System;
using Xamarin.Forms;

namespace GamingInterfaceClient
{
    using Utils;

    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        private void TbiHelp_Activated(object sender, EventArgs e)
        {
            CommonUtils.OpenBrowser("https://github.com/Terence-D/GamingInterfaceClientAndroid/wiki");
        }

        private void TbiToggleTheme_Activated(object sender, EventArgs e)
        {

        }

        private void TbiAbout_Activated(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AboutPage());
        }

        private void BtnScreenManager_Clicked(object sender, EventArgs e)
        {

        }

        private void BtnStart_Clicked(object sender, EventArgs e)
        {

        }
    }
}
