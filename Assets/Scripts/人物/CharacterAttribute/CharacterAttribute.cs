using UnityEngine;

/// <summary>
/// 人物的属性类
/// </summary>
public class CharacterAttribute
{
    /// <summary>
    ///  有关能力属性的部分
    /// </summary>
    public Hobby Combat;        // 战斗能力
    public Hobby Culinary;      // 烹饪能力
    public Hobby Construction;  // 建造能力
    public Hobby Survival;      // 生存能力
    public Hobby Craftsmanship; // 工艺能力
    public Hobby Medical;       // 医护能力
    public Hobby Carrying;      // 搬运能力
    public Hobby Research;      // 科研能力

    public CharacterAttribute()
    {
        UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
        Combat = new Hobby(Random.Range(0, 3), Random.Range(0, 20));
        Culinary = new Hobby(Random.Range(0, 3), Random.Range(0, 20));
        Construction = new Hobby(Random.Range(0, 3), Random.Range(0, 20));
        Survival = new Hobby(Random.Range(0, 3), Random.Range(0, 20));
        Craftsmanship = new Hobby(Random.Range(0, 3), Random.Range(0, 20));
        Medical = new Hobby(Random.Range(0, 3), Random.Range(0, 20));
        Carrying = new Hobby(Random.Range(0, 3), Random.Range(0, 20));
        Research = new Hobby(Random.Range(0, 3), Random.Range(0, 20));
    }
}

/// <summary>
/// 人物的爱好
/// </summary>
public class Hobby
{
    /// <summary>
    /// 爱好的级别，无0，有兴趣1，喜欢2，最爱3
    /// </summary>
    private int _lever;
    public int Level
    {
        get { return _lever; }
        set
        {
            _lever = Mathf.Clamp(value, 0, 3);
        }
    }

    /// <summary>
    /// 爱好的学习进度，一个大于0的值
    /// </summary>
    private float _value;
    public float Value
    {
        get { return _value; }
        set
        {
            _value = value > 0 ? value : 0;
        }
    }

    public Hobby(int lever,int value)
    {
        _lever = lever;
        _value = value;
    }
}
