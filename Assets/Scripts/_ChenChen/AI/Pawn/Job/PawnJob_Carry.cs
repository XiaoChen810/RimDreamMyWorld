using ChenChen_Core;
using ChenChen_Thing;
using System.Linq;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Carry : PawnJob
    {
        //持续最大时间
        private readonly static float tick = 500;

        private readonly Item item;    // 要搬运的东西
        private readonly Building building; // 要搬运的目标
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
            
            Description = $"搬运 {XmlLoader.Instance.GetDef(item.name).name} 到 {building.name}";

            return true;
        }

        public override StateType OnUpdate()
        {
            if (item == null || building == null)
            {
                return StateType.Failed;
            }

            // 阶段一，去拿物资
            if (stage == 0)
            {
                pawn.MoveController.GoToHere(item.gameObject.transform.position);
                stage = 1;
            }
            // 阶段二，运送物资
            if (stage == 1 && pawn.MoveController.ReachDestination)
            {
                //逻辑
                var required = building.RequiredMaterialList.First(x => x.Item1 == item.Label);

                // 当需要的物资多了时
                if(item.Num > required.Item2)
                {
                    Debug.Log($"生成新的剩余数量的item {item.Num - required.Item2}");
                    Item newItem = ThingSystemManager.Instance.GenerateItem(item.Def, item.transform.position, item.Num - required.Item2);

                    // 只拿需要的物资
                    item.Num = required.Item2;
                }

                // 将需要的物资拿在手中
                item.transform.parent = pawn.transform;
                item.transform.localPosition = pawn.hand;
                item.SR.sortingLayerName = "Above";
                item.SR.sortingOrder = 10;

                pawn.MoveController.GoToHere(building.transform.position, endReachedDistance: pawn.WorkRange);

                stage = 2;
            }
            // 阶段三，存放物资
            if (stage == 2 && pawn.MoveController.ReachDestination)
            {
                //逻辑
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