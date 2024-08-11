using ChenChen_AI;
using System;
using System.Collections.Generic;

namespace ChenChen_Core
{
    public interface IGrant
    {
        bool Lock { get; }

        Pawn UserPawn { get; }

        void GetPermission(Pawn pawn);

        void RevokePermission(Pawn pawn);
    }
}
