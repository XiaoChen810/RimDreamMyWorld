using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using ��ͼ����;

namespace ����ϵͳ
{
    public class BuildingSystem : MonoBehaviour
    {
        // ��Ƭ��ͼ������������õ�
        [HideInInspector] public Tilemap BuildingTilemap;
        [HideInInspector] public Tilemap WallTilemap;

        [Header("��ǰ�������Ʒ����")]
        public string BuildingName;
        private BlueprintData blueprintData;

        [Header("��������ģʽ")]
        public bool BuildingMode;
        /// <summary>
        ///  ���л���ǰ��ͼ����ʱ���õ��¼�
        /// </summary>
        public Action<string> OnToggleBlueprint;

        [Header("���ָʾ��")]
        public GameObject MouseIndicator;

        Action<Vector3> OnPlace, OnCancel;

        private void Start()
        {
            OnToggleBlueprint += BuildStart;
        }

        private void OnDestroy()
        {
            OnToggleBlueprint -= BuildStart;
        }

        private void Update()
        {
            // ������ģʽ���ش�ʱ������һ�λ�ȡ��ͼ���ݺ��������ָʾ����Ϣ
            if (BuildingMode)
            {
                BuildingMode = false;
                ToggleBlueprint(BuildingName);
            }

            BuildUpdate();


        }

        #region  Public

        public void ToggleBlueprint(string name)
        {
            OnToggleBlueprint?.Invoke(name);
        }

        #endregion


        private void BuildStart(string name)
        {
            //  �ҵ���ǰӦ�÷��õ���ͼ
            blueprintData = BuildingSystemManager.Instance.GetData(name);
            if (blueprintData == null)
            {
                Debug.LogWarning("�����ڴ���ͼ");
                BuildEnd();
                return;
            }
            else
            {
                // ��ȡTileMap
                BuildingTilemap = MapManager.Instance.GetChildObject("Building").GetComponent<Tilemap>();
                WallTilemap = MapManager.Instance.GetChildObject("Wall").GetComponent<Tilemap>();

                // �������ָʾ����Ϣ
                MouseIndicator.SetActive(true);
                SpriteRenderer sr = MouseIndicator.GetComponent<SpriteRenderer>();
                sr.sprite = blueprintData.PreviewSprite;
            }
        }

        private void BuildUpdate()
        {
            if (MouseIndicator.activeSelf)
            {
                // �������λ����Ϣת������������
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int cellPosition = BuildingTilemap.WorldToCell(mousePosition);
                Vector3 placePosition = BuildingTilemap.CellToWorld(cellPosition) + new Vector3(0.5f, 0.5f);
                MouseIndicator.transform.position = cellPosition + new Vector3(0.5f, 0.5f);

                // ���ú�ȡ��
                if (Input.GetMouseButtonDown(0))
                {
                    OnPlace?.Invoke(cellPosition);
                    GameObject newObject = Instantiate(blueprintData.Prefab, placePosition, Quaternion.identity, this.transform);                   
                    BuildingBlueprintBase blueprint = newObject.GetComponent<BuildingBlueprintBase>();
                    if(blueprint.Name != blueprintData.name)
                    {
                        Debug.LogWarning($"������ͼ��������ͼ������ͻ {blueprint.Name} : {blueprintData.name}");
                    }
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
            MouseIndicator.SetActive(false);
        }
    }
}