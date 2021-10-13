using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Village : MonoBehaviour, IEnumerable<Building>
{
    public PlagueBase Plague { get; private set; }

    public int Recoveries { get; private set; } = 0;
    public int Infections { get; private set; } = 0;

    private float _trust = 50f;

    public float Trust
    {
        get
        {
            if (_trust > 100f)
            {
                return _trust = 100f;
            }
            if (_trust < -100f)
            {
                return _trust = -100f;
            }
            return _trust;
        }
    }



    [Zenject.Inject]
    public void Construct(PlagueBase plague, DayNightCycle dayCycle)
    {
        Plague = plague;
        dayCycle.EndayEvent.AddListener(DoActions);
    }

    private Building[] _buildings;


    public int BuildingsInfected { get; private set; }


    public int NumberOfBuildings
    {
        get
        {
            return _buildings.Length;
        }
    }

    public int TotalResidents
    {
        get
        {
            int TotalResidents = 0;
            for(int i = 0; i < _buildings.Length; i++)
            {
                TotalResidents += _buildings[i].TotalResidents;
            }
            return TotalResidents;
        }
    }

    public int InfectedResidents
    {
        get
        {
            int InfectedResidents = 0;
            for (int i = 0; i < _buildings.Length; i++)
            {
                InfectedResidents += _buildings[i].ResidentsInfected;
            }
            return InfectedResidents;
        }
    }

    public int DeadResidents
    {
        get
        {
            int DeadResidents = 0;
            for (int i = 0; i < _buildings.Length; i++)
            {
                DeadResidents += _buildings[i].Deaths;
            }
            return DeadResidents;
        }
    }

    public int DeathPercentage
    {
        get
        {
            return DeadResidents / (DeadResidents + TotalResidents);
        }
    }

    public void IncreaseRecoveries()
    {
        Recoveries++;
    }

    public void IncreaseInfections()
    {
        Infections++;
    }

    public Building this[int index]
    {
        get
        {
            return _buildings[index];
        }
    }

    public void AddInfectedBuilding()
    {
        BuildingsInfected++;
    }

    private void Awake()
    {
        _buildings = GetComponentsInChildren<Building>();
    }

    private void Start()
    {
        _buildings.GetRandomElement().GetInfected();
    }

    public void IncreaseTrust(float addedTrust)
    {
        if(DayNightCycle.DayCount > 1)
        {
            _trust += addedTrust;
        }
    }

    public void SpreadWithinBuildings()
    {
        foreach (Building building in _buildings)
        {
            building.SpreadWithin();
        }
    }

    public void ConcludeInfected()
    {
        foreach (Building building in _buildings)
        {
            building.ConcludeInfected();
        }
    }

    public void DoActions()
    {
        foreach(Building building in _buildings)
        {
            building.DoActions();
        }
    }

    private struct Enumerator : IEnumerator<Building>
    {
        private int _iEnumeratorIndex;
        private Building[] _buildings;
        public Enumerator(Village village)
        {
            _buildings = village._buildings;
            _iEnumeratorIndex = -1;
        }

        public Building Current
        {
            get
            {
                return _buildings[_iEnumeratorIndex];
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        void IDisposable.Dispose()
        {
        }

        bool IEnumerator.MoveNext()
        {
            return (++_iEnumeratorIndex) < _buildings.Length;
        }

        void IEnumerator.Reset()
        {
            _iEnumeratorIndex = -1;
        }
    }

    public IEnumerator<Building> GetEnumerator()
    {
        //return ((IEnumerable<Building>)_buildings).GetEnumerator();
        return new Enumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

}
