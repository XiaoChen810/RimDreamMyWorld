using UnityEngine;
using ChenChen_AI;

public class GoblinMain : Pawn
{
    protected override void TryToGetJob()
    {
        GameObject job = null;
        if (!IsOnWork && CanGetJob)
        {
            job = new JobGiver_FindEnemy().TryIssueJobPackage(this);
            if (job != null)
            {
                StateMachine.NextState = new ChenChen_AI.PawnJob_Chase(this, job);
                return;
            }
        }
    }
}







