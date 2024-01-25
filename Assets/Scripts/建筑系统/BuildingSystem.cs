using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using 地图生成;

namespace 建筑系统
{
    public class BuildingSystem : MonoBehaviour
    {
        [Header("瓦片地图")]
        public Tilemap BuildingTilemap;
        public Tilemap WallTilemap;

        [Header("当前建造的物品名字")]
        public string BuildingName;
        private BlueprintData blueprintData;

        [Header("开启建造模式")]
        public bool BuildingMode;
        private bool isOpenBuilding;

        [Header("鼠标指示器")]
        public GameObject MouseIndicator;

        Action<Vector3> OnPlace, OnCancel;


        private void Update()
        {
            // 当建造模式开关打开时，只进行一次获取蓝图数据和配置相关信息
            if (BuildingMode && !isOpenBuilding)
            {
                isOpenBuilding = true;

                //  找到当前应该放置的蓝图
                blueprintData = BuildingSystemManager.Instance.GetData(BuildingName);
                if (blueprintData == null)
                {
                    Debug.LogWarning("不存在此蓝图");
                    BuildEnd();
                    return;
                }
                else
                {
                    // 获取TileMap
                    BuildingTilemap = MapManager.Instance.GetChildObject("Building").GetComponent<Tilemap>();
                    WallTilemap = MapManager.Instance.GetChildObject("Wall").GetComponent<Tilemap>();


                    MouseIndicator.SetActive(true);
                    SpriteRenderer sr = MouseIndicator.GetComponent<SpriteRenderer>();
                    sr.sprite = blueprintData.PreviewSprite;
                }
            }

            BuildUpdate();


        }

        private void BuildUpdate()
        {
            if (BuildingMode && isOpenBuilding)
            {
                // 监听鼠标位置信息转换成世界坐标
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int cellPosition = BuildingTilemap.WorldToCell(mousePosition);
                Vector3 placePosition = BuildingTilemap.CellToWorld(cellPosition) + new Vector3(0.5f, 0.5f);
                MouseIndicator.transform.position = cellPosition + new Vector3(0.5f, 0.5f);

                // 放置和取消
                if (Input.GetMouseButtonDown(0))
                {
                    OnPlace?.Invoke(cellPosition);
                    GameObject newObject = Instantiate(blueprintData.Prefab, placePosition, Quaternion.identity);
                    IBlueprint blueprint = newObject.GetComponent<IBlueprint>();
                    blueprint.Placed();
                }
                if (Input.GetMouseButtonDown(1))
                {
                    OnCancel?.Invoke(cellPosition);
                    BuildEnd();
                }
            }
        }

        private void BuildEnd()
        {
            BuildingMode = false;
            isOpenBuilding = false;
            MouseIndicator.SetActive(false);
        }
    }
}