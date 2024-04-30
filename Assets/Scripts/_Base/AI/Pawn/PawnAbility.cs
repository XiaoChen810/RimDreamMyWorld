using UnityEngine;

namespace ChenChen_AI
{

    /// <summary>
    /// 人物的某种能力
    /// </summary>
    [System.Serializable]
    public class PawnAbility
    {
        /// <summary>
        /// 能力具体数值
        /// </summary>
        [SerializeField] private int _value;
        public int Value
        {
            get { return _value; }
            set
            {
                _value = value;
            }
        }

        /// <summary>
        /// 能力兴趣的级别，无0，有兴趣1，喜欢2，最爱3
        /// </summary>
        [SerializeField] private int _interest;
        public int Interest
        {
            get { return _interest; }
            set
            {
                _interest = Mathf.Clamp(value, 0, 3);
            }
        }

        /// <summary>
        /// 能力的经验值，一个大于0的值
        /// </summary>
        [SerializeField] private float _exp;
        public float EXP
        {
            get { return _exp; }
            set
            {
                _exp = value > 0 ? value : 0;
            }
        }

        public PawnAbility(int interest, float exp, int value)
        {
            _interest = interest;
            _exp = exp;
            _value = value;
        }
    }
}