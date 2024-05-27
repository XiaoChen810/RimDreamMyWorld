using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Technology", menuName = "定义/科技", order = 4)]
public class TechnologyDef : ScriptableObject
{
    [Header("科技名")]
    public string TechnologyName;
    [Header("前置科技")]
    public List<string> TechnologyPreLoad;
}
