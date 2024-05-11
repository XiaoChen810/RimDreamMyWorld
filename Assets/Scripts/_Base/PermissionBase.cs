using ChenChen_AI;
using UnityEngine;

/// <summary>
/// 权限基类，用于绑定是否被使用，谁在使用。
/// 继承于MonoBehaviour
/// </summary>
public abstract class PermissionBase : MonoBehaviour
{
    public enum PermissionType 
    {
        IsFree = 0,
        IsBooking = 1,
        IsUsed = 2,
    }

    // 使用的棋子
    [SerializeField] protected Pawn _theUsingPawn;
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
    [SerializeField] protected PermissionType _permission;
    public PermissionType Permission
    {
        get
        {
            return _permission;
        }
        protected set
        {
            _permission = value;
        }
    }

    /// <summary>
    /// 预定
    /// </summary>
    /// <returns></returns>
    public virtual bool BookingMe()
    {
        Permission = PermissionType.IsBooking;
        return true;
    }

    /// <summary>
    /// 获取使用权限
    /// </summary>
    public virtual bool GetPermission(Pawn pawn)
    {
        if (_permission == PermissionType.IsUsed) return false;

        _permission = PermissionType.IsUsed;
        _theUsingPawn = pawn;
        return true;
    }

    /// <summary>
    /// 归还使用权限
    /// </summary>
    /// <param name="pawn"></param>
    /// <returns></returns>
    public virtual bool RevokePermission(Pawn pawn)
    {
        if (_theUsingPawn != pawn) return false;

        _permission = PermissionType.IsFree;
        _theUsingPawn = null;
        return true;
    }
}

