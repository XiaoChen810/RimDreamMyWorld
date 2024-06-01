using UnityEngine;
using System.Collections.Generic;
using ChenChen_UI;

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
            // 切除
            jobGivers.Add(new JobGiver_Cut((GameObject job) =>
            {
                StateMachine.NextState = new PawnJob_Cut(this, job);
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
            // 社交
            jobGivers.Add(new JobGiver_Socialize((GameObject job) =>
            {
                StateMachine.NextState = new PawnJob_Socialize(this, job);
            }));
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