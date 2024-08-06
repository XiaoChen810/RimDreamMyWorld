using UnityEngine;

namespace ChenChen_Thing
{

    /// <summary>
    /// 可切除的
    /// </summary>
    public interface ICut
    {
        void OnMarkCut();   // 标记切割
        void OnCut(int value);  // 切割时
        void OnCanclCut();  // 取消切割
    }
}