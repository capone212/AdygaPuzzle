using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CocosSharp;

namespace AdygaPuzzle
{
    public interface IMainActivity
    {
        void LogInfo(string line);
        Stream OpenAsset(string file);
    }

    public class Director 
    {
        IMainActivity _parent;

        CCGameView _gameView = null;
        CCScene _gameScene = null;
        CCScene _menuScene = null;
        GameLayer _gameLayer = null;

        public Director(IMainActivity parent, CCGameView gameView)
        {
            _parent = parent;
            _gameView = gameView;
            Rand = new Random(Guid.NewGuid().GetHashCode());
            PlayBackgroundMusic();
        }

        public Random Rand
        {
            get; private set;
        }

        public void RunGame(string animal)
        {
            if (_gameScene != null)
                _gameScene.Dispose();

            if (_gameLayer != null)
                _gameLayer.Dispose();
            
            _gameScene = new CCScene(_gameView);
            _gameLayer = new GameLayer(this, animal);
            _gameScene.AddLayer(_gameLayer);
            _gameScene.AddLayer(new MenuLayer2(this));
            _gameLayer.StartGame();
            _gameView.RunWithScene(_gameScene);
        }

        public void RunMenu()
        {
            if (_menuScene == null)
            {
                _menuScene = new CCScene(_gameView);
                _menuScene.AddLayer(new MenuLayer(this));
            }
            _gameView.RunWithScene(_menuScene);
        }

        void PlayBackgroundMusic()
        {
            try
            {
                CCAudioEngine.SharedEngine.BackgroundMusicVolume = 0.08f;
                CCAudioEngine.SharedEngine.EffectsVolume = 1f;
                CCAudioEngine.SharedEngine.PlayBackgroundMusic(filename: "background_theme", loop: true);
            }
            catch (Exception ex)
            {
                LogInfo(string.Format("[ERROR] can't play background music {0}", ex));
            }
        }


        public Director(IMainActivity parent)
        {
            _parent = parent;
        }

        public void LogInfo(string msg)
        {
            _parent.LogInfo(msg);
        }
        public Stream OpenAsset(string file)
        {
            return _parent.OpenAsset(file);
        }
    }
}
