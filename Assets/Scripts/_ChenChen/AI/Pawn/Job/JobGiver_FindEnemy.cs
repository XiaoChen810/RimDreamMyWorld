using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_AI
{
    public class JobGiver_FindEnemy : JobGiver
    {
        private static readonly float interval_time = 1;
        private static readonly string jobName = "Ѳ��";
        public JobGiver_FindEnemy(Action<GameObject> onGetJobSuccessly) : base(onGetJobSuccessly, jobName, interval_time)
        {
        }

        protected override GameObject TryGiveJob(Pawn pawn)
        {
            GameObject job = null;
            float distance = float.MaxValue;
            foreach (var pawnObj in GameManager.Instance.PawnGeneratorTool.PawnsList)
            {
                //Ѱ�����Լ���Ӫ��ͬ��
                if (pawnObj.GetComponent<Pawn>().Def.PawnFaction != pawn.Def.PawnFaction)
                {
                    //�����Լ������
                    float thisDistance = Vector2.Distance(pawnObj.transform.position, pawn.transform.position);
                    distance = distance <= thisDistance ? distance : thisDistance;
                    job = pawnObj;
                }
            }

            return job;
        }
    }
}