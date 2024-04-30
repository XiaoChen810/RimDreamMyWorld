using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_AI
{
    public interface IState
    {
        bool OnEnter();
        StateType OnUpdate();
        void OnExit();
        void OnPause();
        void OnResume();
        void OnInterrupt();
    }
}