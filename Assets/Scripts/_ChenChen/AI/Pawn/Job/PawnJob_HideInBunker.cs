using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_HideInBunker : PawnJob
    {
        //持续最大时间
        private readonly static float tick = 500;
        private readonly Vector3 bunker;
        private readonly Pawn enemy;

        public PawnJob_HideInBunker(Pawn pawn, TargetPtr target) : base(pawn, tick, target)
        {
            bunker = target.PositonA;
            enemy = target.TargetB.GetComponent<Pawn>();
        }

        public override bool OnEnter()
        {
            Vector3 dir = bunker - enemy.transform.position;
            dir.Normalize();
            Vector3 pos = bunker + new Vector3(1 * (dir.x > 0 ? 1 : -1), 1 * (dir.y > 0 ? 1 : -1), 0);
            pawn.MoveController.GoToHere(pos);
            Debug.Log($"前往{bunker} + {dir}");
            return true;
        }

        public override StateType OnUpdate()
        {
            if (pawn.MoveController.ReachDestination)
            {
                pawn.StateMachine.TryChangeState(new PawnJob_Battle(pawn, new TargetPtr(enemy.gameObject)));
                Debug.Log($"战斗");
            }
            return StateType.Doing;
        }
    }
}