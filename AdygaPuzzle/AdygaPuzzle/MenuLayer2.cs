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

        Dictionary<string, EasingFactory> _easings = new Dictionary<string, EasingFactory>();

        public MenuLayer(Director activity) : base(CCColor4B.Blue)
        {
            _activity = activity;
            _easings.Add("none", a => { return a; });
            _easings.Add("CCEaseBackIn", a => { return new CCEaseBackIn(a); });
            _easings.Add("CCEaseBackInOut", a => { return new CCEaseBackInOut(a); });
            _easings.Add("CCEaseBackOut", a => { return new CCEaseBackOut(a); });
            _easings.Add("CCEaseBounceIn", a => { return new CCEaseBounceIn(a); });
            _easings.Add("CCEaseBounceInOut", a => { return new CCEaseBounceInOut(a); });
            _easings.Add("CCEaseBounceOut", a => { return new CCEaseBounceOut(a); });
            _easings.Add("CCEaseElastic", a => { return new CCEaseElastic(a); });
            _easings.Add("CCEaseElasticIn", a => { return new CCEaseElasticIn(a); });
            _easings.Add("CCEaseElasticInOut", a => { return new CCEaseElasticInOut(a); });
            _easings.Add("CCEaseElasticOut", a => { return new CCEaseElasticOut(a); });
            _easings.Add("CCEaseExponentialIn", a => { return new CCEaseExponentialIn(a); });
            _easings.Add("CCEaseExponentialInOut", a => { return new CCEaseExponentialInOut(a); });
            _easings.Add("CCEaseExponentialOut", a => { return new CCEaseExponentialOut(a); });
            _easings.Add("CCEaseIn", a => { return new CCEaseIn(a, 2); });
            _easings.Add("CCEaseInOut", a => { return new CCEaseInOut(a, 2); });
            _easings.Add("CCEaseOut", a => { return new CCEaseOut(a, 2); });

            _selectedName = "none";
        }


        protected override void AddedToScene()
        {
            base.AddedToScene();

            CCMenuItemFont.FontSize = 50;
            CCMenuItemFont.FontName = "arial";

            CCMenuItemFont title1 = new CCMenuItemFont("Easing Style");
            title1.Enabled = false;

            CCMenuItemFont.FontSize = 50;

            var items = from s in _easings
                        select new CCMenuItemFont(s.Key);
            var easingTypes = new CCMenuItemToggle(this.menuCallback, items.ToArray());

          

            var label = new CCLabel("StartGame", "Arial", 50, CCLabelFormat.SystemFont);
            var back = new CCMenuItemLabel(label, this.backCallback);

            CCMenu menu = new CCMenu(
                title1, easingTypes,  back) { Tag = 36 }; // 9 items.

            menu.AlignItemsInColumns(2, 2, 2, 2, 1);

            AddChild(menu);
        }
        string _selectedName = string.Empty;
        public void menuCallback(object pSender)
        {
            var label = ((CCMenuItemToggle)pSender).SelectedItem as CCMenuItemLabel;
            _selectedName = label.Label.Text;
            //UXLOG("selected item: %x index:%d", dynamic_cast<CCMenuItemToggle*>(sender)->selectedItem(), dynamic_cast<CCMenuItemToggle*>(sender)->selectedIndex() ); 
        }

        public void backCallback(object pSender)
        {
            _activity.RunGame(_easings[_selectedName]);
        }
    }
}
