using ChenChen_AI;
using ChenChen_Core;
using DG.Tweening;
using System;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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


    public TargetPtr(GameObject targetA)
    {
        TargetA = targetA;
        isGameObject = true;
    }

    public TargetPtr(GameObject targetA, GameObject targetB)
    {
        TargetA = targetA;
        TargetB = targetB;
        isGameObject = true;
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
        return TargetA.TryGetComponent(out component);
    }

    public void CameraMoveToTarget()
    {
        Vector3 go = this.PositonA;
        go.z = Camera.main.transform.position.z;
        Camera.main.transform.DOMove(go, 1);
    }
}
