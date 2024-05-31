using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChenChen_Map;
using ChenChen_Scene;
using System;

public class GameManager : SingletonMono<GameManager>
{
    public PawnGeneratorTool PawnGeneratorTool { get; private set; }
    public SelectTool SelectTool { get; private set; }
    public AnimatorTool AnimatorTool { get; private set; }
    public WorkSpaceTool WorkSpaceTool { get; private set; }
    public AnimalGenerateTool AnimalGenerateTool { get; private set; }
    public MonsterGeneratorTool MonsterGeneratorTool { get; private set; }
    public TechnologyTool TechnologyTool { get; private set; }

    private bool _gameIsStart = false;
    public bool GameIsStart { get { return _gameIsStart; } }

    public event Action OnGameStart;

    private bool _cinematicMode = false;
    public bool CinematicMode { get { return _cinematicMode; } }  // 电影模式

    [Header("Time")]
    public int currentSeason = 1; // 当前季节，1为春季，2为夏季，3为秋季，4为冬季
    public int currentDay = 1; // 当前天数，每季度15天
    public int currentHour = 0; // 当前小时，24小时制
    public int currentMinute = 0; // 当前分钟
    public float secondsPerGameMinute = 0.7f; // 游戏中的每分钟等于现实中的秒数
    public event Action OnTimeAddOneMinute;   //当时间加了一分钟
    private Coroutine timeCoroutine; // 协程引用

    // 当前游戏时间
    public string CurrentTime
    {
        get
        {
            return string.Format("Season {0} | Day {1}\n {2:00}:{3:00}", currentSeason, currentDay, currentHour, currentMinute);
        }
    }

    public void InitGameTime(int season, int day, int hour, int minute)
    {
        currentSeason = season;
        currentDay = day;
        currentHour = hour;
        currentMinute = minute;
    }

    protected override void Awake()
    {
        base.Awake();
        PawnGeneratorTool = GetComponent<PawnGeneratorTool>();
        SelectTool = GetComponent<SelectTool>();
        AnimatorTool = GetComponent<AnimatorTool>();
        WorkSpaceTool = GetComponent<WorkSpaceTool>();
        AnimalGenerateTool = GetComponent<AnimalGenerateTool>();
        MonsterGeneratorTool = GetComponent<MonsterGeneratorTool>();
        TechnologyTool = GetComponent<TechnologyTool>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F11))
        {
            _cinematicMode = !_cinematicMode;
        }
    }

    public void StartGame()
    {
        if(GameIsStart)
        {
            Debug.LogWarning("游戏只能开始一次");
        }
        //正式开始
        _gameIsStart = true;
        OnGameStart?.Invoke();
        //时间流逝
        timeCoroutine = StartCoroutine(TimeFlow());
        AnimalGenerateTool.CreateAllAnimalThree();
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void RecoverGame()
    {
        Time.timeScale = 1;
    }

    private IEnumerator TimeFlow()
    {
        while (true)
        {
            yield return new WaitForSeconds(secondsPerGameMinute); // 等待一段时间，表示游戏中的一分钟流逝

            currentMinute++;
            OnTimeAddOneMinute?.Invoke();

            // 如果当前分钟数大于等于60，则表示已经过了一小时
            if (currentMinute >= 60)
            {
                currentMinute = 0;
                currentHour++;

                // 如果当前小时数大于等于24，则表示已经过了一天
                if (currentHour >= 24)
                {
                    currentHour = 0;
                    currentDay++;

                    // 如果当前天数大于15，则表示已经过了一个季度
                    if (currentDay > 15)
                    {
                        currentDay = 1;
                        currentSeason = (currentSeason % 4) + 1; // 季节循环：1-春季，2-夏季，3-秋季，4-冬季
                    }
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (timeCoroutine != null)
        {
            StopCoroutine(timeCoroutine);
        }
    }

#if UNITY_EDITOR
    public GameObject Skeleton;
    public GameObject Goblin;

    public void 生成一个敌人()
    {
        Vector2 random = new Vector2(UnityEngine.Random.Range(0, MapManager.Instance.CurMapWidth), UnityEngine.Random.Range(0, MapManager.Instance.CurMapHeight));
        Instantiate(Goblin, random, Quaternion.identity, transform);
    }

    public void 退回开始场景()
    {
        MapManager.Instance.CloseSceneMap(MapManager.Instance.CurrentMapName);
        SceneSystem.Instance.SetScene(new StartScene());
    }

    public void 测试按钮()
    {
        Vector2 random = new Vector2(UnityEngine.Random.Range(0, MapManager.Instance.CurMapWidth), UnityEngine.Random.Range(0, MapManager.Instance.CurMapHeight));
        MonsterGeneratorTool.GenerateMonster(random, 0);
    }

#endif
}
