using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_AI
{
    public class MasterController : MoveController
    {
        /// <summary>
        /// ǰ����Ŀ���
        /// </summary>
        /// <param name="targetPos"></param>
        /// <returns></returns>
        public bool GoToHere(Vector3 targetPos, float endReachedDistance = 0.2f)
        {
            return StartPath(targetPos, endReachedDistance);
        }
    }
}