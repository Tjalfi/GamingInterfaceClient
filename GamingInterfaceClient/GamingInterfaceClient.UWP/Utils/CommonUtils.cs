using Windows.Storage;
using Xamarin.Forms;

[assembly: Dependency(typeof(GamingInterfaceClient.Utils.UWP.CommonUtils))]
namespace GamingInterfaceClient.Utils.UWP
{
    class CommonUtils : ICommonUtils
    {
        public string GetDataDir()
        {
            return ApplicationData.Current.LocalFolder.Path;
        }
    }
}
