using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Thing
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Crop : PermissionBase
    {
        public CropDef Def;
        public WorkSpace_Farm WorkSpace;

        [SerializeField] private float _curNutrient = 0;
        public float CurNutrient    // 当前营养值
        {
            get { return _curNutrient; }
            set
            {
                _curNutrient = value;
                _curNutrient = _curNutrient > Def.GroupNutrientRequiries ? Def.GroupNutrientRequiries : _curNutrient;
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
                    return;
                }
                _curPeriodIndex = value;
                _sr.sprite = Def.CropsSprites[value];
            }
        }

        public int MaxPeriodIndex => Def.CropsSprites.Count - 1;  // 最大生长阶段

        private SpriteRenderer _sr;

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
        }

        private void OnEnable()
        {
            GameManager.Instance.OnTimeAddOneMinute += Group;
        }

        private void OnDisable()
        {
            GameManager.Instance.OnTimeAddOneMinute -= Group;
        }

        private void Group()
        {
            float add = Def.GroupSpeed / 1440;
            CurNutrient += add;
            CurPeriodIndex = (int)(CurNutrient / Def.GroupNutrientRequiries * MaxPeriodIndex);
        }
    }
}
