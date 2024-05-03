using System;
using System.Collections.Generic;
using UnityEngine;

public static class StaticFuction
{
    /// <summary>
    /// 将Vector3向下取整为Vector3Int
    /// </summary>
    /// <param name="vector3"></param>
    /// <returns></returns>
    public static Vector3Int VectorTransToInt(Vector3 vector3)
    {
        Vector3Int vector3Int = new Vector3Int();
        vector3Int.x = Mathf.FloorToInt(vector3.x);
        vector3Int.y = Mathf.FloorToInt(vector3.y);
        vector3Int.z = Mathf.FloorToInt(vector3.z);
        return vector3Int;
    }
}
