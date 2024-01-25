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
        [Header("��Ƭ��ͼ")]
        public Tilemap BuildingTilemap;
        public Tilemap WallTilemap;

        [Header("��ǰ�������Ʒ����")]
        public string BuildingName;
        private BlueprintData blueprintData;

        [Header("��������ģʽ")]
        public bool BuildingMode;
        private bool isOpenBuilding;

        [Header("���ָʾ��")]
        public GameObject MouseIndicator;

        Action<Vector3> OnPlace, OnCancel;


        private void Update()
        {
            // ������ģʽ���ش�ʱ��ֻ����һ�λ�ȡ��ͼ���ݺ����������Ϣ
            if (BuildingMode && !isOpenBuilding)
            {
                isOpenBuilding = true;

                //  �ҵ���ǰӦ�÷��õ���ͼ
                blueprintData = BuildingSystemManager.Instance.GetData(BuildingName);
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
                // �������λ����Ϣת������������
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int cellPosition = BuildingTilemap.WorldToCell(mousePosition);
                Vector3 placePosition = BuildingTilemap.CellToWorld(cellPosition) + new Vector3(0.5f, 0.5f);
                MouseIndicator.transform.position = cellPosition + new Vector3(0.5f, 0.5f);

                // ���ú�ȡ��
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