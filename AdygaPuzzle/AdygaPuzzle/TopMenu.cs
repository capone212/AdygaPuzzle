using System;
using System.Collections.Generic;
using System.Linq;
using CocosSharp;
using Microsoft.Xna.Framework;
using System.IO;

namespace AdygaPuzzle
{
    
    class TopMenuItem
    {
        public string Name { set; get; }
        public CCSprite Sprite {set;  get; }
    }

    public class TopMenu : CCLayerColor
    {
        List<TopMenuItem> _allSprites = new List<TopMenuItem>();
        Director _parent;

        public TopMenu(Director parent) : base(new CCColor4B(227, 79, 62))
        {
            _parent = parent;
        }

        void addLabel(string txt, float x, float y)
        {
            var label = new CCLabel(txt, "Gagalin-36", 36, CCLabelFormat.SpriteFont);
            label.PositionX = x;
            label.PositionY = y;
            label.Color = CCColor3B.Black;
            label.Scale = 0.7f;
            AddChild(label);
        }

        protected override void AddedToScene()
        {
            base.AddedToScene();       
            var bounds = VisibleBoundsWorldspace;

            image currentImage = null;
            using (var stackXml = _parent.OpenAsset("Content/Images/Animals/topmenu/stack.xml"))
            {
                currentImage = ParseHelpers.ParseXML<image>(stackXml);
            }
           
            foreach (var peace in currentImage.stack)
            {
                string prefix = "topmenu";
                var spite = new CCSprite(prefix + "/" + peace.src);
                spite.PositionX = (spite.ContentSize.Width) / 2 + peace.x;
                spite.PositionY = currentImage.h - peace.y - (spite.ContentSize.Height -1) / 2;
                AddChild(spite);
                int n;
                if (!int.TryParse(peace.name, out n))
                {
                    _allSprites.Add(new TopMenuItem { Name = peace.name, Sprite = spite });
                }
            }

            addLabel("УНАГЪУЭ ПСЭУЩХЬЭХЭР", 250, 60);
            addLabel("КЪУАЛЭ БЗУХЭР", 700, 40);
            addLabel("ХЬЭКIЭКХЪУЭКIЭХЭР", bounds.Center.X, 510);

            // Register for touch events
            var touchListener = new CCEventListenerTouchAllAtOnce();
            touchListener.OnTouchesBegan = OnTouchesBegan;
            AddEventListener(touchListener, this);
        }

        

        void OnTouchesBegan(List<CCTouch> touches, CCEvent touchEvent)
        {
            // We only care about the first touch:
            var touch = touches[0];
            foreach (var p in _allSprites)
            {
                if (isTouchingPeace(touch, p.Sprite))
                {
                    _parent.RunMenu(p.Name);
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

