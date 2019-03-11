using System.Collections.Generic;

namespace GamingInterfaceClient.Models.Screen
{
    public interface IScreen
    {
        string GetName();
        void SetName(string newName);
        int GetScreenId();
        string GetBackgroundFile();
        /* Drawable GetImage(string filename);*/
        void AddControl(GICControl control);
        int GetNewControlId();
        List<GICControl> GetControls();
        void SetScreenId(int newId);
        void RemoveControl(GICControl control);
        string GetBackground();
        void SetBackground(string background);
        void SetBackgroundColor(int backgroundColor);
        int GetBackgroundColor();
        void SetBackgroundFile(string backgroundPath);
    }
}
