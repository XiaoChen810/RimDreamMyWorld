using UnityEngine;

namespace ChenChen_AI
{
    public class GoblinMain : Pawn
    {
        protected override void TryToGetJob()
        {
            //GameObject job = null;
            //if (!Info.IsOnWork && Def.CanGetJob)
            //{
            //    job = new JobGiver_FindEnemy().TryIssueJobPackage(this);
            //    if (job != null && !Info.IsOnBattle)
            //    {
            //        StateMachine.NextState = new PawnJob_Chase(this, job);
            //        return;
            //    }
            //}
        }
    }
}