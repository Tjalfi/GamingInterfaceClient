using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GamingInterfaceClient.ScreenManager
{
    using Models;
    using Models.Screen;
    using System.Diagnostics;
    using Utils;

    /**
     * TODO: HEADER COMMENT HERE.
     */
    public class Presentation : IContract.IViewActionListener
    {
        IContract.IView view;
        IScreenRepository repository;

        /**
         * Public constructor to link up our repository and our view via this presenter
         *
         * @param repository Repository to connect to
         * @param view View to connect to
         */
        public Presentation(IScreenRepository repository, IContract.IView view)
        {
            this.repository = repository ?? throw new ArgumentException("repository cannot be null!");
            this.view = view ?? throw new ArgumentException("detail view cannot be null!");
        }

        public void Load()
        {
            view.SetProgressIndicator(true);
            repository.LoadScreens();
        }

        public void Update(int screenId, string newName)
        {
            view.SetProgressIndicator(true);
            Task.Run(() =>
            {
                IScreen screen = repository.GetScreen(screenId);
                screen.SetName(newName);
                repository.Save(screen);
                repository.GetScreenList(new LoadScreenListCallback((Dictionary<int, string> screenList) =>
                {
                    view.UpdateSpinner(screenList);
                    view.SetSpinnerSelection(screenId);
                    view.SetProgressIndicator(false);
                }));
            });
        }

        public void Delete(int toDelete)
        {
            IContract.IView view = this.view;
            repository.GetScreenList(new LoadScreenListCallback((Dictionary<int, string> screenList) =>
            {
                if (screenList.Count <= 1)
                {
                    view.ShowError(Application.Current.Resources["strCannotDeleteLastItem"].ToString());
                }
                else
                {
                    view.SetProgressIndicator(true);
                    Task.Run(() =>
                    {
                        repository.RemoveScreen(toDelete);
                        view.UpdateSpinner(screenList);
                        view.SetProgressIndicator(false);
                        view.SetSpinnerSelection(0);
                    });
                }
            }));
        }

        public void ImportNew(string toImport)
        {
            view.SetProgressIndicator(true);
            string results = "";
            try
            {
                //get a profile name
                string fullPath = Path.Combine(new string[3] {
                    CommonUtils.GetDataDir(),
                    "GIC-Screens",
                    toImport.Substring(toImport.LastIndexOfAny(new char[2] { '/', '\\' })).Replace("/", "").Replace("\\", "").Replace(".zip", "")
                });

                if (ZipHelper.UnZip(toImport.ToString(), fullPath))
                {
                    //now read the json values
                    string file = File.OpenText(Path.Combine(fullPath, "data.json")).ReadToEnd();
                    Screen screen = JsonConvert.DeserializeObject<Screen>(file);
                    //update any filenames to point to the local folder now
                    string imageFile = screen.GetBackgroundFile();
                    if (imageFile != null && imageFile != "")
                    {
                        screen.SetBackgroundFile(Path.Combine(fullPath, imageFile.Substring(imageFile.LastIndexOfAny(new char[2] { '/', '\\' }) + 1)));
                    }
                    foreach (GICControl control in screen.GetControls())
                    {
                        imageFile = control.GetPrimaryImage();
                        if (imageFile != null && imageFile != "")
                        {
                            control.SetPrimaryImage(Path.Combine(fullPath, imageFile.Substring(imageFile.LastIndexOfAny(new char[2] { '/', '\\' }) + 1)));
                        }
                        imageFile = control.GetSecondaryImage();
                        if (imageFile != null && imageFile != "")
                        {
                            control.SetSecondaryImage(Path.Combine(fullPath, imageFile.Substring(imageFile.LastIndexOfAny(new char[2] { '/', '\\' }) + 1)));
                        }
                    }
                    repository.ImportScreen(screen);
                }
                else
                {
                    results = Application.Current.Resources["strInvalidZip"].ToString();
                }
            }
            catch(Exception e)
            {
                results = e.Message;
            }
            results = Application.Current.Resources["strImportSuccessful"].ToString();
            repository.GetScreenList(new LoadScreenListCallback((Dictionary<int, string> screenList) =>
            {
                view.UpdateSpinner(screenList);
                view.SetProgressIndicator(false);
                view.ShowMessage(results);
            }));
        }

        public void ExportCurrent(int screenId)
        {
            view.SetProgressIndicator(true);
            string results = "";
            string cacheDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            try
            {
                List<string> filesToZip = new List<string>();
                Screen screen = (Screen)repository.GetScreen(screenId);
                string json = JsonConvert.SerializeObject(screen);
                if (Directory.Exists(cacheDir))
                {
                    Directory.Delete(cacheDir, true);
                }
                Directory.CreateDirectory(cacheDir);
                //store the json dat in the directory
                string jsonPath = Path.Combine(cacheDir, "data.json");
                File.WriteAllText(jsonPath, json);
                filesToZip.Add(jsonPath);
                //look for any files inside the screen that we need to add
                if (screen.GetBackgroundFile() != null && screen.GetBackgroundFile() != "")
                {
                    filesToZip.Add(screen.GetBackgroundFile());
                }
                foreach (GICControl control in screen.GetControls())
                {
                    if (control.GetPrimaryImage() != null && control.GetPrimaryImage() != "")
                    {
                        filesToZip.Add(control.GetPrimaryImage());
                    }
                    if (control.GetSecondaryImage() != null && control.GetSecondaryImage() != "")
                    {
                        filesToZip.Add(control.GetSecondaryImage());
                    }
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                results = e.Message;
            }
            finally
            {
                if (Directory.Exists(cacheDir))
                {
                    Directory.Delete(cacheDir, true);
                }
            }
            view.SetProgressIndicator(false);
            view.ShowMessage(results);
        }

        public void Create()
        {
            view.SetProgressIndicator(true);
            Task.Run(() =>
            {
                repository.NewScreen();
                repository.GetScreenList(new LoadScreenListCallback((Dictionary<int, string> screenList) =>
                {
                    view.UpdateSpinner(screenList);
                    view.SetProgressIndicator(false);
                    view.SetSpinnerSelection(screenList.Count - 1);
                }));
            });
        }
    }
}
