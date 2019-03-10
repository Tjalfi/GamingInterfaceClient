using System;
using Xamarin.Forms;

namespace GamingInterfaceClient.Utils
{
    static class CommonUtils
    {
        public static void OpenBrowser(string url)
        {
            Device.OpenUri(new Uri(url));
        }
    }
}
