using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBuildingSystem;

/// <summary>
/// 默认蓝图，只有放置和建造的功能，不具备具体的逻辑
/// </summary>
public class DefaultBlueprint : BlueprintBase
{
    protected override void OnEnable()
    {
        base.OnEnable();
        Debug.Log("当前使用的是默认设置");
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
