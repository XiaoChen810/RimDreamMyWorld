using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���϶���
/// </summary>
[CreateAssetMenu(menuName = "����/����", fileName = "StuffDef", order = 3)]
public class StuffDef : ScriptableObject
{
    [Header("����")]
    public string Name;
    [Header("����")]
    public string Description;
    [Header("ͼƬ")]
    public Sprite Icon;
}
