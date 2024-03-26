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
    public class BuildingModeTool : MonoBehaviour
    {
        /// <summary>
        /// ����BuildingSystemManager
        /// </summary>
        [SerializeField] private BuildingSystemManager buildingSystemManager;

        /// <summary>
        ///  ��ǰ��ͼ�ķŽ��������Ƭ��ͼ
        /// </summary>
        [SerializeField] private Tilemap BuildingTilemap;

        /// <summary>
        ///  ��ǰ��ͼ�ķ�ǽ�����Ƭ��ͼ
        /// </summary>
        [SerializeField] private Tilemap WallTilemap;

        /// <summary>
        /// ��ǰ��ͼ������
        /// </summary>
        [SerializeField] private string CurBuildingName;

        /// <summary>
        /// ��ǰ����ͼ����
        /// </summary>
        [SerializeField] private BlueprintData blueprintData;

        /// <summary>
        /// ��ǰ����ϵ���ͼԤ��
        /// </summary>
        [SerializeField] private GameObject MouseIndicator;

        /// <summary>
        /// �Ƿ��ڽ���ģʽ��
        /// </summary>
        [SerializeField] private bool OnBuildMode;

        public BuildingModeTool()
        {
        }

        private void Start()
        {
            buildingSystemManager = GetComponent<BuildingSystemManager>();
        }

        private void Update()
        {
            BuildUpdate();
        }

        #region  Public

        /// <summary>
        /// ���� name �л���ͼ
        /// </summary>
        /// <param name="name"></param>
        public void ToggleBlueprint(string name)
        {
            BuildStart(name);
            CurBuildingName = name;
        }

        #endregion


        private void BuildStart(string name)
        {
            //  �ҵ���ǰӦ�÷��õ���ͼ
            blueprintData = BuildingSystemManager.Instance.GetData(name);
            if (blueprintData == null)
            {
                Debug.LogWarning($"��������ͼ: {name}, �ѷ���");
                OnBuildMode = false;
                return;
            }
            else
            {
                // ��ȡTileMap
                BuildingTilemap = MapManager.Instance.GetChildObject("Building").GetComponent<Tilemap>();
                WallTilemap = MapManager.Instance.GetChildObject("Wall").GetComponent<Tilemap>();

                // �������ָʾ����Ϣ
                OnBuildMode = true;
                MouseIndicator = UnityEngine.Object.Instantiate(blueprintData.Prefab);
                if (!MouseIndicator.GetComponent<BoxCollider2D>())
                {
                    Debug.LogError("ERROR");
                    return;
                }
                MouseIndicator.SetActive(true);

            }
        }

        /// <summary>
        /// �����Update�����������Ϣ��������ͼ��
        /// </summary>
        private void BuildUpdate()
        {
            if (OnBuildMode && MouseIndicator != null)
            {
                // �������λ����Ϣת������������
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int cellPosition = BuildingTilemap.WorldToCell(mousePosition);
                Vector3 placePosition = BuildingTilemap.CellToWorld(cellPosition) + new Vector3(0.5f, 0.5f);
                MouseIndicator.transform.position = placePosition;

                SpriteRenderer sr = MouseIndicator.GetComponent<SpriteRenderer>();
                // ����ܽ�������������Ϊ��ɫ������Ϊ��ɫ
                if (MapManager.Instance.CheckObjectWhetherCanPlaceOnHere(MouseIndicator))
                {
                    sr.color = Color.green;
                    // ����
                    if (Input.GetMouseButtonDown(0))
                    {
                        GameObject newObject = UnityEngine.Object.Instantiate(blueprintData.Prefab,
                                                                              placePosition,
                                                                              MouseIndicator.transform.rotation,
                                                                              buildingSystemManager.transform);
                        BlueprintBase blueprint = newObject.GetComponent<BlueprintBase>();
                        blueprint.Placed();
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

        private void BuildEnd()
        {
            OnBuildMode = false;
            MouseIndicator.SetActive(false);
            UnityEngine.Object.Destroy(MouseIndicator);
        }
    }
}