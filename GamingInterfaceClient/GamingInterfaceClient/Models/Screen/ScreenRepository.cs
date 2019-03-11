using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace GamingInterfaceClient.Models.Screen
{
    using Extensions;

    public class ScreenRepository : IScreenRepository
    {
        private List<IScreen> cache;
        private readonly string PREFS_SCREEN = "screen_";
        private readonly string PREFS_BACKGROUND_SUFFIX = "_background";
        private readonly string PREFS_BACKGROUND_PATH_SUFFIX = "_background_path";
        private readonly string PREFS_CONTROLS = "_control_";
        private readonly string lastIntPattern = "[^0-9]+([0-9]+)$";

        public void LoadScreens(LoadCallback callback)
        {
            LoadScreens();
            callback(cache);
        }

        public void LoadScreens()
        {
            if (cache == null)
            {
                //init the cache
                cache = new List<IScreen>();

                int screenId = 0;
                foreach (KeyValuePair<string, object> pair in Application.Current.Properties)
                {
                    if (pair.Key.Contains(PREFS_SCREEN))
                    {
                        Match match = Regex.Match(pair.Key, lastIntPattern);
                        if (match.Success)
                        {
                            string someNumberStr = match.Groups[1].Value;
                            if (!int.TryParse(someNumberStr, out screenId))
                            {
                                screenId = 0;
                            }
                        }

                        Screen screen = new Screen(screenId);
                        try
                        {
                            //legacy used an integer dummy value, so we need to handle that
                            screen.SetName((string)pair.Value);
                        }
                        catch (Exception e)
                        {
                            screen.SetName("Screen " + screenId);
                        }
                        cache.Add(screen);
                        LoadBackground(screen);
                        LoadControls(screen);
                    }
                }
                if (cache.Count == 0)
                {
                    //load in the legacy
                    cache.Add(BuildInitialScreen());
                }
            }
        }

        //this handles both legacy (1.x) and new builds
        private Screen BuildInitialScreen()
        {
            Screen screen = new Screen(0);

            Application.Current.Properties.AddOrSet(PREFS_SCREEN + screen.GetScreenId(), 1);
            Application.Current.SavePropertiesAsync();

            LoadBackground(screen);
            LoadControls(screen);
            return screen;
        }

        private void LoadControls(Screen screen)
        {
            ConvertLegacyControls(screen);

            foreach (KeyValuePair<string, object> pair in Application.Current.Properties)
            {
                if (pair.Key.Contains(screen.GetScreenId() + PREFS_CONTROLS))
                {
                    try
                    {
                        screen.AddControl(JsonConvert.DeserializeObject<GICControl>((string)pair.Value));
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.StackTrace);
                    }
                }
            }
        }

        private void LoadBackground(Screen screen)
        {
            ConvertLegacyBackground(screen);

            int backgroundColor = -1; //context.getResources().getColor(R.color.default_background)
            if (Application.Current.Properties.ContainsKey(screen.GetScreenId() + PREFS_BACKGROUND_SUFFIX))
            {
                backgroundColor = (int)Application.Current.Properties[screen.GetScreenId() + PREFS_BACKGROUND_SUFFIX];
            }

            string backgroundPath = "";
            if (Application.Current.Properties.ContainsKey(screen.GetScreenId() + PREFS_BACKGROUND_PATH_SUFFIX))
            {
                backgroundPath = (string)Application.Current.Properties[screen.GetScreenId() + PREFS_BACKGROUND_PATH_SUFFIX];
            }

            screen.SetBackgroundColor(backgroundColor);
            screen.SetBackgroundFile(backgroundPath);
        }

        private void ConvertLegacyControls(Screen screen)
        {
            foreach (KeyValuePair<string, object> pair in Application.Current.Properties)
            {
                if (pair.Key.StartsWith("control_"))
                {
                    Application.Current.Properties.Remove(pair.Key);
                    if (pair.Value is int || pair.Value is string)
                    {
                        Application.Current.Properties.AddOrSet(screen.GetScreenId() + "_" + pair.Key, pair.Value);
                    }
                    else
                    {
                        Debug.WriteLine("convertLegacyControls: unknown type of pref " + pair.Key + ": " + pair.Value.ToString());
                    }
                }
            }

            Application.Current.SavePropertiesAsync();
        }

        private void ConvertLegacyBackground(Screen screen)
        {
            if (Application.Current.Properties.ContainsKey("background"))
            {
                int backgroundColor = -1;
                if (Application.Current.Properties["background"] is int)
                {
                    backgroundColor = (int)Application.Current.Properties["background"];
                }
                else if (Application.Current.Properties["background"] is string)
                {
                    if (!int.TryParse((string)Application.Current.Properties["background"], out backgroundColor))
                    {
                        backgroundColor = -1;
                    }
                }
                Application.Current.Properties.AddOrSet(screen.GetScreenId() + "_background", backgroundColor);
                Application.Current.Properties.Remove("background");
                Application.Current.SavePropertiesAsync();
            }
            /*
            string backgroundPath = "background" + ".jpg";
            File file = new File(context.getFilesDir() + "/" + backgroundPath);
            if (file.exists())
            {
                string newBackgroundPath = screen.getScreenId() + "_background.png";
                File newFile = new File(context.getFilesDir() + "/" + newBackgroundPath);
                file.renameTo(newFile);
            }
            */
        }

        public Screen NewScreen()
        {
            Screen newScreen = new Screen(GetUniqueId(cache.Count));
            newScreen.SetBackgroundColor(-1); //context.getResources().getColor(R.color.default_background)
            cache = null; //invalidate the cache
                          //cache.Add(newScreen);

            Save(newScreen);
            LoadScreens();
            return newScreen;
        }


        public void ImportScreen(IScreen screen)
        {
            LoadScreens(new LoadCallback((List<IScreen> screens) =>
            {
                GetUniqueName(screen);
                Screen newScreen = (Screen)screen;
                newScreen.SetScreenId(GetUniqueId(cache.Count));
                cache = null; //invalidate the cache
                              //cache.add(newScreen);
                Save(newScreen);
            }));
        }

        private int GetUniqueId(int startingId)
        {
            int unique = startingId;

            if (cache != null)
            {
                foreach (IScreen screen in cache)
                {
                    if (unique == screen.GetScreenId())
                    {
                        unique = GetUniqueId(startingId + 1);
                    }
                }
            }
            return unique;
        }

        private void GetUniqueName(IScreen newScreen)
        {
            foreach (IScreen screen in cache)
            {
                if (screen.GetName().Equals(newScreen.GetName()))
                {
                    newScreen.SetName(newScreen.GetName() + "1");
                    GetUniqueName(newScreen);
                }
            }
        }



        public void Save(IScreen screen)
        {
            Application.Current.Properties.AddOrSet(PREFS_SCREEN + screen.GetScreenId(), screen.GetName());

            /*
            //save the background image
            if (screen.GetBackground() != null)
            {
                if (screen.GetBackground() is ColorDrawable)
                {
                    ColorDrawable color = (ColorDrawable)screen.GetBackground();
            */
            Application.Current.Properties.AddOrSet(screen.GetScreenId() + PREFS_BACKGROUND_SUFFIX, screen.GetBackgroundColor());
            /*
                }
                else
                {
                    Application.Current.Properties.AddOrSet(screen.GetScreenId() + PREFS_BACKGROUND_SUFFIX, -1);
                }
            */
            Application.Current.Properties.AddOrSet(screen.GetScreenId() + PREFS_BACKGROUND_PATH_SUFFIX, screen.GetBackgroundFile());
            /*
                }
                else
                {
                    screen.SetBackgroundFile(context.getFilesDir() + "/" + screen.GetScreenId() + PREFS_BACKGROUND_SUFFIX + ".png");
                    SaveBitmap(screen.GetScreenId() + PREFS_BACKGROUND_SUFFIX, ((BitmapDrawable)image).getBitmap());
                    Application.Current.Properties.AddOrSet(screen.GetScreenId() + PREFS_BACKGROUND_SUFFIX, -1);
                    Application.Current.Properties.AddOrSet(screen.GetScreenId() + PREFS_BACKGROUND_PATH_SUFFIX, screen.GetBackgroundFile());
                }
            }
            */

            //first we need to remove all existing views
            foreach (KeyValuePair<string, object> pair in Application.Current.Properties)
            {
                if (pair.Key.Contains(screen.GetScreenId() + PREFS_CONTROLS))
                {
                    Application.Current.Properties.Remove(pair.Key);
                }
            }

            try
            {
                int i = 0;
                foreach (GICControl control in screen.GetControls())
                {
                    string json = JsonConvert.SerializeObject(control);
                    Application.Current.Properties.AddOrSet(screen.GetScreenId() + PREFS_CONTROLS + i, json);
                    i++;
                }
                Application.Current.SavePropertiesAsync();
                //Toast.makeText(context, R.string.edit_activity_saved, Toast.LENGTH_SHORT).show();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                //Toast.makeText(context, e.getLocalizedMessage(), Toast.LENGTH_LONG).show();
            }
        }


        public IScreen GetScreen(int id)
        {
            for (int i = 0; i < cache.Count; i++)
            {
                if (cache[i].GetScreenId() == id)
                    return cache[i];
            }
            return null;
        }

        //this is used by the main screen
        //    
        //    public IScreen GetScreenByPosition(int index) {
        //        return cache[index];
        //    }


        public void GetScreenList(LoadScreenListCallback callback)
        {

            if (cache != null)
            {
                Dictionary<int, string> rv = new Dictionary<int, string>();
                foreach (IScreen screen in cache)
                {
                    rv.AddOrSet(screen.GetScreenId(), screen.GetName());
                }
                callback(rv);
            }
            else
            {
                LoadScreens(new LoadCallback((List<IScreen> screens) =>
                {
                    Dictionary<int, string> rv = new Dictionary<int, string>();
                    foreach (IScreen screen in cache)
                    {
                        rv.AddOrSet(screen.GetScreenId(), screen.GetName());
                    }
                    callback(rv);
                }));
            }
        }


        public void RemoveScreen(int id)
        {
            for (int i = 0; i < cache.Count; i++)
            {
                if (cache[i].GetScreenId() == id)
                    DeleteScreen(cache[i]);
            }
            cache = null;
            LoadScreens();
        }

        private void DeleteScreen(IScreen screen)
        {
            Application.Current.Properties.Remove(PREFS_SCREEN + screen.GetScreenId());
            Application.Current.Properties.Remove(screen.GetScreenId() + PREFS_BACKGROUND_SUFFIX);

            //first we need to remove all existing views
            foreach (KeyValuePair<string, object> pair in Application.Current.Properties)
            {
                if (pair.Key.Contains(screen.GetScreenId() + PREFS_CONTROLS))
                {
                    Application.Current.Properties.Remove(pair.Key);
                }
            }
            Application.Current.SavePropertiesAsync();
        }

        /*
        private void SaveBitmap(string fileName, Bitmap image)
        {
            File file = new File(context.getFilesDir(), fileName + ".png");

            try
            {
                FileOutputStream fout = new FileOutputStream(file);
                image.compress(Bitmap.CompressFormat.PNG, 90, fout);
                fout.flush();
                fout.close();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
            }
        }
        */
    }
}
