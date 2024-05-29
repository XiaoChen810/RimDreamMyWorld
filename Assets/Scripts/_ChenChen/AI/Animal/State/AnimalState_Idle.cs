using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_AI
{
    public class AnimalState_Idle : AnimalStateBase
    {
        float _time;
        float _waitTime = 5;

        public AnimalState_Idle(Animal animal, StateBase next = null) : base(animal, next)
        {
        }

        public override StateType OnUpdate()
        {
            if (_animal.StateMachine.NextState != null || !(_animal.StateMachine.StateQueue.Count == 0))
            {
                return StateType.Success;
            }

            _time += Time.deltaTime;
            if (_time > _waitTime)
            {
                _waitTime = 0;
                Vector2 p = _animal.transform.position;
                p += new Vector2(Random.Range(-5, 5), Random.Range(-5, 5));
                _animal.StateMachine.NextState = new AnimalState_Move(_animal, p);
                return StateType.Success;
            }
            return StateType.Doing;
        }
    }
}
