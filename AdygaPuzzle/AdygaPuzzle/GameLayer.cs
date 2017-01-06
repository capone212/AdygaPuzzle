using System;
using System.Collections.Generic;
using System.Linq;
using CocosSharp;
using Microsoft.Xna.Framework;
using System.IO;

//_easings.Add("CCEaseBackOut", a => { return new CCEaseBackOut(a); });

namespace AdygaPuzzle
{
    class Peace
    {
        public Peace(CCSprite s, CCPoint a)
        {
            Sprite = s;
            AssembledPos = a;
        }
        public float HalfWidth
        {
            get { return Sprite.BoundingBox.Size.Width / 2; }
        }

        public float HalfHeigh
        {
            get { return Sprite.BoundingBox.Size.Height / 2; }
        }

        public float DisassembledMaxY
        {
            get { return DisassembledPos.Y + HalfHeigh; }
        }

        public float DisassembledMinY
        {
            get { return DisassembledPos.Y - HalfHeigh; }
        }

        public CCSprite Sprite { get; private set; }
        public CCPoint DisassembledPos { get; set; }
        public CCPoint AssembledPos { get; private set; }
    }

    struct DraggingSpite
    {
        public DraggingSpite(Peace p, CCPoint s)
        {
            Peace = p;
            StartPosition = p.Sprite.Position;
            DragStart = s;
        }

        public Peace Peace;
        public CCPoint StartPosition;
        public CCPoint DragStart;
    }

    public class GameLayer : CCLayerColor
    {
        string _animal;
        Director _parent;
        image _currentImage;
        List<Peace> _peaces = new List<Peace>();
        DraggingSpite? _spiteToDrag = null;
        List<CCSprite> _allSprites = new List<CCSprite>();
        CCSprite _fullPictureSprite;

        public GameLayer(Director parent, string animal) : base(CCColor4B.Blue)
        {
            _parent = parent;
            _animal = animal;
        }

        protected override void AddedToScene()
        {
            base.AddedToScene();

            // Use the bounds to layout the positioning of our drawable assets
            var bounds = VisibleBoundsWorldspace;
            // ----------------------- Start
            _parent.LogInfo("[XML_TEST] writing xml");
            // TODO: open with content manager not with android specific staff.
            using (var stackXml = _parent.OpenAsset("Content/Images/Animals/"+_animal+"/stack.xml"))
            {
                _currentImage = ParseHelpers.ParseXML<image>(stackXml);
            }
            _parent.LogInfo(string.Format("width = {0}, height={1} layers count={2}", _currentImage.w, _currentImage.h, _currentImage.stack.Length));
            _parent.LogInfo(string.Format("VisibleBoundsWorldspace minX={0}, MinY={1}, maxX={2}, maxY={3}", bounds.MinX, bounds.MinY, bounds.MaxX, bounds.MaxY));
            Array.Reverse(_currentImage.stack);
            var background = new CCSprite("background.png");
            background.Position = bounds.Center;
            AddChild(background);
            foreach (var peace in _currentImage.stack)
            {
                string prefix = _animal;
                var spite = new CCSprite(_animal + "/" + peace.src);
                spite.PositionX = (spite.ContentSize.Width) / 2 + peace.x;
                spite.PositionY = _currentImage.h - peace.y - (spite.ContentSize.Height -1) / 2;
                _parent.LogInfo(string.Format("Adding spyte {0} content size X:{1} Y:{2} W:{3} H:{4}", prefix + peace.src, spite.PositionX, spite.PositionY, spite.ContentSize.Width, spite.ContentSize.Height));
                if (peace.name == "!")
                {
                    _fullPictureSprite = spite;
                    continue;
                }
                _allSprites.Add(spite);
                AddChild(spite);
                int n;
                if (int.TryParse(peace.name, out n))
                {
                    _parent.LogInfo(string.Format("Save movable spite with name {0}", peace.name));
                    _peaces.Add(new Peace(spite,  spite.Position));
                }
            }

            AddChild(_fullPictureSprite);

            _peaces.Sort(delegate (Peace x, Peace y)
            {
                return x.Sprite.BoundingBox.Size.Height.CompareTo(y.Sprite.BoundingBox.Size.Height);
            });

            var viewPort = new CCRect(5, 5, bounds.Size.Width - 10, bounds.Size.Height - 10);

            // Calculate exploded location
            _peaces[4].DisassembledPos = new CCPoint(viewPort.MaxX - _peaces[4].HalfWidth, _peaces[4].HalfHeigh + viewPort.MinY);
            _peaces[3].DisassembledPos = new CCPoint(viewPort.MaxX - _peaces[3].HalfWidth, viewPort.MaxY - _peaces[3].HalfHeigh);
            _peaces[2].DisassembledPos = new CCPoint(viewPort.MinX + _peaces[2].HalfWidth, viewPort.MaxY - _peaces[2].HalfHeigh);
            _peaces[1].DisassembledPos = new CCPoint(viewPort.MinX + _peaces[1].HalfWidth, _peaces[1].HalfHeigh + viewPort.MinY);
            _peaces[0].DisassembledPos = new CCPoint(viewPort.MinX + _peaces[0].HalfWidth, _peaces[1].DisassembledMaxY + (_peaces[2].DisassembledMinY - _peaces[1].DisassembledMaxY)/2);

            // --- End

            // Register for touch events
            var touchListener = new CCEventListenerTouchAllAtOnce();
            touchListener.OnTouchesEnded = OnTouchesEnded;
            touchListener.OnTouchesMoved = OnTouchesMoved;
            touchListener.OnTouchesBegan = OnTouchesBegan;
            AddEventListener(touchListener, this);
        }

