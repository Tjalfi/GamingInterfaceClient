using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;

namespace GamingInterfaceClient
{
    using Extensions;
    using Models.Screen;
    using Network;
    using Renderer;
    using Utils;

    public partial class MainPage : ContentPage
    {
        private readonly string PREFS_CHOSEN_ID = "chosen_id";
        private Dictionary<int, string> tmpScreenList;

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
            tmpScreenList = new Dictionary<int, string>();
            
            string password = "";

            if (Application.Current.Properties.ContainsKey("password"))
            {
                password = (string)Application.Current.Properties["password"];
                try
                {
                    password = CryptoHelper.Decrypt(password);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("start: Password Decryption Failure: " + e.Message + "\n" + e.StackTrace);
                    password = "";
                }
            }
            entPassword.Text = password;
            entPort.Text = Application.Current.Properties.ContainsKey("port") ? Application.Current.Properties["port"].ToString() : "8091";
            entAddress.Text = Application.Current.Properties.ContainsKey("address") ? Application.Current.Properties["address"].ToString() : "";
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ScreenRepository screenRepository = new ScreenRepository();
            screenRepository.LoadScreens(new LoadCallback((List<IScreen> screens) =>
            {
                screenRepository.GetScreenList(new LoadScreenListCallback((Dictionary<int, string> screenList) =>
                {
                    tmpScreenList = screenList;
                    BuildScreenSpinner(screenList);
                }));
            }));
        }

        private void BuildScreenSpinner(Dictionary<int, string> screenList)
        {
            List<string> sourceList = new List<string>();
            int chosenId = 0;
            if (Application.Current.Properties.ContainsKey(PREFS_CHOSEN_ID))
            {
                chosenId = (int)Application.Current.Properties[PREFS_CHOSEN_ID];
            }
            int chosenIndex = 0;

            int i = 0;
            foreach (KeyValuePair<int, string> pair in screenList)
            {
                sourceList.Add(pair.Value);
                if (pair.Key == chosenId)
                {
                    chosenIndex = i;
                }
                i++;
            }

            pcrScreens.ItemsSource = sourceList;
            pcrScreens.SelectedIndex = chosenIndex;
        }

        private void CheckServerVersion(string address, int port, GamePage page)
        {
            string url = "http://" + address + ":" + port + "/";
            CommandService routeMap = new CommandService(url);
            CommandResult<string> result = routeMap.GetVersion();
            if (result != null && result.successful && result.response != "")
            {
                if (result.response.Equals("1.3.0.0"))
                {
                    foreach (KeyValuePair<int, string> pair in tmpScreenList)
                    {
                        if (pair.Value == (string)pcrScreens.SelectedItem)
                        {
                            page.screenIndex = pair.Key;
                        }
                    }
                    Navigation.PushAsync(page);
                }
                else
                {
                    DisplayUpgradeWarning();
                }
            }
            else
            {
                Toast.LongAlert("Something went wrong...Please try later! " + result.error);
            }
        }

        private void DisplayUpgradeWarning()
        {

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
            Navigation.PushAsync(new ScreenManagerPage());
        }

        private void BtnStart_Clicked(object sender, EventArgs e)
        {
            string address = entAddress.Text;
            int port = 0;
            string password = entPassword.Text;


            if (password.Length < 6)
            {
                Toast.ShortAlert(Application.Current.Resources["strPasswordInvalid"].ToString());
                return;
            }
            if (!int.TryParse(entPort.Text, out port))
            {
                Toast.ShortAlert(Application.Current.Resources["strPortInvalid"].ToString());
                return;
            }

            if (address.Length < 7)
            {
                Toast.ShortAlert(Application.Current.Resources["strAddressInvalid"].ToString());
                return;
            }

            try
            {
                password = CryptoHelper.Encrypt(password);
            }
            catch (Exception ex)
            {
                password = "";
                Debug.WriteLine(ex.StackTrace);
            }

            if (password == null || password == "")
            {
                Toast.ShortAlert(Application.Current.Resources["strPasswordInvalid"].ToString());
                return;
            }

            address = address.Trim();

            GamePage gamePage = new GamePage(address, port, password);

            Application.Current.Properties.AddOrSet("address", address);
            Application.Current.Properties.AddOrSet("port", port);
            Application.Current.Properties.AddOrSet("password", password);
            Application.Current.SavePropertiesAsync();

            CheckServerVersion(address, port, gamePage);
        }

        private void PcrScreens_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (KeyValuePair<int, string> pair in tmpScreenList)
            {
                if (pair.Value == (string)pcrScreens.SelectedItem)
                {
                    Application.Current.Properties.AddOrSet(PREFS_CHOSEN_ID, pair.Key);
                    Application.Current.SavePropertiesAsync();
                    break;
                }
            }
        }
    }
}
