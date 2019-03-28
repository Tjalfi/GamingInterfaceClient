using System;
using Xamarin.Forms;

[assembly: Dependency(typeof(GamingInterfaceClient.Utils.iOS.CommonUtils))]
namespace GamingInterfaceClient.Utils.iOS
{
    class CommonUtils : ICommonUtils
    {
        public string GetDataDir()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        }
    }
}