using System;
using UnityEngine;

namespace ChenChen_AI
{
    public class TargetPtr
    {
        public GameObject GameObject;
        public Vector3 Vector3;

        public bool IsGameObject;

        public Vector3 Positon
        {
            get
            {
                if(IsGameObject)
                {
                    return GameObject.transform.position;
                }
                else
                {
                    return Vector3;
                }
            }
        }

        public TargetPtr(GameObject gameObject)
        {
            GameObject = gameObject;
            IsGameObject = true;
        }

        public TargetPtr(Vector3 vector3)
        {
            Vector3 = vector3;
            IsGameObject = false;
        }

        public T GetComponent<T>() where T : Component
        {           
            return GameObject.GetComponent<T>();
        }

        public bool TryGetComponent<T>(out T component) where T : Component
        {
            return GameObject.TryGetComponent(out component);
        }
    }
}