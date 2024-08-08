using ChenChen_Thing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Map
{
    public class ItemCreator
    {
        /// <summary>
        /// 生成物体从新
        /// </summary>
        /// <param name="itemName"></param>
        public void GenerateItem(string itemName, Vector2Int pos, string mapName, bool isNotCommonlyUsed = false)
        {
            BuildingDef thing_generated = ThingSystemManager.Instance.GetThingDef(itemName);
            if (thing_generated != null)
            {
                //Data_ThingSave thingSave = new Data_ThingSave(thing_generated.DefName, pos, Quaternion.identity, BuildingLifeStateType.None);
                //ThingSystemManager.Instance.TryGenerateThing(thingSave);             
                ThingSystemManager.Instance.GenerateBuilding(thing_generated, pos, true);
            }
        }
        /// <summary>
        /// 生成物体从存档
        /// </summary>
        /// <param name="thingSave"></param>
        public void LoadItemFromSave(Data_GameSave gameSave)
        {
            foreach (var thingSave in gameSave.SaveThings)
            {
                //ThingSystemManager.Instance.TryGenerateThing(thingSave);
            }         
        }
    }
}
