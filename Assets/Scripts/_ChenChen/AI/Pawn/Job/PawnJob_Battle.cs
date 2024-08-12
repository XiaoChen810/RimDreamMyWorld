using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Battle : PawnJob
    {
        // �������ʱ��
        private readonly static float tick = 500;
        private readonly Pawn enemy;
        private float range;    // ������Χ
        private float warmupTime;    // ǰҡ
        private float rangedWeaponCooldown;  // ��ҡ 
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

            Description = $"����׷�� {enemy.Def.PawnName}";
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
                Debug.LogWarning($"Ŀ�� {enemy.name} ����ɱ");
                return StateType.Success;
            }

            // ����
            if (Vector2.Distance(pawn.transform.position, enemy.transform.position) < range)
            {
                switch (attackState)
                {
                    case AttackState.None:
                        //Debug.Log("��ʼǰҡ");
                        attackState = AttackState.WarmingUp;
                        timer = warmupTime;
                        break;

                    case AttackState.WarmingUp:
                        //Debug.Log("ǰҡ��ʱ");
                        if (timer > 0)
                        {
                            timer -= Time.deltaTime;
                            pawn.SetWeaponAngle(enemy.transform.position);
                            pawn.ChangeMyBar(Mathf.Lerp(0, 1, timer / warmupTime));
                        }
                        else
                        {
                            //Debug.Log("ǰҡ��������ʼ����");
                            attackState = AttackState.Attacking;
                            pawn.MoveController.ForceReach();
                            pawn.SetDamage(enemy.gameObject, pawn.Weapon.isMelee);
                            timer = rangedWeaponCooldown;
                            pawn.ChangeMyBar(0);
                        }
                        break;

                    case AttackState.Attacking:
                        //Debug.Log("������ҡ");
                        pawn.ChangeMyBar(0);
                        if (timer > 0)
                        {
                            timer -= Time.deltaTime;
                        }
                        else
                        {
                            Debug.Log("��ҡ���������¿�ʼ����ѭ��");
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
                //Debug.Log("������˳�����Χ�����ù���״̬");
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
