using ChenChen_Thing;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_CarryToStorage : PawnJob
    {
        //�������ʱ��
        private readonly static float tick = 500;
        private WorkSpace_Storage storage;
        private Item item;
        private Vector2 _positon;
        private bool getItem = false;

        public PawnJob_CarryToStorage(Pawn pawn, TargetPtr target) : base(pawn, tick, target)
        {
            storage = target.TargetA.GetComponent<WorkSpace_Storage>();
            item = target.TargetB.GetComponent<Item>();
        }

        public override bool OnEnter()
        {
            var baseResult = base.OnEnter();
            if (baseResult != true) return baseResult;

            //�߼�
            if (storage != null || item == null)
            {
                Debug.LogWarning("storage != null || item == null");
            }

            if (!storage.TryGetAStoragePosition(item.Label, out _positon))
            {
                Debug.LogWarning("get position failed");
            }

            pawn.MoveController.GoToHere(item.transform.position);

            return true;
        }

        public override StateType OnUpdate()
        {
            var baseResult = base.OnUpdate();
            if (baseResult != StateType.Doing) return baseResult;

            //�߼�
            if (pawn.MoveController.ReachDestination && !getItem)
            {
                var newItem = ThingSystemManager.Instance.GenerateItem(item.Def, item.transform.position, item.Num);
                item.Num = 0;
                item = newItem;
                item.transform.parent = pawn.transform;
                item.SR.sortingLayerName = "Above";
                item.SR.sortingOrder = 0;
                Debug.Log(" ��ȡ ");
                pawn.MoveController.GoToHere(_positon);
                getItem = true;
            }

            if (pawn.MoveController.ReachDestination && getItem)
            {
                storage.TryPutAItemOnPosition(item, _positon);
                Debug.Log(" ��� ");
                return StateType.Success;
            }

            return StateType.Doing;
        }
    }
}