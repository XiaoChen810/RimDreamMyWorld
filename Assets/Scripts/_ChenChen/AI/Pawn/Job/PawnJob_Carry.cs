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

        private readonly Item item;    // Ҫ���˵Ķ���
        private readonly Building building; // Ҫ���˵�Ŀ��
        private int stage = 0;

        public PawnJob_Carry(Pawn pawn, TargetPtr target) : base(pawn, tick, target)
        {
            item = target.TargetA.GetComponent<Item>();
            building = target.TargetB.GetComponent<Building>();
        }

        public override bool OnEnter()
        {
            if (item == null || building == null)
            {
                return false;
            }

            bool res = base.OnEnter();
            if(res == false) { return false; }
            
            Description = $"���� {XmlLoader.Instance.GetDef(item.name).name} �� {building.name}";

            return true;
        }

        public override StateType OnUpdate()
        {
            if (item == null || building == null)
            {
                return StateType.Failed;
            }

            // �׶�һ��ȥ������
            if (stage == 0)
            {
                pawn.MoveController.GoToHere(item.gameObject.transform.position);
                stage = 1;
            }
            // �׶ζ�����������
            if (stage == 1 && pawn.MoveController.ReachDestination)
            {
                //�߼�
                var required = building.RequiredMaterialList.First(x => x.Item1 == item.Label);

                // ����Ҫ�����ʶ���ʱ
                if(item.Num > required.Item2)
                {
                    Debug.Log($"�����µ�ʣ��������item {item.Num - required.Item2}");
                    Item newItem = ThingSystemManager.Instance.GenerateItem(item.Def, item.transform.position, item.Num - required.Item2);

                    // ֻ����Ҫ������
                    item.Num = required.Item2;
                }

                // ����Ҫ��������������
                item.transform.parent = pawn.transform;
                item.transform.localPosition = pawn.hand;
                item.SR.sortingLayerName = "Above";
                item.SR.sortingOrder = 10;

                pawn.MoveController.GoToHere(building.transform.position, endReachedDistance: pawn.WorkRange);

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
            item.transform.parent = null;
            item.SR.sortingLayerName = "Default";
            item.SR.sortingOrder = 0;
            OnExit();
        }
    }
}