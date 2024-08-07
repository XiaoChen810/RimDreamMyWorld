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

        private Item item;    // 要搬运的东西
        private Building building; // 要搬运的目标
        private int stage = 0;

        public PawnJob_Carry(Pawn pawn, TargetPtr target) : base(pawn, tick, target)
        {
            this.Description = $"搬运 {XmlLoader.Instance.GetDef(target.TargetA.name).name} 到 {target.TargetB.name}";
        }

        public override bool OnEnter()
        {
            bool res = base.OnEnter();
            if(res == false) { return false; }

            if (!target.TargetA.TryGetComponent<Item>(out item))
            {
                DebugLogDescription = $"无法搬运这个东西：{target.TargetA.name}";
                return false;
            }

            if (!target.TargetB.TryGetComponent<Building>(out building))
            {
                DebugLogDescription = ($"{target.TargetB.name}不是可存储物件");
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
                Debug.LogError($"{pawn.name} 错误");
                return StateType.Failed;
            }

            // 阶段一，去拿物资
            if (stage == 0)
            {
                pawn.MoveController.GoToHere(target.TargetA);
                stage = 1;
            }
            // 阶段二，运送物资
            if (stage == 1 && pawn.MoveController.ReachDestination)
            {
                //逻辑
                var wuzi = building.RequiredMaterialList.First(x => x.Item1 == item.Label);

                // 当需要的物资多了时
                if(item.Num > wuzi.Item2)
                {
                    Debug.Log($"生成新的剩余数量的item {item.Num - wuzi.Item2}");
                    Item newItem = ThingSystemManager.Instance.TryGenerateItem(item.Def, item.transform.position, item.Num - wuzi.Item2);

                    // 只拿需要的物资
                    item.Num = wuzi.Item2;
                }

                // 将需要的物资拿在手中
                item.transform.parent = pawn.transform;
                item.transform.localPosition = pawn.hand;
                item.SR.sortingLayerName = "Above";
                item.SR.sortingOrder = 10;

                pawn.MoveController.GoToHere(target.TargetB);

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
            OnExit();
        }
    }
}