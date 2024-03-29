using System.Collections;
using System.Collections.Generic;
using ChenChen_UISystem;
using UnityEngine;
using ChenChen_MapGenerator;
using MyScene;

public class GameManager : SingletonMono<GameManager>
{
    public SceneSystem SceneSystem {  get; private set; }

    public PanelManager PanelManager { get; private set; }

    private bool isMenuPanelCreated;

    public List<GameObject> CharactersList;
    public List<GameObject> EnemyList;

    public GameObject CharacterTest;
    public GameObject GoblinPrefab;

    protected override void Awake()
    {
        base.Awake();
        Init();
    }

    private void Init()
    {
        SceneSystem = new();
        PanelManager = new PanelManager();
        CharactersList = new List<GameObject>();

        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        SceneSystem.SetScene(new StartScene());
    }

    private void Update()
    {
        OpenBuildingMenuPanel();
    }

    private void OpenBuildingMenuPanel()
    {
        if (Input.GetKeyDown(KeyCode.U) && !isMenuPanelCreated)
        {
            // �������OnEnterʱ�Ļص�����������isPanelCreatedΪtrue
            PanelBase.Callback onEnterCallback = () =>
            {
                isMenuPanelCreated = true;
            };

            // �������OnExitʱ�Ļص�����������isPanelCreatedΪfalse
            PanelBase.Callback onExitCallback = () =>
            {
                isMenuPanelCreated = false;
            };

            PanelManager.AddPanel(new BuildingMenuPanel(onEnterCallback, onExitCallback));
        }
    }

    public void ����һ������С��()
    {
        Vector3 createPos = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0);
        GameObject newCharacter = Instantiate(CharacterTest, createPos, Quaternion.identity);
        CharactersList.Add(newCharacter);   
    }

    public void ����Χ����һ������()
    {
        Vector3 createPos = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0);
        createPos += new Vector3(Random.Range(15, 20), Random.Range(15, 20));
        GameObject newEnemy = Instantiate(GoblinPrefab, createPos, Quaternion.identity);
        EnemyList.Add(newEnemy);
    }

    public void �˻ؿ�ʼ����()
    {
        MapManager.Instance.CloseSceneMap(MapManager.Instance.CurrentMapName);
        SceneSystem.SetScene(new StartScene());
    }
}
