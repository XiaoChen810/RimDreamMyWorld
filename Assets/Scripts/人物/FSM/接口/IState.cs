using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StateType
{
    Success, Failed, Doing, Interrupt
}

public interface IState
{
    void OnEnter();
    StateType OnUpdate();
    void OnExit();
    void OnPause();
    void OnResume();
    void OnInterrupt();
}
