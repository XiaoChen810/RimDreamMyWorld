using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Technology", menuName = "����/�Ƽ�", order = 4)]
public class TechnologyDef : ScriptableObject
{
    [Header("�Ƽ���")]
    public string TechnologyName;
    [Header("ǰ�ÿƼ�")]
    public List<string> TechnologyPreLoad;
}
