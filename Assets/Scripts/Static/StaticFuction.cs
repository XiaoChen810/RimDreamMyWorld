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

    /// <summary>
    /// 比较两点之间的距离，在指定距离内返回 true，大于指定距离返回 false
    /// </summary>
    /// <param name="v1">第一个点</param>
    /// <param name="v2">第二个点</param>
    /// <param name="compare">比较的距离</param>
    /// <returns>如果两点之间的距离小于 compare，返回 true；否则返回 false</returns>
    public static bool CompareDistance(Vector2 v1, Vector2 v2, float compare)
    {
        // 计算两点之间的平方距离
        float distanceSquared = (v1.x - v2.x) * (v1.x - v2.x) + (v1.y - v2.y) * (v1.y - v2.y);
        // 计算比较距离的平方
        float compareSquared = compare * compare;

        // 比较平方距离
        return distanceSquared <= compareSquared;
    }

}
