using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_AI
{
    public class EnemyPawnBehavior
    {
        private Pawn pawn;

        public EnemyPawnBehavior(Pawn pawn)
        {
            this.pawn = pawn;

            time_prepare = 10;

            target_center = new Vector2(ChenChen_Map.MapManager.Instance.CurMapWidth / 2, ChenChen_Map.MapManager.Instance.CurMapHeight / 2);
        }

        private Vector2 target_center = Vector2.zero;
        private Pawn target_pawn = null;
        private Pawn target_pawn_last = null;

        private float range_chase = 14;
        private float time_prepare = 0;

        private bool raid_start = false;

        private float time_check_target = 2f; 
        private float time_since_last_check = 0f;

        public void Behavior()
        {
            // 准备袭击时
            if (time_prepare > 0)
            {
                time_prepare -= Time.deltaTime;
                if (time_prepare <= 0 && !raid_start)
                {
                    raid_start = true;
                    pawn.StateMachine.TryChangeState(new PawnJob_Move(pawn, target_center, Urgency.Wander));
                    //Debug.Log($"{pawn.name}发起袭击");
                    return;
                }
            }

            // 发起袭击后
            if (raid_start)
            {
                time_since_last_check += Time.deltaTime;
                if (time_since_last_check >= time_check_target)
                {
                    time_since_last_check = 0f;

                    target_pawn = GetNearestColonyPawn();

                    if (target_pawn != null)
                    {
                        TryChaseTarget();
                    }
                }
            }

            if (pawn.StateMachine.IsIdle && target_pawn == null)
            {
                if (raid_start)
                {
                    pawn.StateMachine.TryChangeState(new PawnJob_Move(pawn, target_center, Urgency.Wander));
                }
                else
                {
                    Vector2 moveTo = pawn.transform.position + new Vector3(UnityEngine.Random.Range(-3f, 3f), UnityEngine.Random.Range(-3f, 3f), 0f);
                    pawn.StateMachine.TryChangeState(new PawnJob_Move(pawn, moveTo, Urgency.Wander));
                }
            }
        }

        private void TryChaseTarget()
        {
            if(pawn.StateMachine.CurStateType == typeof(PawnJob_Attack)) return;
            if (target_pawn == target_pawn_last) return;
            target_pawn_last = target_pawn;
            pawn.StateMachine.TryChangeState(new PawnJob_Chase(pawn, target_pawn.gameObject));
            Debug.Log($"{pawn.name} 追击 {target_pawn.name}");
        }

        private Pawn GetNearestColonyPawn()
        {
            IReadOnlyList<Pawn> colonyPawnList = GameManager.Instance.PawnGeneratorTool.PawnList_Colony;
            Pawn nearestPawn = null;
            float nearestDistance = float.MaxValue;

            foreach (var colonyPawn in colonyPawnList)
            {
                float distance = Vector2.Distance(pawn.transform.position, colonyPawn.transform.position);
                if (distance < nearestDistance && distance <= range_chase)
                {
                    nearestDistance = distance;
                    nearestPawn = colonyPawn;
                }
            }

            return nearestPawn;
        }
    }
}