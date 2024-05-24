using System;
using UnityEngine;

namespace ChenChen_AI
{
    public class SimpleMoveController : MoveController
    {

        /// <summary>
        /// 前往到目标点
        /// </summary>
        /// <param name="targetPos"></param>
        /// <returns></returns>
        public bool GoToHere(Vector3 targetPos, Urgency urgency = Urgency.Normal, float endReachedDistance = 0.2f)
        {
            switch (urgency)
            {
                case Urgency.Wander:
                    Speed = 1;
                    break;
                case Urgency.Normal:
                    Speed = 2;
                    break;
                case Urgency.Urge:
                    Speed = 3;
                    break;
                default:
                    Speed = 2;
                    break;
            }
            return StartPath(targetPos, endReachedDistance);
        }

        /// <summary>
        /// 跟随目标
        /// </summary>
        /// <param name="targetObj"></param>
        /// <returns></returns>
        public bool GoToHere(GameObject targetObj, Urgency urgency = Urgency.Normal, float endReachedDistance = 0.2f)
        {
            switch (urgency)
            {
                case Urgency.Wander:
                    Speed = 1;
                    break;
                case Urgency.Normal:
                    Speed = 2;
                    break;
                case Urgency.Urge:
                    Speed = 3;
                    break;
                default:
                    Speed = 2;
                    break;
            }
            return StartPath(targetObj, endReachedDistance);
        }
    }
}
