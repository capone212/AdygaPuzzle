
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CocosSharp;

namespace AdygaPuzzle
{
    public class MenuLayer2 : CCLayer
    {
        Director _activity;
        public MenuLayer2(Director director) : base()
        {
            _activity = director;
        }


        protected override void AddedToScene()
        {
            base.AddedToScene();
            var label = new CCLabel("Go To Menu", "Arial", 50, CCLabelFormat.SystemFont);
            var back = new CCMenuItemLabel(label, this.backCallback);
            CCMenu menu = new CCMenu(back) { Tag = 36 }; // 9 items.
            back.PositionY = -250;
            back.PositionX = 0;
            AddChild(menu);
        }

        public void menuCallback(object pSender)
        {
            //UXLOG("selected item: %x index:%d", dynamic_cast<CCMenuItemToggle*>(sender)->selectedItem(), dynamic_cast<CCMenuItemToggle*>(sender)->selectedIndex() ); 
        }

        public void backCallback(object pSender)
        {
            _activity.RunMenu();
        }
    }
}
