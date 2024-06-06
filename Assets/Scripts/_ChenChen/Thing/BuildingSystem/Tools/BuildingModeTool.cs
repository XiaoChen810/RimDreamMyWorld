using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using ChenChen_Map;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace ChenChen_Thing
{
    /// <summary>
    /// �򿪽���ģʽ�����ý���ʲô��
    /// </summary>
    public class BuildingModeTool
    {
        public ThingSystemManager BuildingSystemManager;
        /// <summary>
        /// ��ǰ��ͼ������
        /// </summary>
        public string CurBuildingName;
        /// <summary>
        /// ��ǰ�����嶨��
        /// </summary>
        public ThingDef CurBuildingDef;
        /// <summary>
        /// ��ǰ�����ThingBase���
        /// </summary>
        public ThingBase CurBuildingBase;
        /// <summary>
        /// �Ƿ������ڽ���ģʽ��
        /// </summary>
        public bool OnBuildMode { get; private set; }
        /// <summary>
        /// ��ǰ����ϵ�Ԥ��
        /// </summary>
        public GameObject MouseIndicator;

        public BuildingModeTool(ThingSystemManager buildingSystemManager)
        {
            BuildingSystemManager = buildingSystemManager;
        }

        public static readonly string mouseIndicator_string = "MouseIndicator";
        /// <summary>
        /// Start
        /// </summary>
        /// <param name="def">�����Ķ���</param>
        public void BuildStart(ThingDef def)
        {
            CurBuildingDef = def;
            CurBuildingName = def.DefName;
            MouseIndicator = UnityEngine.Object.Instantiate(CurBuildingDef.Prefab);
            MouseIndicator.name = mouseIndicator_string;
            MouseIndicator.SetActive(true);
            CurBuildingBase = MouseIndicator.GetComponent<ThingBase>();
            OnBuildMode = true;
        }

        /// <summary>
        /// Update�����������Ϣ��������ͼ��
        /// </summary>
        public void BuildUpdate()
        {
            if (OnBuildMode && MouseIndicator != null)
            {
                // �������λ����Ϣת������������, ����ȡ��Ϊ��������
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int placePosition = StaticFuction.VectorTransToInt(mousePosition);
                MouseIndicator.transform.position = placePosition + new Vector3(CurBuildingDef.Offset.x, CurBuildingDef.Offset.y);

                // ����ܽ�������������Ϊ��ɫ������Ϊ��ɫ
                SpriteRenderer sr = CurBuildingBase.SR;
                if (CurBuildingBase.CanBuildHere())
                {
                    sr.color = Color.green;
                    // ����
                    if (Input.GetMouseButtonDown(0))
                    {
                        Vector2 posInt = new Vector2(placePosition.x,placePosition.y);
                        Quaternion rot = MouseIndicator.transform.rotation;
                        BuildingSystemManager.TryGenerateThing(CurBuildingDef, posInt, rot, 0, MapManager.Instance.CurrentMapName);
                    }
                }
                else
                {
                    sr.color = Color.red;
                }

                // ��ת
                if (CurBuildingDef.CanRotation)
                {
                    // ������Q��������˳ʱ����ת90��
                    if (Input.GetKeyDown(KeyCode.Q))
                    {
                        MouseIndicator.transform.Rotate(Vector3.forward, 90f);
                    }
                    // ������E����������ʱ����ת90��
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        MouseIndicator.transform.Rotate(Vector3.forward, -90f);
                    }
                }

                //ȡ��
                if (Input.GetMouseButtonDown(1))
                {
                    BuildEnd();
                }
            }
        }

        /// <summary>
        /// End
        /// </summary>
        public void BuildEnd()
        {
            OnBuildMode = false;
            MouseIndicator.SetActive(false);
            UnityEngine.Object.Destroy(MouseIndicator);
            CurBuildingBase = null;
            CurBuildingDef = null;
            CurBuildingName = null;
        }
    }
}