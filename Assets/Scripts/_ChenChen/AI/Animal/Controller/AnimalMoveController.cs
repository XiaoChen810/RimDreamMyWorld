using ChenChen_Map;
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
        public bool GoToHere(Vector3 target, Urgency urgency = Urgency.Normal, float endReachedDistance = 0.2f, bool isAquaticAnimals = false)
        {
            if (isAquaticAnimals && MapManager.Instance.GetMapNodeHere(target).type != NodeType.water) return false;
            if (!isAquaticAnimals && MapManager.Instance.GetMapNodeHere(target).type == NodeType.water) return false;

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
            return StartPath(target, endReachedDistance);
        }
    }
}