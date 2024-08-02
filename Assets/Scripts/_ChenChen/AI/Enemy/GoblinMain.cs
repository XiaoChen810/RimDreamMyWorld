using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_AI
{
    public class GoblinMain : Pawn
    {
        [SerializeField] private float chaseRange = 14.0f;

        float lastGetJobTime = 0;

        protected override void TryToGetJob()
        {
            if (Time.time > lastGetJobTime + 2f)
            {
                foreach (var pawn in GameManager.Instance.PawnGeneratorTool.PawnsList)
                {
                    float distacne = (Vector2.Distance(transform.position, pawn.transform.position));
                    if (distacne < chaseRange)
                    {
                        StateMachine.NextState = new PawnJob_Chase(this, pawn.gameObject);
                    }
                }
                lastGetJobTime = Time.time;
            }
        }
    }
}
