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
        private string _curBuildingName;
        /// <summary>
        /// ��ǰ�����嶨��
        /// </summary>
        private ThingDef _curBuildingDef;
        /// <summary>
        /// ��ǰ�����ThingBase���
        /// </summary>
        private Thing _curBuildingBase;
        /// <summary>
        /// �Ƿ������ڽ���ģʽ��
        /// </summary>
        public bool OnBuildMode { get; private set; }
        /// <summary>
        /// ��ǰ����ϵ�Ԥ��
        /// </summary>
        public GameObject _mouseIndicator;

        public BuildingModeTool(ThingSystemManager buildingSystemManager)
        {
            BuildingSystemManager = buildingSystemManager;
        }

        /// <summary>
        /// Start
        /// </summary>
        /// <param name="def">�����Ķ���</param>
        public void BuildStart(ThingDef def)
        {
            if(OnBuildMode)
            {
                BuildEnd();
            }
            _curBuildingName = def.DefName;
            _curBuildingDef = def;
            _mouseIndicator = UnityEngine.Object.Instantiate(_curBuildingDef.Prefab);
            _mouseIndicator.name = "MouseIndicator";
            _mouseIndicator.GetComponent<SpriteRenderer>().sortingLayerName = "Above";
            _curBuildingBase = _mouseIndicator.GetComponent<Thing>();
            OnBuildMode = true;
        }

        /// <summary>
        /// Update�����������Ϣ��������ͼ��
        /// </summary>
        public void BuildUpdate()
        {
            if (OnBuildMode && _mouseIndicator != null)
            {
                // �������λ����Ϣת������������, ����ȡ��Ϊ��������
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int placePosition = StaticFuction.VectorTransToInt(mousePosition);
                _mouseIndicator.transform.position = placePosition + new Vector3(_curBuildingDef.Offset.x, _curBuildingDef.Offset.y);

                // ����ܽ�������������Ϊ��ɫ������Ϊ��ɫ
                SpriteRenderer sr = _curBuildingBase.SR;
                if (_curBuildingBase.CanBuildHere())
                {
                    sr.color = Color.green;
                    // ����갴��, ���ж�һ���ܷ��죬Ȼ����
                    if (Input.GetMouseButton(0) && _curBuildingBase.CanBuildHere())
                    {
                        Vector2 posInt = new Vector2(placePosition.x,placePosition.y);
                        Quaternion rot = _mouseIndicator.transform.rotation;
                        BuildingSystemManager.TryGenerateThing(_curBuildingDef, posInt, rot);
                    }
                }
                else
                {
                    sr.color = Color.red;
                }

                // ��ת
                if (_curBuildingDef.CanRotation)
                {
                    // ������Q��������˳ʱ����ת90��
                    if (Input.GetKeyDown(KeyCode.Q))
                    {
                        _mouseIndicator.transform.Rotate(Vector3.forward, 90f);
                    }
                    // ������E����������ʱ����ת90��
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        _mouseIndicator.transform.Rotate(Vector3.forward, -90f);
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
            UnityEngine.Object.Destroy(_mouseIndicator);
            _curBuildingBase = null;
            _curBuildingDef = null;
            _curBuildingName = null;
        }
    }
}