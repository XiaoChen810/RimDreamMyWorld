using ChenChen_MapGenerator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_AI
{
    [RequireComponent(typeof(Collider2D))]
    public class AnimalMoveController : MoveController
    {
        /// <summary>
        /// 前往到目标点
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool GoToHere(Vector3 target, Urgency urgency = Urgency.Normal, float endReachedDistance = 0.2f)
        {
            if (MapManager.Instance.GetMapNodeHere(target).type == NodeType.water)
            {
                return false;
            }

            switch (urgency)
            {
                case Urgency.Wander:
                    speed = 1;
                    break;
                case Urgency.Normal:
                    speed = 2;
                    break;
                case Urgency.Urge:
                    speed = 3;
                    break;
                default:
                    speed = 2;
                    break;
            }
            return StartPath(target, speed, endReachedDistance);
        }
    }
}