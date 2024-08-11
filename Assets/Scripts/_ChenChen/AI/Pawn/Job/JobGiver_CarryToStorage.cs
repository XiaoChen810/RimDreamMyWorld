using ChenChen_Thing;
using System;
using UnityEngine;

namespace ChenChen_AI
{
    public class JobGiver_CarryToStorage : JobGiver
    {
        private static readonly string JOB_NAME = null;
        private static readonly float INTERVAL_TIME = 0.5f;

        public JobGiver_CarryToStorage(Action<TargetPtr> onGetJobSuccessly) : base(onGetJobSuccessly, JOB_NAME, INTERVAL_TIME)
        {
        }

        protected override TargetPtr TryGiveJob(Pawn pawn)
        {
            // 逻辑           
            var itemList = ThingSystemManager.Instance.GetThingsInstance<Item>();
            foreach(var item in itemList)
            {
                if (!item.IsOnStorage)
                {
                    GameObject obj = GameManager.Instance.WorkSpaceTool.GetAWorkSpace(WorkSpaceType.Storage);
                    if (obj != null)
                    {
                        var workSpace = obj.GetComponent<WorkSpace_Storage>();
                        if (workSpace != null)
                        {
                            return new TargetPtr(obj, item.gameObject);
                        }
                    }
                }
            }

            return null;
        }
    }
}
