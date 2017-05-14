using System;
using System.Collections.Generic;

using UIKit;

using CocosSharp;
using AdygaPuzzle;
using System.IO;

namespace AdygaPuzzle.iOS
{
    public partial class ViewController : UIViewController, IMainActivity
    {
        Director _director;

        public ViewController(IntPtr handle)
            : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            if (GameView != null)
            {
                // Set loading event to be called once game view is fully initialised
                GameView.ViewCreated += LoadGame;
            }
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            if (GameView != null)
                GameView.Paused = true;
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            if (GameView != null)
                GameView.Paused = false;
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
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

        public void LogInfo(string line)
        {
            
        }

        public Stream OpenAsset(string file)
        {
            return System.IO.File.OpenRead(file);
        }
    }    
}

