using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_AI
{
    public class ArmyPawnBehavior
    {
        private Pawn me;
        private Pawn target_pawn = null;
        private Pawn target_pawn_last = null;

        public ArmyPawnBehavior(Pawn pawn)
        {
            this.me = pawn;
        }

        public void Behavior()
        {
            target_pawn = GetNearestPawn();
            
            if (target_pawn != null && target_pawn_last != target_pawn)
            {
                target_pawn_last = target_pawn;
                TryChaseTarget();
            }

            return;
        }


        private Pawn GetNearestPawn()
        {
            IReadOnlyList<Pawn> totalPawns = GameManager.Instance.PawnGeneratorTool.PawnList_All;
            Pawn nearestPawn = null;
            float nearestDistance = float.MaxValue;

            foreach (var p in totalPawns)
            {
                if (p == me) continue;
                if (p.Faction == me.Faction) continue;
                if (p.Info.IsDead) continue;

                float distance = Vector2.Distance(me.transform.position, p.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestPawn = p;
                }
            }

            return nearestPawn;
        }

        private void TryChaseTarget()
        {
            me.StateMachine.TryChangeState(new PawnJob_Battle(me, new TargetPtr(target_pawn.gameObject)));
            Debug.Log($"{me.name} 追击 {target_pawn.name}");
        }
    }
}
