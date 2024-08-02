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
            if (!_animal.MoveController.GoToHere(_moveTargetPosition, Urgency.Wander, isAquaticAnimals: _animal.Def.IsAquaticAnimals))
            {
                return false;
            }
            return true;
        }

        public override StateType OnUpdate()
        {
            if (_animal.MoveController.ReachDestination)
            {
                return StateType.Success;
            }

            return StateType.Doing;
        }
    }
}
