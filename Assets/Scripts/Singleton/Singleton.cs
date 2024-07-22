using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> where T : new()
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new T();
                Debug.Log("创建新的单例实例: " + instance.ToString());
            }
            return instance;
        }

        protected set { instance = value; }
    }
}
