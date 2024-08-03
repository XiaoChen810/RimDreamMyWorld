using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChenChen_Map;
using ChenChen_Scene;
using System;
using ChenChen_UI;

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
    private bool _cinematicMode = false;

    public bool GameIsStart { get { return _gameIsStart; } }
    public bool CinematicMode { get { return _cinematicMode; } } 

    public event Action OnGameStart;

    [Header("Time")]
    public int currentSeason = 1; // ��ǰ���ڣ�1Ϊ������2Ϊ�ļ���3Ϊ�＾��4Ϊ����
    public int currentDay = 1; // ��ǰ������ÿ����15��
    public int currentHour = 0; // ��ǰСʱ��24Сʱ��
    public int currentMinute = 0; // ��ǰ����
    public float secondsPerGameMinute = 0.7f; // ��Ϸ�е�ÿ���ӵ�����ʵ�е�����
    public event Action OnTimeAddOneMinute;   //��ʱ�����һ����
    [SerializeField] private Vector2 dayLine;
    public bool IsDayTime => currentHour >= dayLine.x && currentHour < dayLine.y;

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

    public static readonly string PLAYER_FACTION = "Player";

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
            Debug.LogWarning("��Ϸ�Ѿ���ʼ");
            return;
        }
        _gameIsStart = true;
        OnGameStart?.Invoke();
        StartCoroutine(TimeFlow());
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

#if UNITY_EDITOR

    public void ���԰�ť()
    {

    }

#endif

    #region - Debug -

    public void Debug_raid()
    {
        MapManager mapManager = MapManager.Instance;
        // ��ͼ����
        Vector2Int center = new Vector2Int(mapManager.CurMapWidth / 2, mapManager.CurMapHeight / 2);

        // ���ɷ�Χ
        int radius = 10;
        // ��������
        int numbers = 10;

        // ѡ��λ
        Vector2Int direction = Vector2Int.zero;

        // ����ѡ����
        direction = new Vector2Int(1, 0);

        for (int i = 0; i < numbers; i++)
        {
            // �������ƫ��
            Vector2Int randomPosition = center + direction * 125;
            randomPosition += new Vector2Int(UnityEngine.Random.Range(-radius, radius), UnityEngine.Random.Range(-radius, radius));

            // ȷ�����ɵ�λ���ڵ�ͼ�߽���
            randomPosition.x = Mathf.Clamp(randomPosition.x, 0, mapManager.CurMapWidth - 1);
            randomPosition.y = Mathf.Clamp(randomPosition.y, 0, mapManager.CurMapHeight - 1);

            // ����Pawn
            PawnGeneratorTool.GeneratePawn(position: new Vector3(randomPosition.x, randomPosition.y), faction: "enemy");
        }
    }

    public void Debug_night()
    {
        currentHour = (int)dayLine.y;
    }

    public void Debug_dayLight()
    {
        currentHour = (int)dayLine.x;
    }

    public void Debug_Cinema()
    {
        _cinematicMode = !_cinematicMode;
    }
    #endregion
}
