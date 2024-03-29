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
            // 定义面板OnEnter时的回调函数，设置isPanelCreated为true
            PanelBase.Callback onEnterCallback = () =>
            {
                isMenuPanelCreated = true;
            };

            // 定义面板OnExit时的回调函数，重置isPanelCreated为false
            PanelBase.Callback onExitCallback = () =>
            {
                isMenuPanelCreated = false;
            };

            PanelManager.AddPanel(new BuildingMenuPanel(onEnterCallback, onExitCallback));
        }
    }

    public void 生成一个基础小人()
    {
        Vector3 createPos = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0);
        GameObject newCharacter = Instantiate(CharacterTest, createPos, Quaternion.identity);
        CharactersList.Add(newCharacter);   
    }

    public void 在周围生成一个敌人()
    {
        Vector3 createPos = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0);
        createPos += new Vector3(Random.Range(15, 20), Random.Range(15, 20));
        GameObject newEnemy = Instantiate(GoblinPrefab, createPos, Quaternion.identity);
        EnemyList.Add(newEnemy);
    }

    public void 退回开始场景()
    {
        MapManager.Instance.CloseSceneMap(MapManager.Instance.CurrentMapName);
        SceneSystem.SetScene(new StartScene());
    }
}
