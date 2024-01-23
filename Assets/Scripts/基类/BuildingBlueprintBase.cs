using UnityEngine;

[System.Serializable]
public abstract class BuildingBlueprintBase : MonoBehaviour, IBlueprint
{
    // 实现接口中定义的属性和方法
    public BlueprintData BlueprintData { get; set; }

    public abstract void Placed();
    public abstract void Complete();
    public abstract void Build(int thisWorkload);
    public abstract void Cancel();
}


