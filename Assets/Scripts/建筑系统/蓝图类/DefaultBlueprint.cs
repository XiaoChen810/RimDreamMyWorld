using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBuildingSystem;

/// <summary>
/// Ĭ����ͼ��ֻ�з��úͽ���Ĺ��ܣ����߱�������߼�
/// </summary>
public class DefaultBlueprint : BlueprintBase
{
    protected override void OnEnable()
    {
        base.OnEnable();
        Debug.Log("��ǰʹ�õ���Ĭ������");
    }

    public override void Build(float thisWorkload)
    {
        _workloadAlready -= thisWorkload;
    }

    public override void Cancel()
    {
        Destroy(this.gameObject);
        BuildingSystemManager.Instance.CanelTask(this);
    }

    public override void Complete()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.color = new Color(1, 1, 1, 1f);

        BuildingSystemManager.Instance.CompleteTask(this);
    }

    public override void Placed()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.color = new Color(1, 1, 1, 0.5f);

        BuildingSystemManager.Instance.AddTask(this);
    }
}