        void breakToPeaces()
        {
            RemoveChild(_fullPictureSprite);
            CCAudioEngine.SharedEngine.PlayEffect(filename: "break");
            foreach (var p in _peaces)
            {
                DisassemblePease(p);
            }
        }

        void ScheduleAction(Action f, int milliseconds)
        {
            new Timer( x=> { f(); }, null, milliseconds);
        }

        void ScheduleBreak()
        {
            ScheduleAction(breakToPeaces, 3000);
        }

        void Assemble()
        {
            foreach (var p in _peaces)
            {
                p.Sprite.Position = p.AssembledPos;
            }
        }
        void PlayBackgroundMusic()
        {
            try
            {
                CCAudioEngine.SharedEngine.BackgroundMusicVolume = 0.1f;
                CCAudioEngine.SharedEngine.EffectsVolume = 1.0f;
                CCAudioEngine.SharedEngine.PlayBackgroundMusic(filename: "background_theme", loop: true);
            }
            catch (Exception ex)
            {
                _parent.LogInfo(string.Format("[ERROR] can't play background music {0}", ex));
            }
        }

        void Challenge()
        {
            const int MAX_CHALLENGE_COUNT = 2;
            var index = _parent.Rand.Next(1, MAX_CHALLENGE_COUNT + 1);
            CCAudioEngine.SharedEngine.PlayEffect(filename: string.Format("challenge{0}", index));
        }

        public void StartGame()
        {
            Assemble();
            ScheduleBreak();
            Challenge();
        }

        void DisassemblePease(Peace p)
        {
            var easeMove = new CCEaseBackOut(new CCMoveTo(1, p.DisassembledPos));
            p.Sprite.RunAction(easeMove);
        }

        void OnImageAssembled()
        {
            AddChild(_fullPictureSprite);
            foreach(var s in _allSprites)
            {
                RemoveChild(s);
            }
            const int MAX_APPROVE_COUNT = 7;
            var index = _parent.Rand.Next(1, MAX_APPROVE_COUNT + 1);
            CCAudioEngine.SharedEngine.PlayEffect(filename: string.Format("approve{0}", index));
            ScheduleAction(()=>{
                CCAudioEngine.SharedEngine.PlayEffect("happykids");
                ScheduleAction(() =>
                {
                    AnimateCharacter();
                }, 5000);
            },1000);
        }

        void AnimateCharacter()
        {
            var scaleIn = new CCEaseInOut(new CCScaleBy(0.5f, 1.1f), 1.5f);
            var scaleOut = new CCEaseInOut(new CCScaleBy(0.5f, 0.9f), 1.5f);

            // create a delay that is run in between sequence events
            var delay = new CCDelayTime(0.25f);

            // create the sequence of actions, in the order we want to run them
            var animate = new CCSequence(scaleIn, delay, scaleOut);

            _fullPictureSprite.RunAction(animate);
        }

        void StarsFireworks(CCPoint pos)
        {
            try
            {
                var sparks = new CCParticleSystemQuad("sparks.plist");
                sparks.Position = pos;
                AddChild(sparks);
            }
            catch(Exception ex)
            {
                _parent.LogInfo(string.Format("Can't add sparks to scene {0}", ex));
            }
        }

        void AssemblePeace(Peace p)
        {
            CCAudioEngine.SharedEngine.PlayEffect(filename: "success_partial");
            var easeMove = new CCEaseBackOut(new CCMoveTo(0.2f, p.AssembledPos));
            p.Sprite.RunAction(easeMove);
            StarsFireworks(p.AssembledPos);
            var grouped = _peaces.All(s => isPeaceCloseToHome(s));
            if (grouped)
            {
                ScheduleAction(OnImageAssembled, 500);
            }
        }

        void OnTouchesEnded(List<CCTouch> touches, CCEvent touchEvent)
        {
            if (touches.Count > 0)
            {
                if (_spiteToDrag.HasValue)
                {
                    if (!isPeaceCloseToHome(_spiteToDrag.Value.Peace))
                    {
                        DisassemblePease(_spiteToDrag.Value.Peace);
                        CCAudioEngine.SharedEngine.PlayEffect(filename: "fail");
                    }
                    _spiteToDrag = null;
                }
            }
        }

        void OnTouchesBegan(List<CCTouch> touches, CCEvent touchEvent)
        {
            // We only care about the first touch:
            var touch = touches[0];
            foreach (var p in _peaces)
            {
                if (isTouchingPeace(touch, p.Sprite) && !isPeaceAtHome(p))
                {
                    _spiteToDrag = new DraggingSpite(p, touch.Location);
                }
            }

        }

        void OnTouchesMoved(System.Collections.Generic.List<CCTouch> touches, CCEvent touchEvent)
        {
            // We only care about the first touch:
            var locationOnScreen = touches[0].Location;
            if (_spiteToDrag.HasValue)
            {
                var value = _spiteToDrag.Value;
                value.Peace.Sprite.Position = value.StartPosition + locationOnScreen - value.DragStart;
                if (isPeaceCloseToHome(value.Peace))
                {
                    AssemblePeace(value.Peace);
                    _spiteToDrag = null;
                }
            }
        }

        bool isTouchingPeace(CCTouch touch, CCSprite peace)
        {
            // This includes the rectangular white space around our sprite
            return peace.BoundingBox.ContainsPoint(touch.Location);
        }

        bool isPeaceAtHome(Peace peace)
        {
            return peace.Sprite.Position == peace.AssembledPos;
        }

        bool isPeaceCloseToHome(Peace peace)
        {
            const int MIN_DISTANCE = 20;
            return CCPoint.Distance(peace.Sprite.Position, peace.AssembledPos) < MIN_DISTANCE;
        }
    }
}

