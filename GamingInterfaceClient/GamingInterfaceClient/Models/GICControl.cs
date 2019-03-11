using Newtonsoft.Json;
using Xamarin.Forms;

namespace GamingInterfaceClient.Models
{
    public class GICControl
    {
        //this is required for the toggle button, there are 4 stages to track:
        //0 - switched off ready for mouse down
        //1 - switched off, ready for mouse up
        //2 - switched on, ready for mouse down,
        //3 - switched on, ready for mouse up
        //after 3, we reset back to 0
        public int stage = 0;

        [JsonProperty(Order = 0)]
        private Command command = new Command();

        [JsonProperty(Order = 1)]
        private string text = "NONE";

        [JsonProperty(Order = 2)]
        private float left = 140;

        [JsonProperty(Order = 3)]
        private int width = 320;

        [JsonProperty(Order = 4)]
        private float top = 200;

        [JsonProperty(Order = 5)]
        private int height = 240;

        [JsonProperty(Order = 6)]
        private Color fontColor = Color.White;

        [JsonProperty(Order = 7)]
        private int primaryColor = -1;

        [JsonProperty(Order = 8)]
        private int secondaryColor = -1;

        [JsonProperty(Order = 9)]
        private int fontSize = 36;

        [JsonProperty(Order = 10)]
        private int viewType = 0;

        [JsonProperty(Order = 11)]
        private string primaryImageResource = "button_neon.jpg";

        [JsonProperty(Order = 12)]
        private string secondaryImageResource = "button_neon_pushed.jpg";

        [JsonProperty(Order = 13)]
        private string primaryImage = "";

        [JsonProperty(Order = 14)]
        private string secondaryImage = "";

        [JsonProperty(Order = 16)]
        private string fontName = "";

        [JsonProperty(Order = 17)]
        private int fontType = 0;

        [JsonProperty(Order = 15)]
        private Command commandSecondary = new Command();

        public Command GetCommand()
        {
            return command;
        }

        public void SetCommand(Command command)
        {
            this.command = command;
        }

        public void SetText(string text)
        {
            this.text = text;
        }

        public string GetText()
        {
            return text;

        }

        public void SetLeft(float left)
        {
            this.left = left;
        }

        public float GetLeft()
        {
            return left;

        }

        public void SetWidth(int width)
        {
            this.width = width;
        }

        public int GetWidth()
        {
            return width;

        }

        public void SetTop(float top)
        {
            this.top = top;
        }

        public float GetTop()
        {
            return top;

        }

        public void SetHeight(int height)
        {
            this.height = height;
        }

        public int GetHeight()
        {
            return height;
        }

        public Color GetFontColor()
        {
            return fontColor;
        }

        public void SetFontColor(Color fontColor)
        {
            this.fontColor = fontColor;
        }

        public int GetPrimaryColor()
        {
            return primaryColor;
        }

        public void SetPrimaryColor(int primaryColor)
        {
            this.primaryColor = primaryColor;
        }

        public int GetSecondaryColor()
        {
            return secondaryColor;
        }

        public void SetSecondaryColor(int secondaryColor)
        {
            this.secondaryColor = secondaryColor;
        }

        public int GetFontSize()
        {
            return fontSize;
        }

        public void SetFontSize(int fontSize)
        {
            this.fontSize = fontSize;
        }

        public int GetViewType()
        {
            return viewType;
        }

        public void SetViewType(int viewType)
        {
            this.viewType = viewType;
        }

        public string GetPrimaryImageResource()
        {
            return primaryImageResource;
        }

        public void SetPrimaryImageResource(string primaryImageResource)
        {
            this.primaryImageResource = primaryImageResource;
        }

        public string GetSecondaryImageResource()
        {
            return secondaryImageResource;
        }

        public void SetSecondaryImageResource(string secondaryImageResource)
        {
            this.secondaryImageResource = secondaryImageResource;
        }

        public string GetPrimaryImage()
        {
            return primaryImage;
        }

        public void SetPrimaryImage(string primaryImage)
        {
            this.primaryImage = primaryImage;
        }

        public string GetSecondaryImage()
        {
            return secondaryImage;
        }

        public void SetSecondaryImage(string secondaryImage)
        {
            this.secondaryImage = secondaryImage;
        }

        public string GetFontName()
        {
            return fontName;
        }

        public void SetFontName(string fontName)
        {
            this.fontName = fontName;
        }

        public int GetFontType()
        {
            return fontType;
        }

        public void SetFontType(int fontType)
        {
            this.fontType = fontType;
        }

        public Command GetCommandSecondary()
        {
            return commandSecondary;
        }

        public void SetCommandSecondary(Command commandSecondary)
        {
            this.commandSecondary = commandSecondary;
        }
    }
}
