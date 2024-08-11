using ChenChen_AI;
using ChenChen_Core;
using DG.Tweening;
using System;
using UnityEngine;

[System.Serializable]
public class TargetPtr
{
    public GameObject TargetA;
    public GameObject TargetB;

    public Vector3 VecA;
    public Vector3 VecB;

    private bool isGameObject;

    public Vector3 PositonA
    {
        get
        {
            if (isGameObject)
            {
                return TargetA.transform.position;
            }
            else
            {
                return VecA;
            }
        }
    }

    public Vector3 PositionB
    {
        get
        {
            if (isGameObject)
            {
                return TargetB.transform.position;
            }
            else
            {
                return VecB;
            }
        }
    }

    public override string ToString()
    {
        string res = string.Empty;  
        if(TargetA != null)
        {
            res += TargetA.name;
        }
        res += ":";
        if(TargetB != null)
        {
            res += TargetB.name;
        }
        return res;
    }

    public TargetPtr(GameObject a)
    {
        TargetA = a;
        isGameObject = true;
        //Debug.Log($"创建了新的TargetPtr {a.name}");
    }

    public TargetPtr(GameObject a, GameObject b)
    {
        TargetA = a;
        TargetB = b;
        isGameObject = true;
        //Debug.Log($"创建了新的TargetPtr {a.name} {b.name}");
    }

    public TargetPtr(Vector3 vecA)
    {
        VecA = vecA;
        isGameObject = false;
    }

    public TargetPtr(Vector3 vecA, Vector3 vecB)
    {
        VecA = vecA;
        VecB = vecB;
        isGameObject = false;
    }

    public T GetComponent<T>() where T : Component
    {
        return TargetA.GetComponent<T>();
    }

    public bool TryGetComponent<T>(out T component) where T : Component
    {
        if (TargetA == null)
        {
            component = null;
            return false;
        }
        return TargetA.TryGetComponent(out component);
    }

    public void CameraMoveToTarget()
    {
        Vector3 go = this.PositonA;
        go.z = Camera.main.transform.position.z;
        Camera.main.transform.DOMove(go, 1);
    }
}
