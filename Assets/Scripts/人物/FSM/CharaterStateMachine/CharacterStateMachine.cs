using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStateMachine : MonoBehaviour
{
    /// <summary>
    ///  初始状态机
    /// </summary>
    public StateMachine SM = new StateMachine();

    private void OnEnable()
    {
        /* 在游戏开始添加这个人物可以存在的状态 */
        SM.AddState(new IdleState(SM));
    }

    private void Update()
    {
        SM.Update();
    }

}
