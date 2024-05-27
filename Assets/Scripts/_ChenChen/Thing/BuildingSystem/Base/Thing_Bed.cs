using ChenChen_AI;
using System;
using UnityEngine;

namespace ChenChen_Thing
{
    [RequireComponent(typeof(Collider2D))]
    public class Thing_Bed : Thing_Building
    {
        [SerializeField] private Pawn _owner;
        public Pawn Owner
        {
            get { return _owner; }
            set
            {
                Debug.Log($"{this.name}设定了持有者{value.Def.PawnName}");
                _owner = value;
            }
        }
    }
}
