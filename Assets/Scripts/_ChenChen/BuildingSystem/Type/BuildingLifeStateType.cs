using System;
using UnityEngine;

namespace ChenChen_BuildingSystem
{
    [Flags]
    public enum BuildingLifeStateType : byte
    {
        None = 0,
        //�ȴ�����
        MarkBuilding = 1,
        //��ɽ���
        FinishedBuilding = 2,
        //�ȴ����
        MarkDemolished = 3,
        //��ɲ��
        FinishedDemolished = 4,
    }
}