using ChenChen_Thing;

namespace ChenChen_UI
{
    public class DetailViewPanel_Tree : DetailViewPanel
    {
        private Thing_Tree tree;

        public DetailViewPanel_Tree(Thing_Tree tree, Callback onEnter, Callback onExit) : base(onEnter, onExit)
        {
            this.tree = tree;
        }
    }
}
