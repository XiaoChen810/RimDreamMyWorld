using ChenChen_AI;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UISystem
{
    public class DetailViewPanel_Pawn : DetailViewPanel
    {
        private Pawn pawn;

        public DetailViewPanel_Pawn(Pawn pawn, Callback onEnter, Callback onExit) : base(onEnter, onExit)
        {
            this.pawn = pawn;
        }
    }
}
