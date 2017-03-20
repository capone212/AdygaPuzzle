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
        AnimalInfo _animal;
        Director _parent;
        image _currentImage;
        List<Peace> _peaces = new List<Peace>();
        DraggingSpite? _spiteToDrag = null;
        List<CCSprite> _allSprites = new List<CCSprite>();
        CCSprite _fullPictureSprite;
        CCSprite _toMenu;
        PopBalloon _baloonLayer = null;
        string _animalType;
        CCLabel _label;
        CCSprite _nextAnimal;
        volatile bool _cancelled = false;

        public GameLayer(Director parent, string type, AnimalInfo animal) : base(CCColor4B.Blue)
        {
            _parent = parent;
            _animalType = type;
            _animal = animal;
        }

        static void Swap(IList<Peace> list, int indexA, int indexB)
        {
            var tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }

        protected override void AddedToScene()
        {
            base.AddedToScene();

            // Use the bounds to layout the positioning of our drawable assets
            var bounds = VisibleBoundsWorldspace;
            // ----------------------- Start
            _parent.LogInfo("[XML_TEST] writing xml");
            // TODO: open with content manager not with android specific staff.
            using (var stackXml = _parent.OpenAsset("Content/Images/Animals/"+_animal.Id+"/stack.xml"))
            {
                _currentImage = ParseHelpers.ParseXML<image>(stackXml);
            }
            _parent.LogInfo(string.Format("width = {0}, height={1} layers count={2}", _currentImage.w, _currentImage.h, _currentImage.stack.Length));
            _parent.LogInfo(string.Format("VisibleBoundsWorldspace minX={0}, MinY={1}, maxX={2}, maxY={3}", bounds.MinX, bounds.MinY, bounds.MaxX, bounds.MaxY));
            Array.Reverse(_currentImage.stack);
            var background = new CCSprite("background.png");
            background.Position = bounds.Center;
            AddChild(background);

            _toMenu = new CCSprite("back");
            _toMenu.Position = new CCPoint(bounds.Center.X, 40);
            AddChild(_toMenu);

            foreach (var peace in _currentImage.stack)
            {
                string prefix = _animal.Id;
                var spite = new CCSprite(_animal.Id + "/" + peace.src);
                spite.PositionX = (spite.ContentSize.Width) / 2 + peace.x;
                spite.PositionY = _currentImage.h - peace.y - (spite.ContentSize.Height -1) / 2;
                _parent.LogInfo(string.Format("Adding spyte {0} content size X:{1} Y:{2} W:{3} H:{4}", prefix + peace.src, spite.PositionX, spite.PositionY, spite.ContentSize.Width, spite.ContentSize.Height));
                if (peace.name == "!")
                {
                    _fullPictureSprite = spite;
                    continue;
                }
                AddChild(spite);
                int n;
                if (int.TryParse(peace.name, out n))
                {
                    _parent.LogInfo(string.Format("Save movable spite with name {0}", peace.name));
                    _peaces.Add(new Peace(spite,  spite.Position));
                    _allSprites.Add(spite);
                }
                else if (peace.name == "background")
                {
                    _allSprites.Add(spite);
                }
            }

            AddChild(_fullPictureSprite);

            // Sort by heagh
            _peaces.Sort(delegate (Peace x, Peace y)
            {
                var xs = x.Sprite.BoundingBox.Size;
                var ys = y.Sprite.BoundingBox.Size;
                return xs.Height.CompareTo(ys.Height);
            });
            
            if (_peaces[4].HalfWidth < _peaces[3].HalfWidth)
            {
                Swap(_peaces, 4, 3);
            }


            const int XGAP = 30;
            const int YGAP = 20;
            var viewPort = new CCRect(XGAP, YGAP, bounds.Size.Width - 2 * XGAP, bounds.Size.Height - 2 * YGAP);

            // Calculate exploded location
            _peaces[4].DisassembledPos = new CCPoint(viewPort.MaxX - _peaces[4].HalfWidth, viewPort.MaxY - _peaces[4].HalfHeigh);
            _peaces[3].DisassembledPos = new CCPoint(viewPort.MaxX - _peaces[3].HalfWidth, _peaces[3].HalfHeigh + viewPort.MinY);
            _peaces[2].DisassembledPos = new CCPoint(viewPort.MinX + _peaces[2].HalfWidth, viewPort.MaxY - _peaces[2].HalfHeigh);
            _peaces[1].DisassembledPos = new CCPoint(viewPort.MinX + _peaces[1].HalfWidth, _peaces[1].HalfHeigh + viewPort.MinY);
            _peaces[0].DisassembledPos = new CCPoint(viewPort.MinX + _peaces[0].HalfWidth, _peaces[1].DisassembledMaxY + (_peaces[2].DisassembledMinY - _peaces[1].DisassembledMaxY)/2);

            // Add caption label            
            _label = new CCLabel(_animal.DisplayName, "Gagalin-36", 36, CCLabelFormat.SpriteFont);
            _label.PositionX = bounds.Center.X;
            _label.PositionY = 20 + _label.ContentSize.Height / 2;
            _label.Color = CCColor3B.Black;
            AddChild(_label);
            _label.Visible = false;

            // Add next level button
            _nextAnimal = new CCSprite("nextAnimal");
            _nextAnimal.PositionX = bounds.MaxX - 150;
            _nextAnimal.PositionY = bounds.Center.Y;
            AddChild(_nextAnimal);
            _nextAnimal.Visible = false;


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
            CCAudioEngine.SharedEngine.PlayEffect(filename: "elements-break");
            foreach (var p in _peaces)
            {
                DisassemblePease(p, 1.3f);
            }
        }

        void ScheduleAction(Action f, int milliseconds)
        {
            new Timer( x=> 
            {
                if (!_cancelled)
                {
                    f();
                }
            }, null, milliseconds);
        }

        void ScheduleBreak()
        {
            ScheduleAction(breakToPeaces, 1000);
        }

        void Assemble()
        {
            foreach (var p in _peaces)
            {
                p.Sprite.Position = p.AssembledPos;
            }
        }

        void Challenge()
        {
            const int MAX_CHALLENGE_COUNT = 2;
            var index = _parent.Rand.Next(1, 11);
            if (index <= MAX_CHALLENGE_COUNT)
            {
                CCAudioEngine.SharedEngine.PlayEffect(filename: string.Format("challenge{0}", index));
            }
        }

        public void StartGame(PopBalloon layer)
        {
            _baloonLayer = layer;
            Assemble();
            ScheduleBreak();
            Challenge();
        }

        void DisassemblePease(Peace p, float seconds)
        {
            var easeMove = new CCEaseElasticOut(new CCMoveTo(seconds, p.DisassembledPos));
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
                CCAudioEngine.SharedEngine.PlayEffect(string.Format("aplauz{0}", _parent.Rand.Next(1, 3)));
                ScheduleAction(() => { StartPopBaloons(); }, 100);
                ScheduleAction(() => { AnimateCharacter(); }, 8  * 1000);
            },1000);
        }

        DateTime? _lastCharacterAnimated = null;

        void AnimateCharacter()
        {
            _label.Visible = true;
            _nextAnimal.Visible = true;
            _toMenu.PositionX = 50;
            _toMenu.Visible = true;
            _lastCharacterAnimated = DateTime.Now;
            var scaleIn = new CCEaseInOut(new CCScaleBy(0.5f, 1.1f), 1.5f);
            var scaleOut = new CCEaseInOut(new CCScaleBy(0.5f, 0.9f), 1.5f);
            // create a delay that is run in between sequence events
            var delay = new CCDelayTime(0.25f);
            // create the sequence of actions, in the order we want to run them
            var animate = new CCSequence(scaleIn, delay, scaleOut);
            _fullPictureSprite.RunAction(animate);
            CCAudioEngine.SharedEngine.PlayEffect(filename: _animal.Id);
        }

        void HandleCharacterTouch()
        {
            if (!_lastCharacterAnimated.HasValue)
                return;
            var diff = DateTime.Now - _lastCharacterAnimated.Value;
            if (diff.TotalSeconds < 1.5)
                return;
            AnimateCharacter();
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
            CCAudioEngine.SharedEngine.PlayEffect(filename: "wood-click");
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
                        DisassemblePease(_spiteToDrag.Value.Peace, 1f);
                        CCAudioEngine.SharedEngine.PlayEffect(filename: "wrong");
                    }
                    _spiteToDrag = null;
                    return;
                }

                if (isTouchingPeace(touch, _fullPictureSprite))
                {
                    HandleCharacterTouch();
                    return;
                }

                if (_nextAnimal.Visible && isTouchingPeace(touch, _nextAnimal))
                {
                    cancelAllEffects();
                    _parent.RunNextGame(_animalType, _animal);
                    return;
                }

                if (_toMenu.Visible && isTouchingPeace(touch, _toMenu))
                {
                    cancelAllEffects();
                    _parent.RunMenu(_animalType);
                    return;
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
                    return;
                }
            }

        }

        void cancelAllEffects()
        {
            _cancelled = true;
            CCAudioEngine.SharedEngine.StopAllEffects();
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

        bool isTouchingPeace(CCTouch touch, CCNode peace)
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

        public void StartPopBaloons()
        {
            _toMenu.Visible = false;
            var layer = _baloonLayer;
            if (layer == null)
                return;
            try
            {
                layer.StartBaloons();
                ScheduleAction(() =>
                {
                    layer.RemoveFromParent();
                    layer.Dispose();
                }, 8 * 1000);
            }
            catch(Exception ex)
            {
                _parent.LogInfo(string.Format("[ERROR] Cant add baloon layer {0}", ex));
            }
        }

    }
}

