using UnityEngine;

namespace ChenChen_Thing
{

    /// <summary>
    /// ���г���
    /// </summary>
    public interface ICut
    {
        void OnMarkCut();   // ����и�
        void OnCut(int value);  // �и�ʱ
        void OnCanclCut();  // ȡ���и�
    }
}