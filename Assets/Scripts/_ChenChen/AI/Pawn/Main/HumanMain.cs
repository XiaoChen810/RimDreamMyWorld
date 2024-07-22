using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;

namespace ChenChen_AI
{
    public class HumanMain : Pawn
    {
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
                // 将列表重新按优先级大到小排列
                jobGivers = jobGivers.OrderByDescending(jg => jg.Priority).ToList();
            }
            else
            {
                Debug.LogWarning("列表中没有这个属性");
            }
        }

        protected override void Start()
        {
            base.Start();
            jobGivers = new List<JobGiver>();

            // 使用反射自动添加JobGiver和PawnJob
            AddJobGiversAndJobs();
        }

        private void AddJobGiversAndJobs()
        {
            // 获取当前程序集中的所有类型
            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes();

            // 找到所有非抽象的PawnJob子类
            var pawnJobTypes = types.Where(t => t.IsSubclassOf(typeof(PawnJob)) && !t.IsAbstract);

            foreach (var pawnJobType in pawnJobTypes)
            {
                Action<GameObject> onGetJobSuccessly = (GameObject job) =>
                {
                    var pawnJob = (PawnJob)Activator.CreateInstance(pawnJobType, this, job);
                    StateMachine.NextState = pawnJob;
                };

                // 获取对应的JobGiver类型名称
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

        private float _lastGetJobTime = 0;
        private float _getJobDuration = 0.2f;

        protected override void TryToGetJob()
        {
            if (Time.time > _lastGetJobTime + _getJobDuration)
            {
                _lastGetJobTime = Time.time;
                foreach (JobGiver jobGiver in jobGivers)
                {
                    if(jobGiver.Priority == 0)
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
            return;
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