using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_CropSystem
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Crop : PrivilegeBase
    {
        [SerializeField] public CropDef Def { get; private set; }
        [SerializeField] public WorkSpace_Farm WorkSpace { get; private set; }

        private float _curNutrient = 0;
        public float CurNutrient    // 当前营养值
        {
            get { return _curNutrient; }
            set 
            { 
                _curNutrient = value;
                _curNutrient = _curNutrient > Def.GroupNutrientRequiries ? Def.GroupNutrientRequiries : _curNutrient;
            }
        }

        private int _curPeriodIndex = 0;
        public int CurPeriodIndex  // 当前生长阶段
        {
            get { return _curPeriodIndex; }
            set
            {
                if(_curPeriodIndex != value)
                {
                    _curPeriodIndex = value;
                    _sr.sprite = Def.CropsSprites[value];
                }
            }
        }

        public int MaxPeriodIndex;  // 最大生长阶段

        private SpriteRenderer _sr;

        public void Init(CropDef def, WorkSpace_Farm workSpace)
        {
            Def = def;
            WorkSpace = workSpace;

            _sr = GetComponent<SpriteRenderer>();
            _sr.sprite = Def.CropsSprites[CurPeriodIndex];
            _sr.sortingLayerName = workSpace.GetComponent<SpriteRenderer>().sortingLayerName;

            CurNutrient = 0;
            CurPeriodIndex = 0;
            MaxPeriodIndex = Def.CropsSprites.Count - 1;
        }

        private void Update()
        {
            CurNutrient += Time.deltaTime * Def.GroupSpeed;

            CurPeriodIndex = (int)((CurNutrient / Def.GroupNutrientRequiries) * MaxPeriodIndex);
        }
    }
}
