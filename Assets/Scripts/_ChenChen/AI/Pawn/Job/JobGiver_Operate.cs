using ChenChen_Thing;
using System;
using System.Linq;
using UnityEngine;

namespace ChenChen_AI
{
    public class JobGiver_Operate : JobGiver
    {
        private static readonly string JOB_NAME = "操作";
        private static readonly float INTERVAL_TIME = 0.2f;

        public JobGiver_Operate(Action<TargetPtr> onGetJobSuccessly) : base(onGetJobSuccessly, JOB_NAME, INTERVAL_TIME)
        {
        }

        protected override TargetPtr TryGiveJob(Pawn pawn)
        {
            // 逻辑
            var list = ThingSystemManager.Instance.GetThingsInstance<Building>();

            var operate = list.FirstOrDefault(x => x is IOperate && x.GetComponent<IOperate>().IsWaitToOperate);

            if (operate != null)
            {
                return new TargetPtr(operate.gameObject);
            }

            return null;
        }
    }
}
