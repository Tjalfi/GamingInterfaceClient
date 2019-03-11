using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GamingInterfaceClient
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GamePage : ContentPage
	{
        private string address;
        private int port;
        private string password;
        public int screenIndex;

        public GamePage (string address, int port, string password)
		{
			InitializeComponent ();

            this.address = address;
            this.port = port;
            this.password = password;
		}
	}
}