using UnityEngine;

/// <summary>
///  人物属性管理，储存了唯一的属性，负责处理能力学习的方法
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
