using UnityEngine;

/// <summary>
///  �������Թ���������Ψһ�����ԣ�����������ѧϰ�ķ���
/// </summary>
public class CharacterAttributeManager 
{
    public CharacterAttribute CA { get; private set; }

    public CharacterAttributeManager()
    {
        CA = new CharacterAttribute();
        
    }


    public void Study(float baseValue, Hobby hobby)
    {
        hobby.Value += baseValue * (0.5f + hobby.Level);
    }
}
