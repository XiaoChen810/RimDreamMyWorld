using ChenChen_Thing;

namespace ChenChen_UI
{
    public class DetailViewPanel_Thing : DetailViewPanel
    {
        private ThingBase thing;

        public DetailViewPanel_Thing(ThingBase thing, Callback onEnter, Callback onExit) : base(onEnter, onExit) 
        {
            this.thing = thing;
        }    
    }
}