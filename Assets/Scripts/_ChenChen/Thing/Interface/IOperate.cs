using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Thing
{
    public interface IOperate
    {
        /// <summary>
        /// 正在等待去操作
        /// </summary>
        bool IsWaitToOperate {  get; }
        bool IsCompleteOperate { get; }
        /// <summary>
        /// 操作点
        /// </summary>
        Vector3 OperationPosition { get; }  
        /// <summary>
        /// 一次操作所需时间
        /// </summary>
        float OnceTime { get; }
        /// <summary>
        /// 完成一次操作后发生的
        /// </summary>
        void Operate(); 
    }
}
