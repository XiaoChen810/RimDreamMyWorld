using UnityEngine;
using System.Collections.Generic;
using ChenChen_UISystem;

namespace ChenChen_AI
{
    public class HumanMain : Pawn
    {

        protected override void Start()
        {
            base.Start();
        }
        public string CurNeed => Info.Need.Description;
        public bool CurNeedIsComplete => Info.Need.IsCompelte;

        protected override void TryToGetJob()
        {
            GameObject job = null;
            // ½¨Ôì
            job = new JobGiver_Building().TryIssueJobPackage(this);
            if (job != null)
            {
                StateMachine.NextState = new PawnJob_Building(this, job);
                return;
            }
            // ²ð³ý
            job = new JobGiver_Demolish().TryIssueJobPackage(this);
            if (job != null)
            {
                StateMachine.NextState = new PawnJob_Demolished(this, job);
                return;
            }
            // ÖÖÖ²
            job = new JobGiver_Farming().TryIssueJobPackage(this);
            if (job != null)
            {
                StateMachine.NextState = new PawnJob_Farming(this, job);
                return;
            }
            // µöÓã
            job = new JobGiver_Fishing().TryIssueJobPackage(this);
            if (job != null)
            {
                StateMachine.NextState = new PawnJob_Fishing(this, job);
                return;
            }

            return;
        }

        protected override List<PawnNeed> InitNeedsList()
        {
            List<PawnNeed> needs = new List<PawnNeed>();
            needs.Add(new PawnNeed_HavePet());

            _needProbabilityRange = 0;
            foreach (PawnNeed need in needs)
            {
                _needProbabilityRange += need.Probability;
            }
            return needs;
        }


        protected override void TryToGetNeed()
        {
            float randomProbability = Random.Range(0, _needProbabilityRange);
            for (int i = 0; i < _needsList.Count; i++)
            {
                randomProbability -= _needsList[i].Probability;
                if(randomProbability < 0)
                {
                    string content = $"{Def.PawnName}{_needsList[i].Description}";
                    ScenarioManager.Instance.Narrative(content, this.gameObject);
                    Info.Need = (PawnNeed)_needsList[i].Clone();
                    return;
                }
            }
        }
    }
}