using Newtonsoft.Json;
using System.Collections.Generic;

namespace GamingInterfaceClient.Models.Screen
{
    public class Screen : IScreen
    {
        [JsonProperty(Order = 0)]
        private int screenId;

        /* @JsonUnwrapped */
        [JsonProperty(Order = 2, PropertyName = "controls")]
        private List<GICControl> customControls = new List<GICControl>();

        [JsonIgnore]
        private string background;

        [JsonProperty(Order = 5, PropertyName = "newControlId")]
        private int newId = 0;

        [JsonProperty(Order = 3)]
        private int backgroundColor;

        [JsonProperty(Order = 4)]
        private string backgroundPath;

        [JsonProperty(Order = 1)]
        private string name;

        
        public Screen(int screenId,
            List<GICControl> customControls,
            int backgroundColor,
            string backgroundPath,
            int newId,
            string name)
        {
            this.screenId = screenId;
            this.customControls = customControls;
            this.backgroundColor = backgroundColor;
            this.backgroundPath = backgroundPath;
            this.name = name;
            this.newId = newId;
        }

        public Screen(int screenId)
        {
            this.screenId = screenId;
        }

        public string GetBackground()
        {
            if (backgroundPath == "" || backgroundPath == null)
            {
                //load a color
                /*
                ColorDrawable color = new ColorDrawable();
                color.setColor(backgroundColor);
                background = color;
                */
            }
            else
            {
                //load an image
                /*
                Bitmap bitmap = BitmapFactory.decodeFile(backgroundPath);
                if (bitmap == null)
                {
                    background = new ColorDrawable(Color.BLACK);
                }
                else
                {
                    Drawable bitmapDrawable = new BitmapDrawable(context.getResources(), bitmap);
                    background = bitmapDrawable;
                }
                */
            }

            return background;
        }

        public void SetBackground(string background)
        {
            if (background != null)
                this.background = background;
        }

        public void SetBackgroundColor(int backgroundColor)
        {
            this.backgroundColor = backgroundColor;
        }

        public int GetBackgroundColor()
        {
            return backgroundColor;
        }

        public void SetBackgroundFile(string backgroundPath)
        {
            this.backgroundPath = backgroundPath;
        }

        public string GetBackgroundFile()
        {
            return backgroundPath;
        }

        /*
        public Drawable GetImage(string fileName)
        {
            //        if (fileName.contains(screenId + "_control")) {
            Bitmap bitmap = BitmapFactory.decodeFile(fileName);
            Drawable bitmapDrawable = new BitmapDrawable(context.getResources(), bitmap);
            return bitmapDrawable;
            //        }
            //return null;
        }
        */

        public void AddControl(GICControl control)
        {
            customControls.Add(control);
        }

        public int GetNewControlId()
        {
            newId++;
            return newId - 1;
        }

        public List<GICControl> GetControls()
        {
            return customControls;
        }

        public string GetName()
        {
            if (name == null)
                return "Screen " + GetScreenId();
            return name;
        }

        public void SetName(string newName)
        {
            name = newName;
        }

        public int GetScreenId()
        {
            return screenId;
        }

        public void SetScreenId(int newId)
        {
            screenId = newId;
        }

        public void RemoveControl(GICControl control)
        {
            customControls.Remove(control);
        }
    }
}
