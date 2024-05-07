using UnityEngine;
using ChenChen_BuildingSystem;

namespace ChenChen_AI
{
    public class CharacterMain : Pawn
    {
        protected override void Start()
        {
            base.Start();
        }

        protected override void TryToGetJob()
        {
            GameObject job = null;
            if (!Info.IsOnWork && Def.CanGetJob)
            {
                job = new JobGiver_Building().TryIssueJobPackage(this);
                if (job != null)
                {
                    StateMachine.NextState = new PawnJob_Building(this, job);
                    return;
                }

                job = BuildingSystemManager.Instance.GetThingGenerated("µöÓãµã", needFree: true);
                if (job != null)
                {
                    StateMachine.NextState = new PawnJob_Fishing(this, job);
                    return;
                }

                job = BuildingSystemManager.Instance.GetThingGenerated(BuildingLifeStateType.MarkDemolished, needFree: true);
                if (job != null)
                {
                    StateMachine.NextState = new PawnJob_Demolished(this, job);
                    return;
                }
            }
            return;
        }
    }
}