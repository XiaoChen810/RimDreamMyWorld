using UnityEngine;
using System.Collections.Generic;
using ChenChen_CropSystem;


public class WorkSpace_Farm : WorkSpace
{
    [System.Serializable]
    public class Cell
    {
        public Vector2 pos;
        public bool isUse;
        public bool isFarm;
        public Crop Crop;
    }
    /// <summary>
    /// 包含的所有网格
    /// </summary>
    public Dictionary<Vector2,Cell> Cells = new Dictionary<Vector2,Cell>();

    [SerializeField] protected CropDef _whatCrop;
    public CropDef CurCrop
    {
        get { return _whatCrop; }
    }

    /// <summary>
    /// 当工作区大小改变后
    /// </summary>
    protected override void AfterSizeChange()
    {
        Cells.Clear();
        Bounds bounds = SR.bounds;

        // 遍历所有整数点，创建并添加到_cells列表中
        for (float x = Mathf.Floor(bounds.min.x); x < Mathf.Ceil(bounds.max.x); x++)
        {
            for (float y = Mathf.Floor(bounds.min.y); y < Mathf.Ceil(bounds.max.y); y++)
            {
                Vector2 cellPosition = new Vector2Int((int)x, (int)y) + new Vector2(0.5f, 0.5f);
                Cells.Add(cellPosition, new Cell { pos = cellPosition, isUse = false, isFarm = false });
            }
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    public void Init(string cropName)
    {
        WorkSpaceType = WorkSpaceType.Farm;
        _whatCrop = CropManager.Instance.GetCropDef(cropName);
    }

    /// <summary>
    /// 尝试获取一个种植的位置
    /// </summary>
    /// <returns></returns>
    public bool TryGetAFarmingPosition(out Vector2 farmingPositon)
    {
        foreach(var cell in Cells)
        {
            if(!cell.Value.isUse && !cell.Value.isFarm)
            {
                farmingPositon = cell.Key;
                cell.Value.isUse = true;
                return true;
            }
        }
        Debug.Log($"该种植区{name}已经满了，权限设为IsBooking");
        farmingPositon = Vector2.zero;
        _permission = PermissionType.IsBooking;                                
        return false;
    }

    /// <summary>
    /// 尝试在这个位置种植
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public bool TrySetAPositionHadFarmed(Vector2 position)
    {
        if (Cells.ContainsKey(position))
        {
            Cell cell = Cells[position];
            if (cell.pos == position && cell.isUse && !cell.isFarm)
            {
                GameObject cropObj = new GameObject(_whatCrop.CropName);
                cropObj.transform.position = position;
                cropObj.transform.SetParent(transform, true);
                Crop crop = cropObj.AddComponent<Crop>();
                crop.Init(_whatCrop, this);

                cell.isFarm = true;
                cell.isUse = false;
                cell.Crop = crop;
                return true;
            }
        }
        Debug.LogWarning("Warning");
        return false;
    }

    /// <summary>
    /// 在这个位置种植，一般是从存档中直接赋值
    /// </summary>
    /// <param name="data_CropSave"></param>
    /// <returns></returns>
    public void SetAPositionHadFarmed(Data_CropSave data_CropSave)
    {
        if (Cells.ContainsKey(data_CropSave.position))
        {
            Cell cell = Cells[data_CropSave.position];

            GameObject cropObj = new GameObject(_whatCrop.CropName);
            cropObj.transform.position = data_CropSave.position;
            cropObj.transform.SetParent(transform, true);
            Crop crop = cropObj.AddComponent<Crop>();
            crop.Init(_whatCrop, this, data_CropSave);

            cell.isFarm = true;
            cell.isUse = false;
            cell.Crop = crop;
            return;

        }
        Debug.LogWarning("Warning");
    }

    /// <summary>
    /// 返还一个位置，可能是中断种植或者收获作物时使用
    /// </summary>
    /// <param name="position"></param>
    public void ReturnAPosition(Vector2 position)
    {
        if (Cells.ContainsKey(position))
        {
            Cell cell = Cells[position];
            if (cell.pos == position && cell.isUse && !cell.isFarm)
            {
                cell.isUse = false;
                _permission = PermissionType.IsFree;
            }
        }
    }
}

