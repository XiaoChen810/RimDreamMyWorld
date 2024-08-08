using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using ChenChen_Map;
using static UnityEngine.RuleTile.TilingRuleOutput;
using ChenChen_Core;

namespace ChenChen_Thing
{
    /// <summary>
    /// �򿪽���ģʽ�����ý���ʲô��
    /// </summary>
    public class BuildingModeTool
    {
        private ThingSystemManager manager;
        private GameObject _mouseIndicator;

        public bool OnBuildMode { get; private set; }

        private BuildingDef CurBuildingDef {  get; set; }

        public GameObject MouseIndicator
        {
            get
            {
                if(_mouseIndicator == null)
                {
                    _mouseIndicator = new GameObject("MouseIndicator");
                    _mouseIndicator.AddComponent<SpriteRenderer>().sortingLayerName = "Above";
                }
                return _mouseIndicator;
            }
        }

        public BuildingModeTool(ThingSystemManager buildingSystemManager)
        {
            manager = buildingSystemManager;
        }

        public void BuildStart(BuildingDef def)
        {
            if(OnBuildMode)
            {
                BuildEnd();
            }
            CurBuildingDef = def;
            MouseIndicator.gameObject.SetActive(true);
            MouseIndicator.GetComponent<SpriteRenderer>().sprite = def.PreviewSprite;
            OnBuildMode = true;
        }

        public void BuildUpdate()
        {
            if (OnBuildMode && MouseIndicator != null)
            {
                // �������λ����Ϣת������������, ����ȡ��Ϊ��������
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int placePosition = StaticFuction.VectorTransToInt(mousePosition);
                MouseIndicator.transform.position = placePosition + new Vector3(CurBuildingDef.Offset.x, CurBuildingDef.Offset.y);

                // ����ܽ�������������Ϊ��ɫ������Ϊ��ɫ
                SpriteRenderer sr = MouseIndicator.GetComponent<SpriteRenderer>();
                if (CanBuildHere())
                {
                    sr.color = Color.green;
                }
                else
                {
                    sr.color = Color.red;
                }

                // ����
                if (Input.GetMouseButton(0) && CanBuildHere())
                {
                    Vector2Int posInt = new Vector2Int(placePosition.x, placePosition.y);
                    manager.GenerateBuilding(CurBuildingDef, posInt, false);
                }

                // ȡ��
                if (Input.GetMouseButtonDown(1))
                {
                    BuildEnd();
                }
            }
        }

        private bool CanBuildHere()
        {
            Vector2Int posInt = new Vector2Int((int)MouseIndicator.transform.position.x, (int)MouseIndicator.transform.position.y);
            Vector2Int size = CurBuildingDef.Size;

            if (CurBuildingDef.IsEffectBuild) return true;

            return manager.CanBuildHere(posInt, size);
        }

        /// <summary>
        /// End
        /// </summary>
        public void BuildEnd()
        {
            OnBuildMode = false;
            CurBuildingDef = null;
            MouseIndicator.gameObject.SetActive(false);
        }
    }
}