namespace GamingInterfaceClient.Models.Screen
{
    public interface IScreenRepository
    {
        void LoadScreens(LoadCallback callback);
        void LoadScreens();
        Screen NewScreen();
        void ImportScreen(IScreen screen);
        void Save(IScreen screen);
        IScreen GetScreen(int id);
        //this is used by the main screen
        //IScreen GetScreenByPosition(int index);
        void GetScreenList(LoadScreenListCallback callback);
        void RemoveScreen(int id);
    }
}
