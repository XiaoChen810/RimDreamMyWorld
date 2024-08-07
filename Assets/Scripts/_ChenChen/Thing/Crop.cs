using ChenChen_AI;
using ChenChen_Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Thing
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Crop : MonoBehaviour, IGrant
    {
        public CropDef Def;
        public WorkSpace_Farm WorkSpace;

        [SerializeField] private float _curNutrient = 0;
        public float CurNutrient    // 当前营养值
        {
            get { return _curNutrient; }
            set
            {
                _curNutrient = Mathf.Clamp(value, 0, Def.GroupNutrientRequiries);
            }
        }

        [SerializeField] private int _curPeriodIndex = 0;
        public int CurPeriodIndex  // 当前生长阶段
        {
            get { return _curPeriodIndex; }
            set
            {
                if (value > MaxPeriodIndex || value < 0)
                {
                    Debug.LogWarning("out Range: " + value);
                    _curPeriodIndex = Mathf.Clamp(value, 0, MaxPeriodIndex);
                }
                _sr.sprite = Def.CropsSprites[value];
            }
        }

        public int MaxPeriodIndex => Def.CropsSprites.Count - 1;  // 最大生长阶段

        private SpriteRenderer _sr;

        [SerializeField] private float _wiltTime;

        public void Init(CropDef def, WorkSpace_Farm workSpace)
        {
            Def = def;
            WorkSpace = workSpace;
            _sr = GetComponent<SpriteRenderer>();
            _sr.sprite = Def.CropsSprites[CurPeriodIndex];
            _sr.sortingLayerName = "Above";
            _sr.sortingOrder = -(int)gameObject.transform.position.y;
            CurNutrient = 0;
            CurPeriodIndex = 0;
        }
        public void Init(CropDef def, WorkSpace_Farm workSpace, Data_CropSave save)
        {
            Def = def;
            WorkSpace = workSpace;
            _sr = GetComponent<SpriteRenderer>();
            _sr.sprite = Def.CropsSprites[CurPeriodIndex];
            _sr.sortingLayerName = "Above";
            _sr.sortingOrder = -(int)gameObject.transform.position.y;
            CurNutrient = save.CurNutrient;
            CurPeriodIndex = save.CurPeriodIndex;
            groupSpeed = Def.GroupSpeed / 1440;
        }

        private void OnEnable()
        {
            GameManager.Instance.OnTimeAddOneMinute += Group;
            _wiltTime = 0;
        }

        private void OnDisable()
        {
            GameManager.Instance.OnTimeAddOneMinute -= Group;
        }

        private float groupSpeed;
        private void Group()
        {
            CurNutrient += groupSpeed;
            CurPeriodIndex = (int)(CurNutrient / Def.GroupNutrientRequiries * MaxPeriodIndex);

            if(CurNutrient == MaxPeriodIndex)
            {
                _wiltTime += groupSpeed;
            }

            if (_wiltTime > Def.WiltTime)
            {
                Debug.Log("植物枯萎");
                Destroy(gameObject);
            }
        }

        #region - IGrant -

        private object lockObj = new object();

        [Header("权限")]
        [SerializeField] private bool isLocked;
        [SerializeField] private Pawn userPawn;

        public bool UnLock => isLocked;

        public Pawn UserPawn => userPawn;

        public void GetPermission(Pawn pawn)
        {
            lock (lockObj)
            {
                if (!isLocked)
                {
                    isLocked = true;
                    userPawn = pawn;
                    Debug.Log($"Permission granted to {pawn.name}");
                }
                else
                {
                    Debug.Log($"Permission denied to {pawn.name}. Already locked by {userPawn.name}");
                }
            }
        }

        public void RevokePermission(Pawn pawn)
        {
            lock (lockObj)
            {
                if (isLocked && userPawn == pawn)
                {
                    isLocked = false;
                    Debug.Log($"Permission revoked from {pawn.name}");
                    userPawn = null;
                }
                else
                {
                    Debug.Log($"Permission revoke failed for {pawn.name}");
                }
            }
        }
        #endregion
    }
}
