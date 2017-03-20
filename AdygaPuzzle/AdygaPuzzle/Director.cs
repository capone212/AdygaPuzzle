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
        bool _isMusicOn = true;

        public Director(IMainActivity parent, CCGameView gameView)
        {
            _parent = parent;
            _gameView = gameView;
            _menuScene = new CCScene(_gameView);
            _gameScene = new CCScene(_gameView);
            Rand = new Random(Guid.NewGuid().GetHashCode());
            initBackgroundMusic();
        }

        public Random Rand
        {
            get; private set;
        }

        public void RunNextGame(string type, AnimalInfo animal)
        {
            animal = _menuLayer.GetNextAnimal(animal);
            RunGame(type, animal);
        }

        public void RunGame(string type, AnimalInfo animal)
        {
            try
            {
                _gameScene.RemoveAllChildren();
                var gameLayer = new GameLayer(this, type, animal);
                _gameScene.AddLayer(gameLayer);
                var balloonLayer = new PopBalloon(this);
                _gameScene.AddLayer(balloonLayer);
                gameLayer.StartGame(balloonLayer);
               // CCAudioEngine.SharedEngine.PlayEffect(filename: "home-box-slide");
                _gameView.RunWithScene(_gameScene);
            }
            catch (Exception ex)
            {
                _parent.LogInfo(string.Format("Error starting game! {0}", ex));
            }
        }

        MenuLayer _menuLayer;

        public void RunMenu(string type)
        {
            if (_menuLayer == null || _menuLayer.Type != type)
            {
                if (_menuLayer != null)
                {
                    _menuLayer.RemoveFromParent();
                    _menuLayer.Dispose();
                }
                _menuLayer = new MenuLayer(this, type);
                _menuScene.AddLayer(_menuLayer);
            }
            CCAudioEngine.SharedEngine.PlayEffect(filename: "home-box-slide");
            _gameView.RunWithScene(_menuScene);
        }

        CCScene _topMenuScene = null;

        public void RunTopMenu()
        {
            if (_topMenuScene == null)
            {
                _topMenuScene = new CCScene(_gameView);
                _topMenuScene.AddLayer(new TopMenu(this));
            }
            _gameView.RunWithScene(_topMenuScene);
        }

        
        public bool IsMusisOn
        {
            get
            {
                return _isMusicOn;
            }
            set
            {
                _isMusicOn = value;
                playBackgroundMusic(value);
            }
        }
        
        void initBackgroundMusic()
        {
            CCAudioEngine.SharedEngine.BackgroundMusicVolume = 0.05f;
            CCAudioEngine.SharedEngine.EffectsVolume = 0.9f;
            CCAudioEngine.SharedEngine.PlayBackgroundMusic(filename: "Sounds/background_theme", loop: true);
        }

        void playBackgroundMusic(bool value)
        {
            try
            {
                if (value)
                {
                    CCAudioEngine.SharedEngine.ResumeBackgroundMusic();
                }
                else
                {
                    CCAudioEngine.SharedEngine.PauseBackgroundMusic();
                }
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
