using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using ChenChen_MapGenerator;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace ChenChen_BuildingSystem
{
    /// <summary>
    /// �򿪽���ģʽ�����ý���ʲô��
    /// </summary>
    public class BuildingModeTool
    {
        public BuildingSystemManager BuildingSystemManager;
        /// <summary>
        /// ��ǰ��ͼ������
        /// </summary>
        public string CurBuildingName;
        /// <summary>
        /// ��ǰ�����嶨��
        /// </summary>
        public ThingDef CurBuildingDef;
        /// <summary>
        /// �Ƿ������ڽ���ģʽ��
        /// </summary>
        public bool OnBuildMode;
        /// <summary>
        /// ��ǰ����ϵ�Ԥ��
        /// </summary>
        public GameObject MouseIndicator;

        public BuildingModeTool(BuildingSystemManager buildingSystemManager)
        {
            BuildingSystemManager = buildingSystemManager;
        }

        /// <summary>
        /// Start
        /// </summary>
        /// <param name="def">�����Ķ���</param>
        public void BuildStart(ThingDef def)
        {
            CurBuildingDef = def;
            CurBuildingName = def.DefName;

            // ��ȡ��ǰ��ͼ��TileMap
            Tilemap main = MapManager.Instance.CurMapMainTilemap;

            // �������ָʾ����Ϣ
            OnBuildMode = true;
            MouseIndicator = UnityEngine.Object.Instantiate(CurBuildingDef.Prefab);
            if (!MouseIndicator.GetComponent<Collider2D>())
            {
                Debug.LogError("ERROR: MouseIndicator no Collider2D");
                GameObject.Destroy(MouseIndicator);
                return;
            }
            MouseIndicator.SetActive(true);

        }

        /// <summary>
        /// Update�����������Ϣ��������ͼ��
        /// </summary>
        public void BuildUpdate()
        {
            if (OnBuildMode && MouseIndicator != null)
            {
                // �������λ����Ϣת������������
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Tilemap curTilemap = MapManager.Instance.CurMapMainTilemap;
                Vector3Int cellPosition = curTilemap.WorldToCell(mousePosition);
                Vector3 placePosition = curTilemap.CellToWorld(cellPosition);
                MouseIndicator.transform.position = placePosition + new Vector3(CurBuildingDef.offset.x, CurBuildingDef.offset.y);

                SpriteRenderer sr = MouseIndicator.GetComponent<SpriteRenderer>();
                // ����ܽ�������������Ϊ��ɫ������Ϊ��ɫ
                if (CanBuildHere(MouseIndicator))
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
        }

        // ����Ƿ��ܹ���ָ��λ�÷���ָ������Ϸ����
        public bool CanBuildHere(GameObject objectToBuild)
        {
            // ��ȡ�����ö���� Collider2D ���
            Collider2D collider = objectToBuild.GetComponent<Collider2D>();

            // ��������ö���û�� Collider2D ������򷵻� false
            if (collider == null)
            {
                Debug.LogWarning("Object to build does not have a Collider2D component.");
                return false;
            }

            // ��ȡ�����ö��� Collider2D �ı߽����Ϣ
            Bounds bounds = collider.bounds;

            // ִ����ײ��⣬ֻ���ָ��ͼ�����ײ��
            Collider2D[] colliders = Physics2D.OverlapBoxAll(objectToBuild.transform.position, bounds.size, 0f);

            // ������⵽����ײ����������κ�һ����ײ�����ڣ��򷵻� false����ʾ�޷�������Ϸ����
            foreach (Collider2D otherCollider in colliders)
            {
                if (otherCollider.gameObject != objectToBuild) // ���Դ�������Ϸ�������ײ
                {
                    return false;
                }
            }

            // ���û���κ���ײ�����ڣ����ʾ���Է�����Ϸ����
            return true;
        }
    }
}