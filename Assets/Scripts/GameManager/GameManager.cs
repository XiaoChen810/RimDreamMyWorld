using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChenChen_Map;
using System;
using ChenChen_Core;

public class GameManager : SingletonMono<GameManager>
{
    public PawnGeneratorTool PawnGeneratorTool { get; private set; }
    public SelectTool SelectTool { get; private set; }
    public AnimatorTool AnimatorTool { get; private set; }
    public WorkSpaceTool WorkSpaceTool { get; private set; }
    public AnimalGenerateTool AnimalGenerateTool { get; private set; }
    public TechnologyTool TechnologyTool { get; private set; }

    private bool _gameIsStart = false;
    private bool _f_mode_cineme = false;
    private bool _f_mode_water = false;

    public bool GameIsStart { get { return _gameIsStart; } }
    public bool Mode_Cineme { get { return _f_mode_cineme; } } 
    public bool Mode_Water { get { return _f_mode_water; } }

    public event Action OnGameStart;

    [Header("Time")]
    public int currentSeason = 1; // 当前季节，1为春季，2为夏季，3为秋季，4为冬季
    public int currentDay = 1; // 当前天数，每季度15天
    public int currentHour = 0; // 当前小时，24小时制
    public int currentMinute = 0; // 当前分钟
    public float secondsPerGameMinute = 0.7f; // 游戏中的每分钟等于现实中的秒数
    public event Action OnTimeAddOneMinute;   //当时间加了一分钟
    [SerializeField] private Vector2 dayLine;
    public bool IsDayTime => currentHour >= dayLine.x && currentHour < dayLine.y;

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

    public static readonly string PLAYER_FACTION = "Player";
    public static readonly string ENEMY_FACTION = "Enemy";
    public static readonly string PLATER_ARMY = "Army";

    public bool IsGodMode = false;

    protected override void Awake()
    {
        base.Awake();
        PawnGeneratorTool = GetComponent<PawnGeneratorTool>();
        SelectTool = GetComponent<SelectTool>();
        AnimatorTool = GetComponent<AnimatorTool>();
        WorkSpaceTool = GetComponent<WorkSpaceTool>();
        AnimalGenerateTool = GetComponent<AnimalGenerateTool>();
        TechnologyTool = GetComponent<TechnologyTool>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            _f_mode_water = !_f_mode_water;
        }
        if (Input.GetKeyDown(KeyCode.F11))
        {
            _f_mode_cineme = !_f_mode_cineme;
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            GameManager.Instance.WorkSpaceTool.SetNewWorkSpace_Storage();
        }
    }

    public void StartGame()
    {
        if(GameIsStart)
        {
            Debug.LogWarning("游戏已经开始");
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

#if UNITY_EDITOR

    public void 测试按钮()
    {

    }

#endif

    #region - Debug -

    public void Debug_raid()
    {
        MapManager mapManager = MapManager.Instance;
        // 地图中心
        Vector2Int center = new Vector2Int(mapManager.CurMapWidth / 2, mapManager.CurMapHeight / 2);

        // 生成范围
        int radius = 10;
        // 生成数量
        int numbers = 10;

        // 选择方位
        Vector2Int direction = Vector2Int.zero;

        // 例如选择向东
        direction = new Vector2Int(1, 0);

        for (int i = 0; i < numbers; i++)
        {
            // 生成随机偏移
            Vector2Int randomPosition = center + direction * 125;
            randomPosition += new Vector2Int(UnityEngine.Random.Range(-radius, radius), UnityEngine.Random.Range(-radius, radius));

            // 确保生成的位置在地图边界内
            randomPosition.x = Mathf.Clamp(randomPosition.x, 0, mapManager.CurMapWidth - 1);
            randomPosition.y = Mathf.Clamp(randomPosition.y, 0, mapManager.CurMapHeight - 1);

            // 生成Pawn
            var p = PawnGeneratorTool.GeneratePawn(position: new Vector3(randomPosition.x, randomPosition.y), faction: ENEMY_FACTION);
            p.SetWeapon(XmlLoader.Instance.GetRandom<WeaponDef>(XmlLoader.Def_Weapon));
        }
    }

    public void Debug_raid1()
    {
        MapManager mapManager = MapManager.Instance;

        Vector2Int center = new Vector2Int(mapManager.CurMapWidth / 2, mapManager.CurMapHeight / 2);

        var p = PawnGeneratorTool.GeneratePawn(position: new Vector3(center.x, center.y), faction: "enemy");
        p.SetWeapon(XmlLoader.Instance.GetRandom<WeaponDef>(XmlLoader.Def_Weapon));
    }

    public void Debug_night()
    {
        currentHour = (int)dayLine.y;
    }

    public void Debug_dayLight()
    {
        currentHour = (int)dayLine.x;
    }

    public void Debug_cinema()
    {
        _f_mode_cineme = !_f_mode_cineme;
    }

    public void Debug_godmode()
    {
        IsGodMode = !IsGodMode;
    }

    public void Debug_colony()
    {
        MapManager mapManager = MapManager.Instance;
        // 地图中心
        Vector2Int center = new Vector2Int(mapManager.CurMapWidth / 2, mapManager.CurMapHeight / 2);
        // 生成范围
        int radius = 10;
        // 生成数量
        int numbers = 10;

        Vector2Int direction = new Vector2Int(1, 0);

        for (int i = 0; i < numbers; i++)
        {
            // 生成随机偏移
            Vector2Int randomPosition = center + direction * 70;
            randomPosition += new Vector2Int(UnityEngine.Random.Range(-radius, radius), UnityEngine.Random.Range(-radius, radius));

            // 确保生成的位置在地图边界内
            randomPosition.x = Mathf.Clamp(randomPosition.x, 0, mapManager.CurMapWidth - 1);
            randomPosition.y = Mathf.Clamp(randomPosition.y, 0, mapManager.CurMapHeight - 1);

            // 生成Pawn
            var p = PawnGeneratorTool.GeneratePawn(position: new Vector3(randomPosition.x, randomPosition.y),faction: PLATER_ARMY);
            p.SetWeapon(XmlLoader.Instance.GetRandom<WeaponDef>(XmlLoader.Def_Weapon));
        }
    }

    public void Debug_battle()
    {
        Debug_raid();
        Debug_colony();
    }
    #endregion
}
