using System;
using System.Collections.Generic;
using UnityEngine;


namespace ChenChen_Thing
{
    public class WorkSpace_Storage : WorkSpace
    {
        [System.Serializable]
        public class Cell
        {
            public Vector2 pos;
            public bool isUsed
            {
                get
                {
                    if(item != null)
                    {
                        if(item.Num < MAXNUM)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }

            public Item item;
        }

        public Dictionary<Vector2, Cell> Cells = new Dictionary<Vector2, Cell>();

        public override bool IsFull
        {
            get
            {
                foreach (var cell in Cells)
                {
                    if (!cell.Value.isUsed)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        private static readonly int MAXNUM = 3;

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
                    Cells.Add(cellPosition, new Cell { pos = cellPosition, item = null });
                }
            }
        }

        public bool TryGetAStoragePosition(string itemLabel, out Vector2 position)
        {
            foreach (var cell in Cells)
            {
                if(cell.Value.item != null && cell.Value.item.Label == itemLabel)
                {
                    if (cell.Value.item.Num < MAXNUM)
                    {
                        position = cell.Key;
                        return true;
                    }
                }

                if (!cell.Value.isUsed)
                {
                    position = cell.Key;
                    return true;
                }
            }
            position = Vector2.zero;
            return false;
        }

        public bool TryPutAItemOnPosition(Item item, Vector2 position)
        {
            if (Cells.ContainsKey(position))
            {
                Cell cell = Cells[position];
                if (cell.item == null)
                {
                    item.transform.parent = null;
                    item.transform.position = position;
                    cell.item = item;
                    cell.item.IsOnStorage = true;
                }
                else
                {
                    cell.item.Num += item.Num;
                    item.Num = 0;
                }
                return true;
            }
            Debug.LogWarning("Warning");
            return false;
        }
    }
}
