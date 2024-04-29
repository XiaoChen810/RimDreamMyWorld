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
        /// ��ǰ����ϵ���ͼԤ��
        /// </summary>
        public GameObject MouseIndicator;

        public BuildingModeTool()
        {

        }

        /// <summary>
        /// Start
        /// </summary>
        /// <param name="def">�����Ķ���</param>
        public void BuildStart(ThingDef def)
        {
            CurBuildingDef = def;
            CurBuildingName = def.Name;

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
                Vector3 placePosition = curTilemap.CellToWorld(cellPosition) + new Vector3(0.5f, 0.5f);
                MouseIndicator.transform.position = placePosition;

                SpriteRenderer sr = MouseIndicator.GetComponent<SpriteRenderer>();
                // ����ܽ�������������Ϊ��ɫ������Ϊ��ɫ
                if (MapManager.Instance.ContainsObstaclesList(MouseIndicator))
                {
                    sr.color = Color.green;
                    // ����
                    if (Input.GetMouseButtonDown(0))
                    {
                        TryPlaced(placePosition);
                    }
                }
                else
                {
                    sr.color = Color.red;
                }
                // ������Q����������ʱ����ת90��
                if (Input.GetKeyDown(KeyCode.E))
                {
                    MouseIndicator.transform.Rotate(Vector3.forward, -90f);
                }
                // ������Q��������˳ʱ����ת90��
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    MouseIndicator.transform.Rotate(Vector3.forward, 90f);
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

        protected void TryPlaced(Vector3 placePosition)
        {
            GameObject newObject = UnityEngine.Object.Instantiate(CurBuildingDef.Prefab,
                                                      placePosition,
                                                      MouseIndicator.transform.rotation,
                                                      BuildingSystemManager.Instance.transform);
            
            MapManager.Instance.AddToObstaclesList(newObject);
            ThingBase Thing = newObject.GetComponent<ThingBase>();
            Thing.OnPlaced();
            //Todo
        }
    }
}