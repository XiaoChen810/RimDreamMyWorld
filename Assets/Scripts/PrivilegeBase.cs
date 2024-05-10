using ChenChen_AI;
using UnityEngine;

public abstract class PrivilegeBase : MonoBehaviour
{
    // 使用的棋子
    protected Pawn _theUsingPawn;
    public Pawn TheUsingPawn
    {
        get
        {
            return _theUsingPawn;
        }
        protected set
        {
            _theUsingPawn = value;
        }
    }

    // 是否被使用
    protected bool _isUsed;
    public bool IsUsed
    {
        get
        {
            return _isUsed;
        }
        protected set
        {
            _isUsed = value;
        }
    }

    /// <summary>
    /// 获取使用权限
    /// </summary>
    public virtual bool GetPrivilege(Pawn pawn)
    {
        if (_isUsed) return false;

        _isUsed = true;
        _theUsingPawn = pawn;
        return true;
    }

    /// <summary>
    /// 归还使用权限
    /// </summary>
    /// <param name="pawn"></param>
    /// <returns></returns>
    public virtual bool RevokePrivilege(Pawn pawn)
    {
        if (_theUsingPawn != pawn) return false;

        _isUsed = false;
        _theUsingPawn = null;
        return true;
    }
}

