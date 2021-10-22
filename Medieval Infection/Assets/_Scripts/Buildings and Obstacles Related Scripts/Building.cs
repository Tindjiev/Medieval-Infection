using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNM;

public abstract class Building : MonoBehaviour
{
    public Door Door { get; protected set; }
    protected Village _village;

    public PlagueBase Plague => _village.Plague;

    public int ResidentsInfected { get; protected set; } = 0;

    public int Deaths { get; protected set; } = 0;
    public bool Dead { get; protected set; } = false;

    protected List<Person> _residents;

    public bool Useless { get; protected set; } = false;

    public int TotalResidents => _residents.Count;

    public Vector3 position => _coll.bounds.center;//transform.position + _coll.center;//_coll.center.MulElementWise(_coll.size / 2f);

    private BoxCollider _coll;

    public Vector2 SizeAsSeenFromAbove => new Vector2(_coll.size.x, _coll.size.z);

    protected void Awake()
    {
        Door = GetComponentInChildren<Door>();
        _village = this.getvars<Village>();
        _coll = GetComponentInChildren<BoxCollider>();
    }

    public abstract void EnterBuilding();

    public abstract bool IsInfected();

    public abstract void SpreadWithin();

    public abstract void ConcludeInfected();

    public abstract void DoActions();

    public abstract bool AppliedActions();

    public void GetInfected()
    {
        if (IsInfected())
        {
            return;
        }
        _village.AddInfectedBuilding();
        GetInfectedSpecific();
    }

    protected abstract void GetInfectedSpecific();
}
