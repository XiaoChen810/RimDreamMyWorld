using ChenChen_Core;
using ChenChen_Thing;
using System.Linq;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Carry : PawnJob
    {
        //�������ʱ��
        private readonly static float tick = 500;

        private Item item;    // Ҫ���˵Ķ���
        private Building building; // Ҫ���˵�Ŀ��
        private int stage = 0;

        public PawnJob_Carry(Pawn pawn, TargetPtr target) : base(pawn, tick, target)
        {
            this.Description = $"���� {XmlLoader.Instance.GetDef(target.TargetA.name).name} �� {target.TargetB.name}";
        }

        public override bool OnEnter()
        {
            bool res = base.OnEnter();
            if(res == false) { return false; }

            if (!target.TargetA.TryGetComponent<Item>(out item))
            {
                DebugLogDescription = $"�޷��������������{target.TargetA.name}";
                return false;
            }

            if (!target.TargetB.TryGetComponent<Building>(out building))
            {
                DebugLogDescription = ($"{target.TargetB.name}���ǿɴ洢���");
                return false;
            }

            return true;
        }

        public override StateType OnUpdate()
        {
            if (target != null && (target.TargetA == null || target.TargetB == null))
            {
                return StateType.Failed;
            }

            if(item.UserPawn != pawn || building.UserPawn != pawn)
            {
                Debug.LogError($"{pawn.name} ����");
                return StateType.Failed;
            }

            // �׶�һ��ȥ������
            if (stage == 0)
            {
                pawn.MoveController.GoToHere(target.TargetA);
                stage = 1;
            }
            // �׶ζ�����������
            if (stage == 1 && pawn.MoveController.ReachDestination)
            {
                //�߼�
                var wuzi = building.RequiredMaterialList.First(x => x.Item1 == item.Label);

                // ����Ҫ�����ʶ���ʱ
                if(item.Num > wuzi.Item2)
                {
                    Debug.Log($"�����µ�ʣ��������item {item.Num - wuzi.Item2}");
                    Item newItem = ThingSystemManager.Instance.TryGenerateItem(item.Def, item.transform.position, item.Num - wuzi.Item2);

                    // ֻ����Ҫ������
                    item.Num = wuzi.Item2;
                }

                // ����Ҫ��������������
                item.transform.parent = pawn.transform;
                item.transform.localPosition = pawn.hand;
                item.SR.sortingLayerName = "Above";
                item.SR.sortingOrder = 10;

                pawn.MoveController.GoToHere(target.TargetB);

                stage = 2;
            }
            // �׶������������
            if (stage == 2 && pawn.MoveController.ReachDestination)
            {
                //�߼�
                building.Put(item.Label, item.Num);
                item.Num = 0;
                stage = 3;
            }

            if (stage == 3)
            {
                return StateType.Success;
            }

            return StateType.Doing;
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void OnInterrupt()
        {
            OnExit();
        }
    }
}