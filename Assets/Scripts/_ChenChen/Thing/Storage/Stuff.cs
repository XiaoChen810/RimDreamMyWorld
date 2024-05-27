using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Thing
{

    /// <summary>
    /// ������
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