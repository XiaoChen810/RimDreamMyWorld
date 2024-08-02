using ChenChen_Thing;

namespace ChenChen_UI
{
    public class DetailViewPanel_Thing : DetailViewPanel
    {
        private Thing thing;

        public DetailViewPanel_Thing(Thing thing, Callback onEnter, Callback onExit) : base(onEnter, onExit) 
        {
            this.thing = thing;
        }    
    }
}