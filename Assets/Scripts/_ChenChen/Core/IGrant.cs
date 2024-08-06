using ChenChen_AI;
using System;
using System.Collections.Generic;

namespace ChenChen_Core
{
    public interface IGrant
    {
        bool IsFree { get; }
        bool IsLocked { get; }
        Pawn UserPawn { get; }

        void GetPermission(Pawn pawn);

        void RevokePermission(Pawn pawn);
    }
}
