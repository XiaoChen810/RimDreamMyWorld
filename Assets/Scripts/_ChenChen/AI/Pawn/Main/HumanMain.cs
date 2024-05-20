using UnityEngine;
using System.Collections.Generic;
using ChenChen_UISystem;

namespace ChenChen_AI
{
    public class HumanMain : Pawn
    {
        protected List<JobGiver> jobGivers;

        protected override void Start()
        {
            base.Start();
            jobGivers = new List<JobGiver>();
            // 睡觉
            jobGivers.Add(new JobGiver_Sleep((GameObject job) =>
            {
                StateMachine.NextState = new PawnJob_Sleep(this, job);
            }));
            // 建造
            jobGivers.Add(new JobGiver_Building((GameObject job) =>
            {
                StateMachine.NextState = new PawnJob_Building(this, job);
            }));
            // 拆除
            jobGivers.Add(new JobGiver_Demolish((GameObject job) =>
            {
                StateMachine.NextState = new PawnJob_Demolished(this, job);
            }));
            // 种植
            jobGivers.Add(new JobGiver_Farming((GameObject job) =>
            {
                StateMachine.NextState = new PawnJob_Farming(this, job);
            }));
            // 钓鱼
            jobGivers.Add(new JobGiver_Fishing((GameObject job) =>
            {
                StateMachine.NextState = new PawnJob_Fishing(this, job);
            }));
        }

        protected override void TryToGetJob()
        {    
            foreach (JobGiver jobGiver in jobGivers)
            {
                GameObject job = jobGiver.TryIssueJobPackage(this);
                if (job != null)
                {
                    CurJobTarget = job;
                    return;
                }
            }
            return;
        }

        protected override List<PawnNeed> InitNeedsList()
        {
            List<PawnNeed> needs = new List<PawnNeed>();
            needs.Add(new PawnNeed_HavePet());
            needs.Add(new PawnNeed_Misc("想要吃饭"));
            needs.Add(new PawnNeed_Misc("想要睡觉"));
            needs.Add(new PawnNeed_Misc("想要玩永杰无间"));
            needs.Add(new PawnNeed_Misc("想要玩英雄联盟"));
            return needs;
        }


        protected override void TryToGetNeed()
        {
            float needProbabilityRange = 0;
            foreach (PawnNeed need in _needsList)
            {
                needProbabilityRange += need.Probability;
            }
            float randomProbability = Random.Range(0f, needProbabilityRange); 
            for (int i = 0; i < _needsList.Count; i++)
            {
                randomProbability -= _needsList[i].Probability;
                if (randomProbability <= 0) 
                {
                    string content = $"{Def.PawnName}{_needsList[i].Description}";
                    ScenarioManager.Instance.Narrative(content, this.gameObject);
                    return;
                }
            }
        }

    }
}