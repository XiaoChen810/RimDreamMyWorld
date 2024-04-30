using ChenChen_BuildingSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_MapGenerator
{
    public class ItemCreator
    {
        /// <summary>
        /// 生成物体
        /// </summary>
        /// <param name="itemName"></param>
        public void GenerateItem(string itemName, Vector2 pos, string mapName,bool isFromSave)
        {
            ThingDef thing_generated = BuildingSystemManager.Instance.GetThingDef(itemName);
            if (thing_generated != null)
            {
                Data_ThingSave thingSave = new Data_ThingSave(thing_generated, pos, Quaternion.identity, mapName);
                BuildingSystemManager.Instance.TryGenerateThing(thingSave);
                if (!isFromSave) PlayManager.Instance.SaveDate.Things.Add(thingSave);
            }
        }
    }
}
