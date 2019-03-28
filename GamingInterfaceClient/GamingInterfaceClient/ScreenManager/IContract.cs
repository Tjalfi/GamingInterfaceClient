using System;
using System.Collections.Generic;

namespace GamingInterfaceClient.ScreenManager.IContract
{
    public interface IView
    {
        /**
         * Our linkage back to the other half of the contract
         *
         * @param listener Action Listener
         */
        void SetViewActionListener(IViewActionListener listener);

        /**
         * Display an error message
         *
         * @param error which string to display
         */
        void ShowError(string error);

        /**
         * What message to display to the user
         *
         * @param message which string to display
         */
        void ShowMessage(string message);

        /**
         * Displays / hides loading window
         *
         * @param show true to show, false to hide
         */
        void SetProgressIndicator(bool show);

        void UpdateSpinner(Dictionary<int, string> screenList);

        void SetSpinnerSelection(int screenId);
    }

    public interface IViewActionListener
    {
        void Load();
        void Update(int screenId, string newName);
        void Delete(int toDelete);
        void ImportNew(string toImport);
        void ExportCurrent(int screenId);
        void Create();
    }
}
