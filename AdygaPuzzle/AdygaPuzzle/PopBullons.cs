using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CocosSharp;

namespace AdygaPuzzle
{
    class MoovingBalloon
    {
        public bool IsTouchingBalloon(CCTouch touch)
        {
            if (balloon == null)
                return false;
            // This includes the rectangular white space around our sprite
            return node.Position.IsNear(touch.Location, balloon.BoundingBox.Size.Width / 2);
        }

        public void Pop()
        {
            if (balloon == null)
                return;
            node.RemoveChild(balloon);
            balloon = null;
            CCAudioEngine.SharedEngine.PlayEffect(filename: "balloon_pop");
            node.StopAllActions();
            var easefall = new CCEaseIn(new CCMoveTo(2, new CCPoint(node.PositionX, -300)), 1.5f);
            node.RunAction(easefall);
            var rotate = new CCRepeatForever(new CCRotateBy(2, 360f));
            symbol.RunAction(rotate);
            balloon = null;
        }

        public CCSprite balloon;
        public CCSprite symbol;
        public CCNode node;
    }

    public class PopBalloon : CCLayer
    {

        Director _activity;
        List<MoovingBalloon> _moovings = new List<MoovingBalloon>();

        public PopBalloon(Director director) : base()
        {
            _activity = director;
        }


        protected override void AddedToScene()
        {
            base.AddedToScene();
            try
            {
                StartBalloons();
            }
            catch(Exception ex)
            {
                _activity.LogInfo("PopBalloon::AddedToScene error: " + ex.ToString());
            }
        }

        void StartBalloons()
        {
            const int MAX_BALLON_SHELL = 7;
            const int MAX_BALLON_SYMBOL = 15;
            var bounds = VisibleBoundsWorldspace;
            for(int i = 0; i < 30; ++i)
            {
                var entry = new MoovingBalloon();
                entry.balloon = new CCSprite("Ex/" + (i % MAX_BALLON_SHELL + 1).ToString() + ".png");
                entry.symbol = new CCSprite("In/" + (i % MAX_BALLON_SYMBOL + 1).ToString() + ".png");
                entry.node = new CCNode();

                entry.node.AddChild(entry.balloon);
                entry.node.AddChild(entry.symbol);
                entry.node.PositionX =  _activity.Rand.Next(50, (int)bounds.MaxX - 50);
                entry.node.PositionY = -1 * _activity.Rand.Next(200, 300);
                entry.node.Scale = _activity.Rand.Next(5, 7) * 0.1f;
                AddChild(entry.node);
                var easeMove = new CCEaseElastic(new CCMoveTo(_activity.Rand.Next(5,15), new CCPoint(entry.node.PositionX, 700)));
                entry.node.RunAction(easeMove);
                _moovings.Add(entry);
            }

            // Register for touch events
            var touchListener = new CCEventListenerTouchAllAtOnce();
            //touchListener.OnTouchesMoved = OnTouches;
            touchListener.OnTouchesBegan = OnTouches;
            AddEventListener(touchListener, this);
        }

        void OnTouches(List<CCTouch> touches, CCEvent touchEvent)
        {
            foreach(var t in touches)
            {
                foreach (var b in Enumerable.Reverse(_moovings))
                {
                    if (b.IsTouchingBalloon(t))
                    {
                        b.Pop();
                        break;
                    }
                }
            }
        }
    }
}
