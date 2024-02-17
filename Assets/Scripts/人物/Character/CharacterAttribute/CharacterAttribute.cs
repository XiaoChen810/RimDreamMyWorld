using UnityEngine;

/// <summary>
/// �����������
/// </summary>
public class CharacterAttribute
{
    /// <summary>
    /// ս������
    /// </summary>
    public Ability A_Combat;
    /// <summary>
    /// �������
    /// </summary>
    public Ability A_Culinary;
    /// <summary>
    /// ��������
    /// </summary>
    public Ability A_Construction;
    /// <summary>
    /// ��������
    /// </summary>
    public Ability A_Survival;
    /// <summary>
    /// ��������
    /// </summary>
    public Ability A_Craftsmanship;
    /// <summary>
    /// ҽ������
    /// </summary>
    public Ability A_Medical;
    /// <summary>
    /// ��������
    /// </summary>
    public Ability A_Carrying;
    /// <summary>
    /// ��������
    /// </summary>
    public Ability A_Research;      

    /// <summary>
    /// ��ʼ�������������������
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
    /// ��������ֵ�ɳ����㷨
    /// </summary>
    /// <param name="baseValue"></param>
    /// <param name="ablility"> Ҫ�ı������ </param>
    public void Study(float baseValue, Ability ablility)
    {
        ablility.EXP += baseValue * (0.5f + ablility.Interest);
    }
}

/// <summary>
/// �����ĳ������
/// </summary>
public class Ability
{
    /// <summary>
    /// �����ļ�����0������Ȥ1��ϲ��2���3
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
    /// �����ľ���ֵ��һ������0��ֵ
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
