using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_UI
{

    /// <summary>
    /// 存储UI的信息，路径和名字 
    /// </summary>
    [System.Serializable]
    public class UIType
    {
        public string Name { get; private set; }

        public string Path { get; private set; }

        public UIType(string Path)
        {
            this.Path = Path;
            Name = Path.Substring(Path.LastIndexOf('/') + 1);
        }

        public override bool Equals(object obj)
        {
            UIType other = obj as UIType;
            return other != null && other.Name == Name && other.Path == Path;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}