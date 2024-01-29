using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStateMachine : MonoBehaviour
{
    /// <summary>
    ///  ��ʼ״̬��
    /// </summary>
    public StateMachine SM = new StateMachine();

    private void OnEnable()
    {
        /* ����Ϸ��ʼ������������Դ��ڵ�״̬ */
        SM.AddState(new IdleState(SM));
    }

    private void Update()
    {
        SM.Update();
    }

}
