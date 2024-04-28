using UnityEngine;
using ChenChen_BuildingSystem;
using ChenChen_AI;

public class CharacterMain : Pawn
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void TryToGetJob()
    {
        GameObject job = null;
        if (!IsOnWork && CanGetJob)
        {
            job = new JobGiver_Building().TryIssueJobPackage(this);
            if (job != null)
            {
                StateMachine.NextState = new ChenChen_AI.PawnJob_Building(this, job);
                return;
            }

            job = BuildingSystemManager.Instance.GetThingGenerated("µöÓãµã", needFree: true);
            if (job != null)
            {
                StateMachine.NextState = new ChenChen_AI.PawnJob_Fishing(this, job);
                return;
            }

            job = BuildingSystemManager.Instance.GetThingGenerated(BuildingStateType.WaitingDemolished, needFree: true);
            if (job != null)
            {
                StateMachine.NextState = new ChenChen_AI.PawnJob_Demolished(this, job);
                return;
            }
        }
        return;
    }
}
