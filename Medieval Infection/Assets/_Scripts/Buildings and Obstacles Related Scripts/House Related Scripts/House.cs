using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using MathNM;
using RandNM;

public class House : Building, IEnumerable<Person>
{

    [SerializeField]
    private HouseSO _houseInfo;

    public RandFloat ResidanceCooperation { get; private set; }
    public float ResidanceAcceptance => _houseInfo.ResidanceAcceptance;

    private bool _infected
    {
        get
        {
            return ResidentsInfected != 0;
        }
    }

    protected new void Awake()
    {
        base.Awake();
        ResidanceCooperation = _houseInfo.ResidanceCooperation;
        createResidents();
    }

    private void createResidents()
    {
        _residents = new List<Person>(_houseInfo.NumberOfResidents_Elderly + _houseInfo.NumberOfResidents_MiddleAged + _houseInfo.NumberOfResidents_Young + _houseInfo.NumberOfResidents_Infants);
        for (int i = 0; i < _houseInfo.NumberOfResidents_Elderly; i++)
        {
            _residents.Add(new Elder(this));
        }
        for (int i = 0; i < _houseInfo.NumberOfResidents_MiddleAged; i++)
        {
            _residents.Add(new MiddleAged(this));
        }
        for (int i = 0; i < _houseInfo.NumberOfResidents_Young; i++)
        {
            _residents.Add(new Young(this));
        }
        for (int i = 0; i < _houseInfo.NumberOfResidents_Infants; i++)
        {
            _residents.Add(new Infant(this));
        }
    }

    public void ResidentGetInfected(Person resident)
    {
        if (!resident.GettingInfected)
        {
            resident.GetInfected();
            return;
        }
        _village.IncreaseTrust(resident.TrustChangeInfected());
        _village.IncreaseInfections();
        ResidentsInfected++;
    }

    public void ResidentDeath(Person resident)
    {
        if (!resident.Dying)
        {
            resident.Die();
            return;
        }
        Deaths++;
        _village.IncreaseTrust(resident.TrustChangeDeath());
        _residents.Remove(resident);
        if (!Useless && _residents.Find(x => x.Age != PeopleAge.Infant && x.Age != PeopleAge.Young) == null)
        {
            Useless = true;
            _village.IncreaseTrust(-15f * _houseInfo.ResidanceValue);
        }
        if (_residents.Count == 0)
        {
            Dead = true;
        }
        if (resident.Infected)
        {
            ResidentsInfected--;
        }
    }

    public void ResidentRecover(Person resident)
    {
        if (!resident.Recovering)
        {
            resident.Recover();
            return;
        }
        _village.IncreaseTrust(resident.TrustChangeRecover());
        _village.IncreaseRecoveries();
        ResidentsInfected--;
    }


    public override void ConcludeInfected()
    {
        for (int i = _residents.Count - 1; i != -1; i--)
        {
            _residents[i].ConcludeInfected();
        }
    }


    public override void SpreadWithin()
    {
        if (!IsInfected() || TotalResidents == ResidentsInfected)
        {
            return;
        }
        float maxr = TotalResidents - ResidentsInfected;
        int peopleToTry = (int)(Random.Range(0f, 1f).sq() * maxr);
        //ArrayLib.RandomizeArray(ref _residents);
        for(int i = 0; i < peopleToTry; i++)
        {
            _residents.GetRandomElement().TryToBeInfected();
        }
        
    }

    public override void EnterBuilding()
    {
        MySceneManager.LoadHouseScene(this);
    }

    public override bool IsInfected()
    {
        return _infected;
    }

    protected override void GetInfectedSpecific()
    {
        //try
        //{
            _residents.GetRandomElement().GetInfected();
      /*  }
        catch(System.Exception e)
        {
            Debug.Log(_residents.Count,this);
            Debug.Log(e);
            Debug.Break();

        }*/
    }

    public override void DoActions()
    {
        for (int i = _residents.Count - 1; i != -1; i--)
        {
            _residents[i].DoAction();
        }
    }

    public override bool AppliedActions()
    {
        foreach(Person resident in _residents)
        {
            if (resident.Infected && resident.ActionToBeTaken == null)
            {
                return false;
            }
        }
        return true;
    }

    public Person GetResident(int index)
    {
        return _residents[index];
    }

    public IEnumerator<Person> GetEnumerator()
    {
        return ((IEnumerable<Person>)_residents).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

}
