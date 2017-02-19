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
    public class MenuLayer : CCLayerColor
    {
        Director _activity;
        List<string> _animals;
        List<CCPoint> _menuPositions = new List<CCPoint>();

        Dictionary<string, CCSprite> _sprites = new Dictionary<string, CCSprite>();
        CCSprite _menuLeft;
        CCSprite _menuRight;
        List<CCSprite> _pageBalls = new List<CCSprite>();
        int _currentPage = 0;

        public MenuLayer(Director activity) : base(CCColor4B.Gray)
        {
            _activity = activity;

           //_animals = new List<string>(new string[] {"cat", "chicken", "cock", "cow", "dog", "donkey", "duck", "goat", "goose", "horse", "lamb", "rabbit", "turkey" });
            _animals = new List<string>(new string[] { "bear", "camel", "deer", "elephant", "fox", "hedgehog", "lion", "squirrel", "tiger", "wolf" });
            for (int i = 0; i < 4; i++)
            {
                for (int j=0; j < 2; j++)
                {
                    var x = 150 + i * 230;
                    var y = 540 - 130 - j * 180 - 50;
                    _menuPositions.Add(new CCPoint(x, y));
                }
            }
        }


        void buildMenu()
        {
            _menuLeft = new CCSprite("left");
            _menuRight = new CCSprite("right");
            var menuY = _menuLeft.ContentSize.Height / 2 + 20;
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
                var name = _animals[position];
                var sprite = new CCSprite(name + ".png");
                sprite.Position = _menuPositions[i];
                _sprites[name] = sprite;
                _activity.LogInfo(string.Format("Drawing sprite {0} position x={1} y={2} ", _animals[position], sprite.PositionX, sprite.PositionY));
                AddChild(sprite);
            }
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
                var caption = new CCSprite("home_menu_caption");
                caption.PositionX = bounds.Center.X;
                caption.PositionY = bounds.MaxY - 30 - caption.ContentSize.Height / 2;
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
                    _activity.RunGame(p.Key);
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
        }

        void OnTouchesEnd(List<CCTouch> touches, CCEvent touchEvent)
        {
            // We only care about the first touch:
            var touch = touches[0];
            var diff = touch.Location.X - touch.StartLocation.X;
            const int MIN_DRAG_WIDTH = 100;
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
            // This includes the rectangular white space around our sprite
            return peace.BoundingBox.ContainsPoint(touch.Location);
        }
    }
}
