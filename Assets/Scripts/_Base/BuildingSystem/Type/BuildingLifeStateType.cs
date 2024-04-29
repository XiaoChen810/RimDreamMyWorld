using System;
using UnityEngine;

namespace ChenChen_BuildingSystem
{
    public enum BuildingLifeStateType : Byte
    {
        None,
        //�ȴ�����
        MarkBuilding,
        //��ɽ���
        FinishedBuilding,
        //�ȴ����
        MarkDemolished,
        //��ɲ��
        FinishedDemolished
    }
}