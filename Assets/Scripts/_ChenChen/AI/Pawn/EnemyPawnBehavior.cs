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

                if (pawn.StateMachine.IsDefault)
                {
                    pawn.StateMachine.TryChangeState(new PawnJob_Move(pawn, 
                        pawn.transform.position + new Vector3(UnityEngine.Random.Range(-5, 5), UnityEngine.Random.Range(-5, 5)), 
                        Urgency.Wander));
                }

                if (time_prepare <= 0 && !raid_start)
                {
                    raid_start = true;
                    Debug.Log($"{pawn.name}发起袭击");
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
        }

        private Pawn GetNearestColonyPawn()
        {
            IReadOnlyList<Pawn> colonyPawnList = GameManager.Instance.PawnGeneratorTool.PawnList_Colony;
            Pawn nearestPawn = null;
            float nearestDistance = float.MaxValue;

            foreach (var colonyPawn in colonyPawnList)
            {
                float distance = Vector2.Distance(pawn.transform.position, colonyPawn.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestPawn = colonyPawn;
                }
            }

            return nearestPawn;
        }

        private void TryChaseTarget()
        {
            if (pawn.StateMachine.CurStateType == typeof(PawnJob_Battle)) return;
            if (target_pawn == target_pawn_last) return;
            target_pawn_last = target_pawn;
            pawn.StateMachine.TryChangeState(new PawnJob_Battle(pawn, new TargetPtr(target_pawn.gameObject)));
            Debug.Log($"{pawn.name} 追击 {target_pawn.name}");
        }


    }
}