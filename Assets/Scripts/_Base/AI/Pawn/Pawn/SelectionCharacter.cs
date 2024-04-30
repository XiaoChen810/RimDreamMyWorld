using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_AI
{
    public class SelectionCharacter : Pawn
    {
        protected override void Start()
        {
            Animator = GetComponent<Animator>();
        }

        protected override void Update()
        {
            Animator.SetBool("IsWalk", true);
        }

        protected override void TryToGetJob()
        {
            throw new System.NotImplementedException();
        }
    }
}