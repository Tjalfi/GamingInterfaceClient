using System;
using Xamarin.Forms;

[assembly: Dependency(typeof(GamingInterfaceClient.Utils.Droid.CommonUtils))]
namespace GamingInterfaceClient.Utils.Droid
{
    class CommonUtils : ICommonUtils
    {
        public string GetDataDir()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        }
    }
}