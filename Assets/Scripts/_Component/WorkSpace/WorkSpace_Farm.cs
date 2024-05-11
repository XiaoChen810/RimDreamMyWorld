using UnityEngine;
using System.Collections.Generic;
using ChenChen_CropSystem;


public class WorkSpace_Farm : WorkSpace
{
    [System.Serializable]
    protected class Cell
    {
        public Vector2 pos;
        public bool isUse;
        public bool isFarm;
        public Crop Crop;
    }
    /// <summary>
    /// 包含的所有网格
    /// </summary>
    //protected List<Cell> _cells = new();
    protected Dictionary<Vector2,Cell> _cells = new Dictionary<Vector2,Cell>();

    [SerializeField] protected CropDef _whatCrop;
    public CropDef CurCrop
    {
        get { return _whatCrop; }
    }

    protected override void AfterSizeChange()
    {
        _cells.Clear();
        Bounds bounds = SR.bounds;

        // 遍历所有整数点，创建并添加到_cells列表中
        for (float x = Mathf.Floor(bounds.min.x); x < Mathf.Ceil(bounds.max.x); x++)
        {
            for (float y = Mathf.Floor(bounds.min.y); y < Mathf.Ceil(bounds.max.y); y++)
            {
                Vector2 cellPosition = new Vector2Int((int)x, (int)y) + new Vector2(0.5f, 0.5f);
                _cells.Add(cellPosition, new Cell { pos = cellPosition, isUse = false, isFarm = false });
            }
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _whatCrop = CropManager.Instance.GetCropDef("小麦");
    }

    /// <summary>
    /// 尝试获取一个种植的位置
    /// </summary>
    /// <returns></returns>
    public bool TryGetAFarmingPosition(out Vector2 farmingPositon)
    {
        foreach(var cell in _cells)
        {
            if(!cell.Value.isUse && !cell.Value.isFarm)
            {
                farmingPositon = cell.Key;
                cell.Value.isUse = true;
                return true;
            }
        }
        Debug.LogWarning("No Position Can Farm");
        farmingPositon = Vector2.zero;
        return false;
    }

    /// <summary>
    /// 尝试在这个位置种植
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public bool TrySetAPositionHadFarmed(Vector2 position)
    {
        if (_cells.ContainsKey(position))
        {
            Cell cell = _cells[position];
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

    public void ReturnAPosition(Vector2 position)
    {
        if (_cells.ContainsKey(position))
        {
            Cell cell = _cells[position];
            if (cell.pos == position && cell.isUse && !cell.isFarm)
            {
                cell.isUse = false;
            }
        }
    }
}

