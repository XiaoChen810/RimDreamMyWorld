using System;
using UnityEngine;

namespace ChenChen_Thing
{
    public enum BuildingLifeStateType
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