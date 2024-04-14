using System;
using UnityEngine;

namespace ChenChen_BuildingSystem
{
    public enum BuildingStateType : Byte
    {
        None,
        //�ȴ�����
        WaitingBuilt,
        //��ɽ���
        FinishedBuilding,
        //�ȴ����
        WaitingDemolished
    }
}