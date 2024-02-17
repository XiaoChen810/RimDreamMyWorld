using System.Collections;
using System.Collections.Generic;
using MyUISystem;
using UnityEngine;
using MyMapGenerate;
using MyScene;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public SceneSystem SceneSystem {  get; private set; }

    public PanelManager PanelManager { get; private set; }

    private bool isMenuPanelCreated;

    public List<GameObject> CharactersList;
    public List<GameObject> EnemyList;

    public GameObject CharacterTest;
    public GameObject GoblinPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            Init();
        }
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
        Vector3 createPos = new Vector3(20f, 15f, 0f);
        GameObject newCharacter = Instantiate(CharacterTest, createPos, Quaternion.identity);
        CharactersList.Add(newCharacter);   
    }

    public void 在周围生成一个敌人()
    {
        Vector3 createPos = new Vector3(20f + Random.Range(15, 20), 15f + Random.Range(5, 10), 0f);
        GameObject newEnemy = Instantiate(GoblinPrefab, createPos, Quaternion.identity);
        EnemyList.Add(newEnemy);
    }

    public void 退回开始场景()
    {
        MapManager.Instance.CloseSceneMap(MapManager.Instance.CurrentMapName);
        SceneSystem.SetScene(new StartScene());
    }
}
