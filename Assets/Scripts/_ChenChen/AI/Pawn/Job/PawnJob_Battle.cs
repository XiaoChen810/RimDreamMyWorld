using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Battle : PawnJob
    {
        // 持续最大时间
        private readonly static float tick = 500;
        private readonly Pawn enemy;
        private float range;    // 攻击范围
        private float warmupTime;    // 前摇
        private float rangedWeaponCooldown;  // 后摇 
        private float timer;

        private enum AttackState
        {
            None,
            WarmingUp,
            Attacking,
            CoolingDown
        }

        private AttackState attackState = AttackState.None;

        public PawnJob_Battle(Pawn pawn, TargetPtr target) : base(pawn, tick, target)
        {
            enemy = target.TargetA.GetComponent<Pawn>();
        }

        public override bool OnEnter()
        {
            if (enemy == null)
            {
                return false;
            }

            range = pawn.Weapon.range;
            warmupTime = pawn.Weapon.warmupTime;
            rangedWeaponCooldown = pawn.Weapon.rangedWeaponCooldown;
            timer = 0f;
            attackState = AttackState.None;

            Description = $"正在追击 {enemy.Def.PawnName}";
            pawn.JobToDo(target);
            pawn.MoveController.GoToHere(enemy.gameObject, endReachedDistance: range);
            return true;
        }

        public override StateType OnUpdate()
        {
            if (target.TargetA == null)
            {                
                return StateType.Failed;
            }

            if (enemy == null || enemy.Info.IsDead)
            {
                Debug.LogWarning($"目标 {enemy.name} 被击杀");
                return StateType.Success;
            }

            // 索敌
            if (Vector2.Distance(pawn.transform.position, enemy.transform.position) < range)
            {
                switch (attackState)
                {
                    case AttackState.None:
                        //Debug.Log("开始前摇");
                        attackState = AttackState.WarmingUp;
                        timer = warmupTime;
                        break;

                    case AttackState.WarmingUp:
                        //Debug.Log("前摇计时");
                        if (timer > 0)
                        {
                            timer -= Time.deltaTime;
                            pawn.SetWeaponAngle(enemy.transform.position);
                            pawn.ChangeMyBar(Mathf.Lerp(0, 1, timer / warmupTime));
                        }
                        else
                        {
                            //Debug.Log("前摇结束，开始攻击");
                            attackState = AttackState.Attacking;
                            pawn.MoveController.ForceReach();
                            pawn.SetDamage(enemy.gameObject, pawn.Weapon.isMelee);
                            timer = rangedWeaponCooldown;
                            pawn.ChangeMyBar(0);
                        }
                        break;

                    case AttackState.Attacking:
                        //Debug.Log("攻击后摇");
                        pawn.ChangeMyBar(0);
                        if (timer > 0)
                        {
                            timer -= Time.deltaTime;
                        }
                        else
                        {
                            Debug.Log("后摇结束，重新开始攻击循环");
                            attackState = AttackState.None;
                        }
                        break;

                    case AttackState.CoolingDown:
                        break;
                }
            }
            else
            {
                if (pawn.MoveController.ReachDestination)
                {
                    pawn.MoveController.GoToHere(enemy.gameObject, endReachedDistance: range);
                }
                attackState = AttackState.None;
                pawn.ChangeMyBar(0);
                //Debug.Log("如果敌人超出范围，重置攻击状态");
            }

            return StateType.Doing;
        }

        public override void OnExit()
        {
            pawn.ChangeMyBar(0);
            pawn.EndBattle();
            pawn.MoveController.ForceReach();
        }
    }
}
