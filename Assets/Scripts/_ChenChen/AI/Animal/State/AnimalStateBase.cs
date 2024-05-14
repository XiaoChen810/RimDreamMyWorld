using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_AI
{
    public abstract class AnimalStateBase : StateBase
    {
        protected Animal _animal;

        protected AnimalStateBase(Animal animal, StateBase next = null) : base(animal.StateMachine, next)
        {
            _animal = animal;
        }
    }
}