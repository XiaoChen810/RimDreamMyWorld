using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;

namespace ChenChen_AI
{
    public class HumanMain : Pawn
    {
        #region - JobGiver -

        protected List<JobGiver> jobGivers;

        public IReadOnlyCollection<JobGiver> JobGivers
        {
            get
            {
                return jobGivers.AsReadOnly();
            }
        }

        public void ChangeJobGiverPriority(JobGiver changed, int value)
        {
            if (jobGivers.Contains(changed))
            {
                changed.Priority = value;
                jobGivers = jobGivers.OrderByDescending(jg => jg.Priority).ToList();
            }
            else
            {
                Debug.LogWarning("列表中没有这个属性");
            }
        }

        private void AddJobGiversAndJobs()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            var pawnJobTypes = types.Where(t => t.IsSubclassOf(typeof(PawnJob)) && !t.IsAbstract);
            
            foreach (var pawnJobType in pawnJobTypes)
            {
                Action<GameObject> onGetJobSuccessly = (GameObject job) =>
                {
                    var pawnJob = (PawnJob)Activator.CreateInstance(pawnJobType, this, job);
                    StateMachine.NextState = pawnJob;
                };

                var jobTypeName = pawnJobType.Name.Replace("PawnJob", "JobGiver");
                var jobGiverType = types.FirstOrDefault(t => t.Name == jobTypeName && t.IsSubclassOf(typeof(JobGiver)) && !t.IsAbstract);

                JobGiver jobGiver = null;

                if (jobGiverType != null)
                {
                    jobGiver = (JobGiver)Activator.CreateInstance(jobGiverType, onGetJobSuccessly);
                }

                if (jobGiver != null)
                {
                    jobGivers.Add(jobGiver);
                }
            }
        }

        #endregion

        protected override void Start()
        {
            base.Start();
            jobGivers = new List<JobGiver>();

            AddJobGiversAndJobs();
        }

        protected override void TryToGetJob()
        {
            foreach (JobGiver jobGiver in jobGivers)
            {
                if (jobGiver.Priority == 0)
                {
                    break;
                }
                GameObject job = jobGiver.TryIssueJobPackage(this);
                if (job != null)
                {
                    CurJobTarget = job;
                    return;
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("MonsterAttackBox"))
            {
                Monster monster = collision.GetComponentInParent<Monster>();
                if (monster != null)
                {
                    GetDamage(monster.gameObject, monster.attackDamage);
                }
                else
                {
                    Debug.LogWarning("没有组件");
                }
            }
        }
    }
}