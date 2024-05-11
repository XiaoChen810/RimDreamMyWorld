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
    public SelectTool SelectTool { get; private set; }
    public PawnGeneratorTool PawnGeneratorTool { get; private set; }
    public AnimatorTool AnimatorTool { get; private set; }
    public WorkSpaceTool WorkSpaceTool { get; private set; }

    [SerializeField] private List<GameObject> _pawnsList = new List<GameObject>();
    /// <summary>
    /// ��Ϸ��ȫ����Pawn��GameObject�б�
    /// </summary>
    public List<GameObject> PawnsList   
    {
        get { return _pawnsList; }
    }

    [SerializeField] private List<Pawn> _pawnWhenStartList = new List<Pawn>();
    /// <summary>
    /// ������������ѡ��ʱʹ�õĽ�ɫ�б�
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
        SelectTool = GetComponent<SelectTool>();
        PawnGeneratorTool = new PawnGeneratorTool(this);
        AnimatorTool = GetComponent<AnimatorTool>();
        WorkSpaceTool = GetComponent<WorkSpaceTool>();
    }

#if UNITY_EDITOR

    public void ����һ������()
    {

    }

    public void �˻ؿ�ʼ����()
    {
        MapManager.Instance.CloseSceneMap(MapManager.Instance.CurrentMapName);
        SceneSystem.Instance.SetScene(new StartScene());
    }

#endif
}
