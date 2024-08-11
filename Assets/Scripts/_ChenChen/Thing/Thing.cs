using UnityEngine;
using ChenChen_UI;
using ChenChen_Core;
using ChenChen_AI;
using System;
using System.Collections.Generic;

namespace ChenChen_Thing
{
    public abstract class Thing : MonoBehaviour, IDetailView , IGrant, ICommand
    {
        public BoxCollider2D ColliderSelf { get; protected set; }

        public SpriteRenderer SR {  get; protected set; }

        #region - 重要属性 - 

        // 物品耐久度
        public int MaxDurability
        {
            get { return 100; }
        }

        public int Durability { get; protected set; }

        public List<ValueTuple<string,int>> DestroyOutputs = new();

        #endregion

        #region - Unity Life -
        protected virtual void Start()
        {
            name = name.Replace("(Clone)", "").Trim();
            name = name.Replace("_Prefab", "").Trim();
            tag = "Thing";
        }

        protected virtual void OnEnable()
        {
            ColliderSelf = GetComponent<BoxCollider2D>();
            ColliderSelf.isTrigger = true;

            SR = GetComponent<SpriteRenderer>();
            SR.sortingLayerName = "Default";
            SR.sortingOrder = -(int)transform.position.y;

            Durability = MaxDurability;
        }

        protected virtual void OnDestroy()
        {
            if (!Application.isPlaying) return;

            var thingSystemManager = ThingSystemManager.Instance;

            if(thingSystemManager != null ) thingSystemManager.RemoveThing(this.gameObject);
        }
        #endregion

        #region - IDetailView -
        protected DetailView _detailView;
        public virtual DetailView DetailView
        {
            get
            {
                if (_detailView == null)
                {
                    if (!TryGetComponent<DetailView>(out _detailView))
                    {
                        _detailView = gameObject.AddComponent<DetailView_Thing>();
                    }
                }
                return _detailView;
            }
        }
        #endregion

        #region - IGrant -
        [Header("权限")]
        [SerializeField] private Pawn userPawn = null;

        public bool Lock => userPawn != null;

        public Pawn UserPawn => userPawn;

        public void GetPermission(Pawn pawn)
        {
            if (!Lock)
            {
                userPawn = pawn;
                Debug.Log($"{pawn.name} 获取 {gameObject.name} 权限");
            }
            else
            {
                Debug.Log($"{pawn.name} 获取 {gameObject.name} 权限失败. 已经被 {userPawn.name} 获取");
            }
        }

        public void RevokePermission(Pawn pawn)
        {
            if (Lock && userPawn == pawn)
            {
                Debug.Log($"{pawn.name} 归还权限");
                userPawn = null;
            }
            else
            {
                Debug.Log($"{pawn.name} 归还权限失败");
            }
        }

        #endregion

        #region - ICommand -
        public abstract string CommandName { get; }

        public abstract void CommandFunc(Pawn p);
        #endregion
    }
}