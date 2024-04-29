using System;
using UnityEngine;

namespace ChenChen_BuildingSystem
{
    public enum BuildingLifeStateType : Byte
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