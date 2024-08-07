using ChenChen_Thing;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_AI
{
    public class JobGiver_Carry : JobGiver
    {
        private static readonly string JOB_NAME = "搬运";
        private static readonly float INTERVAL_TIME = 0.5f;

        public JobGiver_Carry(Action<TargetPtr> onGetJobSuccessly) : base(onGetJobSuccessly, JOB_NAME, INTERVAL_TIME)
        {
        }

        protected override TargetPtr TryGiveJob(Pawn pawn)
        {
            if (!pawn.Def.CanBuild) return null;

            ThingSystemManager thingSystemManager = ThingSystemManager.Instance;

            // 搬运去建造
            var buildings = thingSystemManager.GetThingsInstance<Building>();

            foreach (var building in buildings)
            {
                if (building.LifeState == BuildingLifeStateType.MarkBuilding && !building.RequiredMaterialsLoadFull)
                {
                    // 检查所需材料
                    foreach (var required in building.RequiredMaterialList)
                    {
                        string materialLabel = required.Item1; // 材料标签
                        int requiredAmount = required.Item2;  // 所需数量

                        Item selectedItem = FindSuitableItem(materialLabel, requiredAmount);

                        if (selectedItem != null)
                        {
                            return new TargetPtr(selectedItem.gameObject, building.gameObject);
                        }
                    }
                }
            }

            // 搬运去制作
            var tools = thingSystemManager.GetThingsInstance<Thing_Tool>();

            foreach (var tool in tools)
            {
                if (tool.IsFullRequiredForMade)
                {
                    foreach(var required in tool.RequiredForMade)
                    {
                        string materialLabel = required.Item1; // 材料标签
                        int requiredAmount = required.Item2;  // 所需数量

                        Item selectedItem = FindSuitableItem(materialLabel, requiredAmount);

                        if (selectedItem != null)
                        {
                            return new TargetPtr(selectedItem.gameObject, tool.gameObject);
                        }
                    }
                }
            }

            return null;
        }

        private static Item FindSuitableItem(string materialLabel, int requiredAmount)
        {
            Item res = null;
            int maxAvailable = 0;

            IReadOnlyList<Item> items = ThingSystemManager.Instance.GetThingsInstance<Item>();

            foreach (var item in items)
            {
                if (item.Label == materialLabel)
                {
                    if (item.Num == requiredAmount)
                    {
                        res = item;
                        break;
                    }
                    else if (maxAvailable < requiredAmount && item.Num > maxAvailable)
                    {
                        maxAvailable = item.Num;
                        res = item;
                    }
                }
            }

            return res;
        }
    }
}
