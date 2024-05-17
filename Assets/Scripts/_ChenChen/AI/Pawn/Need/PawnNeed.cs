using System;
using UnityEngine;

namespace ChenChen_AI
{
    [System.Serializable]
    public class PawnNeed : ICloneable
    {
        private float _probability = -1;

        /// <summary>
        /// 优先级，越低越优先
        /// </summary>
        public float Probability
        {
            get
            {
                if (_probability < 0) Debug.LogWarning("Probability no set");
                return _probability;
            }
            set
            {
                _probability = value;
            }
        }

        private string _description;

        /// <summary>
        /// 需求描述，xxx想要 “Description” ...
        /// </summary>
        public string Description
        {
            get => _description;
            set => _description = value;
        }

        private bool isComplete;

        public PawnNeed(float probability, string description)
        {
            _probability = probability;
            _description = description;
        }

        /// <summary>
        /// 是否达成需要，一开时都是False
        /// </summary>
        public bool IsCompelte
        {
            get => isComplete;
            set => isComplete = value;
        }

        public object Clone()
        {
            PawnNeed clone = (PawnNeed)MemberwiseClone();
            return clone;
        }
    }
}
