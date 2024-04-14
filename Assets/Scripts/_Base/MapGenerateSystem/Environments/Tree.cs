using ChenChen_BuildingSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Tree : ThingBase
{
    private Collider2D coll;
    private SpriteRenderer sr;

    private void Start()
    {
        coll = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        coll.isTrigger = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Pawn"))
        {
            sr.color = new Color(1, 1, 1, 0.5f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Pawn"))
        {
            sr.color = new Color(1, 1, 1, 1);
        }
    }

    public override void Placed()
    {
        throw new System.NotImplementedException();
    }

    public override void Build(int thisWorkload)
    {
        throw new System.NotImplementedException();
    }

    public override void Complete()
    {
        throw new System.NotImplementedException();
    }

    public override void Cancel()
    {
        throw new System.NotImplementedException();
    }

    public override void Interpret()
    {
        throw new System.NotImplementedException();
    }

    public override void OnMarkDemolish()
    {
        throw new System.NotImplementedException();
    }

    public override void Demolish(int value)
    {
        throw new System.NotImplementedException();
    }

    public override void OnDemolished()
    {
        throw new System.NotImplementedException();
    }
}
