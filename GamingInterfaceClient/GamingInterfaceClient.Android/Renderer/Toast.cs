using Android.App;
using Android.Widget;

[assembly: Xamarin.Forms.Dependency(typeof(GamingInterfaceClient.Renderer.Droid.Toast))]
namespace GamingInterfaceClient.Renderer.Droid
{
    class Toast : IToast
    {
        public void LongAlert(string message)
        {
            Android.Widget.Toast.MakeText(Application.Context, message, ToastLength.Long).Show();
        }

        public void ShortAlert(string message)
        {
            Android.Widget.Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
        }
    }
}