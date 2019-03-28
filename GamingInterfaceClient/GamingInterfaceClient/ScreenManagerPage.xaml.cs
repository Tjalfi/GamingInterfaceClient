using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GamingInterfaceClient
{
    using Models.Screen;
    using Renderer;
    using ScreenManager;
    using ScreenManager.IContract;

    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ScreenManagerPage : ContentPage, IView
	{
        // permissions request code
        private readonly int REQUEST_CODE_ASK_PERMISSIONS = 501;
        private readonly int REQUEST_CODE_IMPORT = 510;

        private IViewActionListener actionListener;
        private Dictionary<int, string> screenList;
        private int selectedScreenIndex = 0;

        public ScreenManagerPage ()
		{
			InitializeComponent ();
            SetViewActionListener(new Presentation(new ScreenRepository(), this));
            actionListener.Load();
        }

        public void SetViewActionListener(IViewActionListener listener)
        {
            actionListener = listener;
        }

        public void ShowError(string error)
        {
            Toast.LongAlert(error);
        }

        public void ShowMessage(string message)
        {
            Toast.LongAlert(message);
        }

        public void SetProgressIndicator(bool show)
        {
            grdLoading.IsVisible = show;
        }

        public void UpdateSpinner(Dictionary<int, string> screenList)
        {
            List<string> sourceList = new List<string>();
            foreach (KeyValuePair<int, string> pair in screenList)
            {
                sourceList.Add(pair.Value);
            }
            pcrScreens.ItemsSource = sourceList;
            pcrScreens.SelectedIndex = 0;
        }

        public void SetSpinnerSelection(int screenId)
        {
            pcrScreens.SelectedItem = screenList[screenId];
        }

        private void PcrScreens_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (KeyValuePair<int, string> pair in screenList)
            {
                if (pair.Value == (string)pcrScreens.SelectedItem)
                {
                    selectedScreenIndex = pair.Key;
                    entName.Text = pair.Value;
                }
            }
        }

        private void BtnExport_Clicked(object sender, EventArgs e)
        {
            actionListener.ExportCurrent(selectedScreenIndex);
        }

        private void BtnEdit_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new EditPage(selectedScreenIndex));
        }

        private void BtnNew_Clicked(object sender, EventArgs e)
        {
            actionListener.Create();
        }

        private void BtnDelete_Clicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                bool result = await DisplayAlert("", string.Format(Application.Current.Resources["strPageScreensConfirmDelete"].ToString(), screenList[selectedScreenIndex]), Application.Current.Resources["strYes"].ToString(), Application.Current.Resources["strNo"].ToString());
                if (result)
                {
                    actionListener.Delete(selectedScreenIndex);
                }
            });
        }

        private void BtnUpdate_Clicked(object sender, EventArgs e)
        {
            actionListener.Update(selectedScreenIndex, entName.Text);
        }

        private void BtnImport_Clicked(object sender, EventArgs e)
        {
        }
    }
}