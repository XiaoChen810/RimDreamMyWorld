using System;
using UnityEngine;

namespace ChenChen_AI
{
    public class AnimalState_Move : AnimalStateBase
    {
        private Vector3 _moveTargetPosition;

        public AnimalState_Move(Animal animal, Vector3 positon) : base(animal)
        {
            _moveTargetPosition = positon;
        }

        public override bool OnEnter()
        {
            // 设置目标点
            if (!_animal.MoveController.GoToHere(_moveTargetPosition, Urgency.Wander, isAquaticAnimals: _animal.Def.IsAquaticAnimals))
            {
                //DebugLogDescription = ("无法移动到目标点");
                return false;
            }
            return true;
        }

        public override StateType OnUpdate()
        {
            // 判断是否到达目标点
            if (_animal.MoveController.ReachDestination)
            {
                return StateType.Success;
            }

            return StateType.Doing;
        }
    }
}
