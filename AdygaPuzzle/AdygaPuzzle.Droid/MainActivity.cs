using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;

using CocosSharp;
using AdygaPuzzle;
using System.IO;


namespace AdygaPuzzle.Droid
{
    [Activity(Label = "Nita Puzzle", MainLauncher = true, Icon = "@drawable/icon",
        AlwaysRetainTaskState = true,
        ScreenOrientation = ScreenOrientation.Landscape,
        LaunchMode = LaunchMode.SingleInstance,
        Theme = "@android:style/Theme.Black.NoTitleBar.Fullscreen",
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden)]
    public class MainActivity : Activity, IMainActivity
    {
        Director _director;

        public void LogInfo(string line)
        {
            Log.Info("adyga_puzzle", line);
        }

        public Stream OpenAsset(string file)
        {
            return Assets.Open(file);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our game view from the layout resource,
            // and attach the view created event to it
            CCGameView gameView = (CCGameView)FindViewById(Resource.Id.GameView);
            gameView.ViewCreated += LoadGame;
        }

        void LoadGame(object sender, EventArgs e)
        {
            CCGameView gameView = sender as CCGameView;

            if (gameView != null)
            {
                var contentSearchPaths = new List<string>() { "Fonts", "Sounds", "Sounds/Animals", "Images/Animals", "Images/Ballons" };
                CCSizeI viewSize = gameView.ViewSize;

                int width = 960;
                int height = 540;

                // Set world dimensions
                gameView.DesignResolution = new CCSizeI(width, height);
                gameView.ResolutionPolicy = CCViewResolutionPolicy.ShowAll;

                // Determine whether to use the high or low def versions of our images
                // Make sure the default texel to content size ratio is set correctly
                // Of course you're free to have a finer set of image resolutions e.g (ld, hd, super-hd)
                if (width < viewSize.Width)
                {
                    //contentSearchPaths.Add("Images/Hd");
                    //CCSprite.DefaultTexelToContentSizeRatio = 2.0f;
                }
                else
                {
                    //contentSearchPaths.Add("Images/Ld");
                    //CCSprite.DefaultTexelToContentSizeRatio = 1.0f;
                }

                gameView.ContentManager.SearchPaths = contentSearchPaths;

                // Construct game scene
                _director = new Director(this, gameView);
                _director.RunTopMenu();
            }
        }
    }
}

