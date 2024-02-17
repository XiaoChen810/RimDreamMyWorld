using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using MyMapGenerate;

namespace MyBuildingSystem
{
    /// <summary>
    /// �򿪽���ģʽ�����ý�����ͼʲô��
    /// </summary>
    public class BlueprintTool
    {
        /// <summary>
        /// �����Ľ���ϵͳ������
        /// </summary>
        public BuildingSystemManager BSM { get; private set; }

        /// <summary>
        ///  ��ǰ��ͼ�ķŽ��������Ƭ��ͼ
        /// </summary>
        public Tilemap BuildingTilemap { get; private set; }

        /// <summary>
        ///  ��ǰ��ͼ�ķ�ǽ�����Ƭ��ͼ
        /// </summary>
        public Tilemap WallTilemap { get; private set; }

        /// <summary>
        /// ����������ʱ���õ��¼�
        /// </summary>
        public Action<string> OnToggle;

        /// <summary>
        /// ����������ʱ���õ��¼�
        /// </summary>
        public Action<Vector3> OnPlace, OnCancel;

        /// <summary>
        /// ��ǰ��ͼ������
        /// </summary>
        public string BlueprintName { get; private set; }

        /// <summary>
        /// ��ǰ����ͼ����
        /// </summary>
        public BlueprintData blueprintData { get; private set; }

        /// <summary>
        /// ��ǰ����ϵ���ͼԤ��
        /// </summary>
        public GameObject MouseIndicator { get; private set; }

        /// <summary>
        /// �Ƿ��ڽ���ģʽ��
        /// </summary>
        public bool OnBuildMode { get; private set; }

        public BlueprintTool(BuildingSystemManager buildingSystemManager)
        {
            this.BSM = buildingSystemManager;
            this.MouseIndicator = BSM.MouseIndicator;
        }

        #region  Public

        /// <summary>
        /// ���� name �л���ͼ
        /// </summary>
        /// <param name="name"></param>
        public void ToggleBlueprint(string name)
        {
            OnToggle?.Invoke(name);
            BuildStart(name);
        }

        /// <summary>
        /// �����Update�����������Ϣ��������ͼ��
        /// </summary>
        public void BuildUpdate()
        {
            if (OnBuildMode)
            {
                // �������λ����Ϣת������������
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int cellPosition = BuildingTilemap.WorldToCell(mousePosition);
                Vector3 worldPosition = BuildingTilemap.CellToWorld(cellPosition);
                Vector3 placePosition = worldPosition + new Vector3(0.5f, 0.5f);
                MouseIndicator.transform.position = cellPosition + new Vector3(0.5f, 0.5f);

                SpriteRenderer sr = MouseIndicator.GetComponent<SpriteRenderer>();
                // ����ܽ�������������Ϊ��ɫ������Ϊ��ɫ
                if (MapManager.Instance.CheckBlueprintCanPlaced(blueprintData, worldPosition))
                {
                    sr.color = Color.green;
                    // ����
                    if (Input.GetMouseButtonDown(0))
                    {
                        OnPlace?.Invoke(cellPosition);
                        GameObject newObject = UnityEngine.Object.Instantiate(blueprintData.Prefab, placePosition, Quaternion.identity, BSM.transform);
                        BlueprintBase blueprint = newObject.GetComponent<BlueprintBase>();
                        blueprint.Placed();
                    }
                }
                else
                {
                    sr.color = Color.red;
                }

                //ȡ��
                if (Input.GetMouseButtonDown(1))
                {
                    OnCancel?.Invoke(cellPosition);
                    BuildEnd();
                }
            }
        }

        #endregion


        private void BuildStart(string name)
        {
            //  �ҵ���ǰӦ�÷��õ���ͼ
            blueprintData = BuildingSystemManager.Instance.GetData(name);
            if (blueprintData == null)
            {
                Debug.LogWarning("��������ͼ, �ѷ���");
                BuildEnd();
                return;
            }
            else
            {
                // ��ȡTileMap
                BuildingTilemap = MapManager.Instance.GetChildObject("Building").GetComponent<Tilemap>();
                WallTilemap = MapManager.Instance.GetChildObject("Wall").GetComponent<Tilemap>();

                // �������ָʾ����Ϣ
                OnBuildMode = true;
                MouseIndicator.SetActive(true);
                SpriteRenderer sr = MouseIndicator.GetComponent<SpriteRenderer>();
                sr.sprite = blueprintData.PreviewSprite;

            }
        }

        private void BuildEnd()
        {
            OnBuildMode = false;
            MouseIndicator.SetActive(false);
        }
    }
}