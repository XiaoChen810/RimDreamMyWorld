using UnityEngine;

/// <summary>
/// 人物的属性类
/// </summary>
public class CharacterAttribute
{
    /// <summary>
    /// 战斗能力
    /// </summary>
    public Ability A_Combat;
    /// <summary>
    /// 烹饪能力
    /// </summary>
    public Ability A_Culinary;
    /// <summary>
    /// 建造能力
    /// </summary>
    public Ability A_Construction;
    /// <summary>
    /// 生存能力
    /// </summary>
    public Ability A_Survival;
    /// <summary>
    /// 工艺能力
    /// </summary>
    public Ability A_Craftsmanship;
    /// <summary>
    /// 医护能力
    /// </summary>
    public Ability A_Medical;
    /// <summary>
    /// 搬运能力
    /// </summary>
    public Ability A_Carrying;
    /// <summary>
    /// 科研能力
    /// </summary>
    public Ability A_Research;      

    /// <summary>
    /// 初始化人物能力，随机分配
    /// </summary>
    public CharacterAttribute()
    {
        UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
        A_Combat = new Ability(Random.Range(0, 3), Random.Range(0, 20));
        A_Culinary = new Ability(Random.Range(0, 3), Random.Range(0, 20));
        A_Construction = new Ability(Random.Range(0, 3), Random.Range(0, 20));
        A_Survival = new Ability(Random.Range(0, 3), Random.Range(0, 20));
        A_Craftsmanship = new Ability(Random.Range(0, 3), Random.Range(0, 20));
        A_Medical = new Ability(Random.Range(0, 3), Random.Range(0, 20));
        A_Carrying = new Ability(Random.Range(0, 3), Random.Range(0, 20));
        A_Research = new Ability(Random.Range(0, 3), Random.Range(0, 20));
    }

    /// <summary>
    /// 人物能力值成长的算法
    /// </summary>
    /// <param name="baseValue"></param>
    /// <param name="ablility"> 要改变的能力 </param>
    public void Study(float baseValue, Ability ablility)
    {
        ablility.EXP += baseValue * (0.5f + ablility.Interest);
    }
}

/// <summary>
/// 人物的某种能力
/// </summary>
public class Ability
{
    /// <summary>
    /// 能力的级别，无0，有兴趣1，喜欢2，最爱3
    /// </summary>
    private int _interest;
    public int Interest
    {
        get { return _interest; }
        set
        {
            _interest = Mathf.Clamp(value, 0, 3);
        }
    }

    /// <summary>
    /// 能力的经验值，一个大于0的值
    /// </summary>
    private float _exp;
    public float EXP
    {
        get { return _exp; }
        set
        {
            _exp = value > 0 ? value : 0;
        }
    }

    public Ability(int interest, int exp)
    {
        _interest = interest;
        _exp = exp;
    }
}
