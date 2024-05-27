using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Thing
{

    /// <summary>
    /// ≤ƒ¡œ¿‡
    /// </summary>
    [System.Serializable]
    public class Stuff
    {
        public StuffDef Def;

        public int Num;

        public Stuff(StuffDef def)
        {
            Def = def;
        }
    }
}