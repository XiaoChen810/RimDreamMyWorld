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
    public bool CinematicMode { get { return _cinematicMode; } }  // ��Ӱģʽ

    [Header("Time")]
    public int currentSeason = 1; // ��ǰ���ڣ�1Ϊ������2Ϊ�ļ���3Ϊ�＾��4Ϊ����
    public int currentDay = 1; // ��ǰ������ÿ����15��
    public int currentHour = 0; // ��ǰСʱ��24Сʱ��
    public int currentMinute = 0; // ��ǰ����
    public float secondsPerGameMinute = 0.7f; // ��Ϸ�е�ÿ���ӵ�����ʵ�е�����
    public event Action OnTimeAddOneMinute;   //��ʱ�����һ����
    private Coroutine timeCoroutine; // Э������

    // ��ǰ��Ϸʱ��
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
            Debug.LogWarning("��Ϸֻ�ܿ�ʼһ��");
        }
        //��ʽ��ʼ
        _gameIsStart = true;
        OnGameStart?.Invoke();
        //ʱ������
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
            yield return new WaitForSeconds(secondsPerGameMinute); // �ȴ�һ��ʱ�䣬��ʾ��Ϸ�е�һ��������

            currentMinute++;
            OnTimeAddOneMinute?.Invoke();

            // �����ǰ���������ڵ���60�����ʾ�Ѿ�����һСʱ
            if (currentMinute >= 60)
            {
                currentMinute = 0;
                currentHour++;

                // �����ǰСʱ�����ڵ���24�����ʾ�Ѿ�����һ��
                if (currentHour >= 24)
                {
                    currentHour = 0;
                    currentDay++;

                    // �����ǰ��������15�����ʾ�Ѿ�����һ������
                    if (currentDay > 15)
                    {
                        currentDay = 1;
                        currentSeason = (currentSeason % 4) + 1; // ����ѭ����1-������2-�ļ���3-�＾��4-����
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

    public void ����һ������()
    {
        Vector2 random = new Vector2(UnityEngine.Random.Range(0, MapManager.Instance.CurMapWidth), UnityEngine.Random.Range(0, MapManager.Instance.CurMapHeight));
        Instantiate(Goblin, random, Quaternion.identity, transform);
    }

    public void �˻ؿ�ʼ����()
    {
        MapManager.Instance.CloseSceneMap(MapManager.Instance.CurrentMapName);
        SceneSystem.Instance.SetScene(new StartScene());
    }

    public void ���԰�ť()
    {
        Vector2 random = new Vector2(UnityEngine.Random.Range(0, MapManager.Instance.CurMapWidth), UnityEngine.Random.Range(0, MapManager.Instance.CurMapHeight));
        MonsterGeneratorTool.GenerateMonster(random, 0);
    }

#endif
}
