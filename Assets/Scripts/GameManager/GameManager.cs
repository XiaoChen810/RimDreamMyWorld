using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChenChen_UISystem;
using ChenChen_MapGenerator;
using ChenChen_Scene;
using System;
using ChenChen_AI;

public class GameManager : SingletonMono<GameManager>
{
    public PawnGeneratorTool PawnGeneratorTool { get; private set; }
    public SelectTool SelectTool { get; private set; }
    public AnimatorTool AnimatorTool { get; private set; }
    public WorkSpaceTool WorkSpaceTool { get; private set; }
    public AnimalGenerateTool AnimalGenerateTool { get; private set; }
    [SerializeField] private List<GameObject> _pawnsList = new List<GameObject>();
    /// <summary>
    /// 游戏内全部的Pawn的GameObject列表
    /// </summary>
    public List<GameObject> PawnsList   
    {
        get { return _pawnsList; }
    }

    [SerializeField] private List<Pawn> _pawnWhenStartList = new List<Pawn>();
    /// <summary>
    /// 仅当进行人物选择时使用的角色列表
    /// </summary>
    public List<Pawn> PawnWhenStartList
    {
        get { return _pawnWhenStartList; }
    }
    public GameObject CharacterTest;
    public GameObject GoblinPrefab;

    protected override void Awake()
    {
        base.Awake();
        Init();
    }

    private void Init()
    {
        PawnGeneratorTool = new PawnGeneratorTool(this);
        SelectTool = GetComponent<SelectTool>();
        AnimatorTool = GetComponent<AnimatorTool>();
        WorkSpaceTool = GetComponent<WorkSpaceTool>();
        AnimalGenerateTool = GetComponent<AnimalGenerateTool>();
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void RecoverGame()
    {
        Time.timeScale = 1;
    }

#if UNITY_EDITOR

    public void 生成一个敌人()
    {

    }

    public void 退回开始场景()
    {
        MapManager.Instance.CloseSceneMap(MapManager.Instance.CurrentMapName);
        SceneSystem.Instance.SetScene(new StartScene());
    }

    public void 测试按钮()
    {
        Vector2 random = new Vector2(UnityEngine.Random.Range(0, 50), UnityEngine.Random.Range(0, 50));
        AnimalGenerateTool.GenerateAnimal("绵羊", random);
    }

#endif
}
