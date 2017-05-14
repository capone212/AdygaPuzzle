/****************************************************************************
Copyright (c) 2010-2012 cocos2d-x.org
Copyright (c) 2008-2009 Jason Booth
Copyright (c) 2011-2012 openxlive.com

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CocosSharp;

namespace AdygaPuzzle
{
    public struct AnimalInfo
    {
        public AnimalInfo(string id, string name)
        {
            Id = id;
            DisplayName = name;
        }
        public string Id;
        public string DisplayName;
    }

    public class MenuLayer : CCLayerColor
    {
        Director _activity;
        List<AnimalInfo> _animals;
        List<CCPoint> _menuPositions = new List<CCPoint>();

        Dictionary<AnimalInfo, CCSprite> _sprites = new Dictionary<AnimalInfo, CCSprite>();
        CCSprite _menuLeft;
        CCSprite _menuRight;
        CCSprite _toTopmenu;
        CCSprite _musicOnOff;
        List<CCSprite> _pageBalls = new List<CCSprite>();
        int _currentPage = 0;
        string _displayName;
        float _menuY;

        void initByType(string type)
        {
            if (type == "home")
            {
                _animals = new List<AnimalInfo>(new AnimalInfo[] { new AnimalInfo("cat", "ДЖЭДУ"), new AnimalInfo("chicken", "ДЖЭД"), new AnimalInfo("cock", "АДАКЪЭ"),
                    new AnimalInfo("cow", "ЖЭМ"), new AnimalInfo("dog", "ХЬЭ"), new AnimalInfo("donkey", "ШЫД"), new AnimalInfo("duck", "БАБЫЩ"), new AnimalInfo("goat", "БЖЭН"),
                    new AnimalInfo("goose", "КЪАЗ"), new AnimalInfo("horse", "ШЫ"), new AnimalInfo("lamb", "МЭЛ"), new AnimalInfo("rabbit", "ТХЬЭКIУМЭКIЫХЬ"), new AnimalInfo("turkey", "ГУЭГУШ") });
                _displayName = "УНАГЪУЭ ПСЭУЩХЬЭХЭР";
                return;
            }
            else if (type == "wild")
            {
                _animals = new List<AnimalInfo>(new AnimalInfo[] { new AnimalInfo("bear", "МЫЩЭ"), new AnimalInfo("camel", "МАХЪШЭ"), new AnimalInfo("deer", "ЩЫХЬ"), new AnimalInfo("elephant", "ПЫЛ"),
                    new AnimalInfo("fox", "БАЖЭ"), new AnimalInfo("hedgehog", "ЦЫЖЬБАНЭ"), new AnimalInfo("lion", "АСЛЪЭН"), new AnimalInfo("squirrel", "КIЭПХЪ"), new AnimalInfo("tiger", "КЪАПЛЪЭН"), new AnimalInfo("wolf", "ДЫГЪУЖЬ") });
                _displayName = "ХЬЭКIЭКХЪУЭКIЭХЭР";
                return;
            }

            // birds
            _animals = new List<AnimalInfo>(new AnimalInfo[] { new AnimalInfo("dyatel", "ЖЫГУIУ"), new AnimalInfo("golub", "ТХЬЭРЫКЪУЭ"), new AnimalInfo("lastochka", "ПЦIАЩХЪУЭ"), new AnimalInfo("lebed", "КЪЫУ"), new AnimalInfo("sinica", "ЦЫЖЬДАДЭ"),
                new AnimalInfo("snegir", "БЗУПАГУЭ"), new AnimalInfo("soroka", "КЪАНЖЭ"), new AnimalInfo("sova", "ЖЬЫНДУ"), new AnimalInfo("vorobey", "БЗУ"), new AnimalInfo("vorona", "КЪУАРГЪ"), new AnimalInfo("orel", "БГЪЭ") });
            _displayName = "КЪУАЛЭ БЗУХЭР";
        }

        public MenuLayer(Director activity, string type) : base(CCColor4B.Gray)
        {
            _activity = activity;
            Type = type;

            initByType(type);
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 1; j++)
                {
                    var x = 150 + i * 230;
                    var y = 540 - 200 - j * 180 - 50;
                    _menuPositions.Add(new CCPoint(x, y));
                }
            }
        }

        public string Type { get; set; }

        void buildMenu()
        {
            _menuLeft = new CCSprite("left");
            _menuRight = new CCSprite("right");
            var menuY = _menuLeft.ContentSize.Height / 2 + 20;
            _menuY = menuY;

            if (_toTopmenu == null)
            {
                _toTopmenu = new CCSprite("back");
                _toTopmenu.Position = new CCPoint(50, menuY);
                AddChild(_toTopmenu);
            }

            if (_musicOnOff == null)
            {
                onMusicOnOff();
            }

            int pagesCount = (_animals.Count - 1) / _menuPositions.Count + 1;
            for (int i = 0; i < pagesCount; ++i)
            {
                _pageBalls.Add(new CCSprite("ball"));
            }
            const int MENU_GAP = 20;
            var ballsTotallWidth = pagesCount * _pageBalls.First().ContentSize.Width + MENU_GAP * (pagesCount - 1);
            var transformX = VisibleBoundsWorldspace.Size.Width / 2 - ballsTotallWidth / 2;
            for (int i = 0; i < pagesCount; ++i)
            {
                var spr = _pageBalls[i];
                spr.PositionY = menuY;
                spr.PositionX = transformX + (spr.ContentSize.Width + MENU_GAP) * i ;
                AddChild(spr);
            }

            _menuLeft.PositionX = _pageBalls.First().PositionX - MENU_GAP - _pageBalls.First().ContentSize.Width / 2 - _menuLeft.ContentSize.Width / 2;
            _menuLeft.PositionY = menuY;
            _menuRight.PositionX = _pageBalls.Last().PositionX + MENU_GAP  + _pageBalls.Last().ContentSize.Width / 2 + _menuRight.ContentSize.Width / 2;
            _menuRight.PositionY = menuY;
            AddChild(_menuRight);
            AddChild(_menuLeft);
        }

        void onMusicOnOff()
        {
            if (_musicOnOff != null)
            {
                RemoveChild(_musicOnOff);
                _musicOnOff.Dispose();
            }
            _musicOnOff = new CCSprite(_activity.IsMusisOn ? "sound-on" : "sound-off");
            _musicOnOff.Position = new CCPoint(150, _menuY);
            _musicOnOff.Color = CCColor3B.Blue;
            AddChild(_musicOnOff);
        }

        void RefreshControls()
        {
            for (int i = 0; i < _pageBalls.Count; ++i)
            {
                _pageBalls[i].Color = i != _currentPage ? CCColor3B.DarkGray : CCColor3B.White;
            }
            _menuLeft.Color = _currentPage == 0 ? CCColor3B.Gray : CCColor3B.White;
            _menuRight.Color = _currentPage == _pageBalls.Count - 1 ? CCColor3B.DarkGray : CCColor3B.White;
        }

        void OnPageChanged()
        {
            foreach(var s in _sprites)
            {
                RemoveChild(s.Value);
            }
            FillAnimals();
            RefreshControls();
        }

        void NextPage()
        {
            if (_currentPage >= _pageBalls.Count - 1)
                return;
            _currentPage++;
            OnPageChanged();
        }

        void PrevPage()
        {
            if (_currentPage == 0)
                return;
            _currentPage--;
            OnPageChanged();
        }

        void FillAnimals()
        {
            _sprites.Clear();
            for (int i = 0; i < _menuPositions.Count; ++i)
            {
                var position = _menuPositions.Count * _currentPage + i;
                if (position >= _animals.Count)
                    break;
                var animalInfo = _animals[position];
                var sprite = new CCSprite(animalInfo.Id + ".png");
                sprite.Position = _menuPositions[i];
                _sprites[animalInfo] = sprite;
                _activity.LogInfo(string.Format("Drawing sprite {0} position x={1} y={2} ", _animals[position], sprite.PositionX, sprite.PositionY));
                AddChild(sprite);
            }
        }

        public AnimalInfo GetNextAnimal(AnimalInfo info)
        {
            var index = _animals.IndexOf(info) + 1;
            index = index < _animals.Count ? index : 0;
            return _animals[index];
        }

        protected override void AddedToScene()
        {
            base.AddedToScene();
            try
            {
                _sprites.Clear();
                // TODO: customize
                var bounds = VisibleBoundsWorldspace;
                var background = new CCSprite("home_background_menu");
                background.Position = bounds.Center;
                AddChild(background);

                // TODO: customize home_
                var caption = new CCLabel(_displayName, "Gagalin-36", 36, CCLabelFormat.SpriteFont);
                caption.PositionX = bounds.Center.X;
                caption.PositionY = bounds.MaxY - 30 - caption.ContentSize.Height / 2;
                caption.Color = CCColor3B.Black;
                AddChild(caption);

                buildMenu();
                OnPageChanged();

                var touchListener = new CCEventListenerTouchAllAtOnce();
                touchListener.OnTouchesBegan = OnTouchesBegan;
                touchListener.OnTouchesEnded = OnTouchesEnd;
                AddEventListener(touchListener, this);

            }
            catch (Exception ex)
            {
                _activity.LogInfo(string.Format("Error adding to scene {0}", ex));
            }
        }

        //float dragPosition = 0;

        void OnTouchesBegan(List<CCTouch> touches, CCEvent touchEvent)
        {
            // We only care about the first touch:
            var touch = touches[0];
            foreach (var p in _sprites)
            {
                if (isTouchingPeace(touch, p.Value))
                {
                    _activity.RunGame(Type, p.Key);
                    return;
                }
            }

            if (isTouchingPeace(touch, _menuLeft))
            {
                PrevPage();
                return;
            }

            if (isTouchingPeace(touch, _menuRight))
            {
                NextPage();
                return;
            }

            if (isTouchingPeace(touch, _toTopmenu))
            {
                _activity.RunTopMenu();
                return;
            }

            if (isTouchingPeace(touch, _musicOnOff))
            {
                _activity.IsMusisOn = !_activity.IsMusisOn;
                onMusicOnOff();
            }
            
        }

        void OnTouchesEnd(List<CCTouch> touches, CCEvent touchEvent)
        {
            // We only care about the first touch:
            var touch = touches[0];
            var diff = touch.Location.X - touch.StartLocation.X;
            const int MIN_DRAG_WIDTH = 50;
            if (diff > MIN_DRAG_WIDTH)
            {
                PrevPage();
                return;
            }

            if (diff < -1 * MIN_DRAG_WIDTH)
            {
                NextPage();
                return;
            }
        }


        bool isTouchingPeace(CCTouch touch, CCSprite peace)
        {
            if (peace == null)
                return false;
            // This includes the rectangular white space around our sprite
            return peace.BoundingBox.ContainsPoint(touch.Location);
        }
    }
}
