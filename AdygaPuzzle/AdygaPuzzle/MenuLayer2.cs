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
        //"cock", "cow", "donkey", "duck", "goat", "goose", "lamb", "rabbit", "turkey"
        List<string> _animals;
        List<CCPoint> _menuPositions = new List<CCPoint>();

        Dictionary<string, CCSprite> _sprites = new Dictionary<string, CCSprite>();

        public MenuLayer(Director activity) : base(CCColor4B.Gray)
        {
            _activity = activity;
            _animals = new List<string>(new string[] { "cock", "cow", "donkey", "duck", "goat", "goose", "lamb", "rabbit", "turkey" });
            for(int i = 0; i < 4; i++)
            {
                for (int j=0; j < 2; j++)
                {
                    var x = 100 + i * 200 + 75;
                    var y = 540 - 150 - j * 140 - 50;
                    _menuPositions.Add(new CCPoint(x, y));
                }
            }
        }


        protected override void AddedToScene()
        {
            base.AddedToScene();
            try
            {
                _sprites.Clear();
                for (int i = 0; i < _menuPositions.Count; ++i)
                {
                    var position = i;
                    if (position >= _animals.Count)
                        break;
                    var name = _animals[position];
                    var sprite = new CCSprite(name + ".png");
                    sprite.Position = _menuPositions[i];
                    _sprites[name] = sprite;
                    _activity.LogInfo(string.Format("Drawing sprite {0} position x={1} y={2} ", _animals[position], sprite.PositionX, sprite.PositionY));
                    AddChild(sprite);
                }

                var touchListener = new CCEventListenerTouchAllAtOnce();
                touchListener.OnTouchesBegan = OnTouchesBegan;
                AddEventListener(touchListener, this);
            }
            catch (Exception ex)
            {
                _activity.LogInfo(string.Format("Error adding to scene {0}", ex));
            }
        }

        void OnTouchesBegan(List<CCTouch> touches, CCEvent touchEvent)
        {
            // We only care about the first touch:
            var touch = touches[0];
            foreach (var p in _sprites)
            {
                if (isTouchingPeace(touch, p.Value))
                {
                    _activity.RunGame(p.Key);
                }
            }

        }
        bool isTouchingPeace(CCTouch touch, CCSprite peace)
        {
            // This includes the rectangular white space around our sprite
            return peace.BoundingBox.ContainsPoint(touch.Location);
        }
    }
}
