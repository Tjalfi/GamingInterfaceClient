namespace GamingInterfaceClient.Renderer
{
    public interface IToast
    {
        void LongAlert(string message);
        void ShortAlert(string message);
    }
}
