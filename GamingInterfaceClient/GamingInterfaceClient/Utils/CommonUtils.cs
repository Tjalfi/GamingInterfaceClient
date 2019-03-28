using System;
using Xamarin.Forms;

namespace GamingInterfaceClient.Utils
{
    static class CommonUtils
    {
        public static string GetDataDir()
        {
            return DependencyService.Get<ICommonUtils>().GetDataDir();
        }

        public static void OpenBrowser(string url)
        {
            Device.OpenUri(new Uri(url));
        }
    }
}
