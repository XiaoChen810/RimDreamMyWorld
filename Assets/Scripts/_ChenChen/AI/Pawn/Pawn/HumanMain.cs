using UnityEngine;
using ChenChen_BuildingSystem;

namespace ChenChen_AI
{
    public class HumanMain : Pawn
    {
        protected override void Start()
        {
            base.Start();
        }

        protected override void TryToGetJob()
        {
            GameObject job = null;
            // ½¨Ôì
            job = new JobGiver_Building().TryIssueJobPackage(this);
            if (job != null)
            {
                StateMachine.NextState = new PawnJob_Building(this, job);
                return;
            }
            // ²ð³ý
            job = new JobGiver_Demolish().TryIssueJobPackage(this);
            if (job != null)
            {
                StateMachine.NextState = new PawnJob_Demolished(this, job);
                return;
            }
            // ÖÖÖ²
            job = new JobGiver_Farming().TryIssueJobPackage(this);
            if (job != null)
            {
                StateMachine.NextState = new PawnJob_Farming(this, job);
                return;
            }
            // µöÓã
            job = new JobGiver_Fishing().TryIssueJobPackage(this);
            if (job != null)
            {
                StateMachine.NextState = new PawnJob_Fishing(this, job);
                return;
            }

            return;
        }
    }
}