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
    }
    /// <summary>
    /// 包含的所有网格
    /// </summary>
    [SerializeField] protected List<Cell> _cells = new();

    [SerializeField] protected CropDef _whatCrop;

    protected override void AfterSizeChange()
    {
        _cells.Clear();
        Bounds bounds = SR.bounds;

        // 遍历所有整数点，创建并添加到_cells列表中
        for (float x = Mathf.Floor(bounds.min.x); x < Mathf.Ceil(bounds.max.x); x++)
        {
            for (float y = Mathf.Floor(bounds.min.y); y < Mathf.Ceil(bounds.max.y); y++)
            {
                Vector2Int cellPosition = new Vector2Int((int)x, (int)y);
                _cells.Add(new Cell { pos = cellPosition + new Vector2(0.5f, 0.5f), isUse = false, isFarm = false });
            }
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _whatCrop = CropManager.Instance.GetCropDef("小麦");
    }

    /// <summary>
    /// 尝试获取一个种植的位置，随机的
    /// </summary>
    /// <returns></returns>
    public bool TryGetAFarmingPosition(out Vector2 farmingPositon)
    {
        List<Cell> cellsNoUse = new();
        foreach(Cell cell in _cells)
        {
            if (!cell.isUse && !cell.isFarm) cellsNoUse.Add(cell);
        }
        if (cellsNoUse.Count > 0)
        {
            int index = Random.Range(0, cellsNoUse.Count);
            farmingPositon = cellsNoUse[index].pos;
            cellsNoUse[index].isUse = true;
            return true;
        }
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
        foreach(Cell cell in _cells)
        {
            if(cell.pos == position && cell.isUse && !cell.isFarm)
            {
                GameObject cropObj = new GameObject(_whatCrop.CropName);
                cropObj.transform.position = position;
                cropObj.transform.SetParent(transform, true);
                Crop crop = cropObj.AddComponent<Crop>();
                crop.Init(_whatCrop, this);

                cell.isFarm = true;
                cell.isUse = false;
                Debug.Log($"{position} is Farm");
                return true;
            }
        }
        Debug.LogWarning("Warning");
        return false;
    }

}

