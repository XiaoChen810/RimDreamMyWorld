using ChenChen_Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Thing
{
    [Serializable]
    public class AppealDef : Def
    {
        public string name;
        public string description;
        public Sprite icon;
        public int workload;
        public int costFabric;

        public AppealDef(string name, string description, Sprite icon, int workload, int costFabric)
        {
            this.name = name ?? throw new ArgumentNullException(nameof(name));
            this.description = description ?? throw new ArgumentNullException(nameof(description));
            this.icon = icon ?? throw new ArgumentNullException(nameof(icon));
            this.workload = workload;
            this.costFabric = costFabric;
        }

        public override object DeepClone()
        {
            throw new NotImplementedException();
        }
    }
}
