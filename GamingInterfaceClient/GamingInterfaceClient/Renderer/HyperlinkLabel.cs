using Xamarin.Forms;

namespace GamingInterfaceClient.Renderer
{
    using Utils;

    public class HyperlinkLabel : Label
    {
        public static readonly BindableProperty UrlProperty = BindableProperty.Create(nameof(Url), typeof(string), typeof(HyperlinkLabel), null);

        public string Url
        {
            get
            {
                string url = (string)GetValue(UrlProperty);
                if (url == "" || url == null)
                {
                    url = Text;
                }
                return url;
            }
            set
            {
                SetValue(UrlProperty, value);
            }
        }

        public HyperlinkLabel()
        {
            TextDecorations = TextDecorations.Underline;
            TextColor = Color.Blue;
            GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => CommonUtils.OpenBrowser(Url))
            });
        }
    }
}
