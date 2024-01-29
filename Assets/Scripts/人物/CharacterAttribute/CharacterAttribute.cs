using UnityEngine;

/// <summary>
/// �����������
/// </summary>
public class CharacterAttribute
{
    /// <summary>
    ///  �й��������ԵĲ���
    /// </summary>
    public Hobby Combat;        // ս������
    public Hobby Culinary;      // �������
    public Hobby Construction;  // ��������
    public Hobby Survival;      // ��������
    public Hobby Craftsmanship; // ��������
    public Hobby Medical;       // ҽ������
    public Hobby Carrying;      // ��������
    public Hobby Research;      // ��������

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
/// ����İ���
/// </summary>
public class Hobby
{
    /// <summary>
    /// ���õļ�����0������Ȥ1��ϲ��2���3
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
    /// ���õ�ѧϰ���ȣ�һ������0��ֵ
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
