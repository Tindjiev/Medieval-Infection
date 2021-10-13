using UnityEngine;
using RandNM;
using MathNM;

public enum PeopleAge
{
    Elder,MiddleAged,Young,Infant
}

public abstract class Person
{
    private bool _appliedWater = false;
    public bool HasBeenInfectedBefore { get; private set; } = false;

    protected int _dayOfInfection;
    public int DaysInfected => DayNightCycle.DayCount - _dayOfInfection;

    protected int _daysTillJudgement;

    public const int MIN_DAYS_TILL_JUDGEMENT = 2;
    public const int MAX_DAYS_TILL_JUDGEMENT = 5;


    protected int _daysTillInfectedAgain = 0;
    protected int _daysTillInfectedAgainStart = 0;

    public const int MIN_DAYS_TILL_INFECTED_AGAIN = 8;
    public const int MAX_DAYS_TILL_INFECTED_AGAIN = 15;

    protected float _actionPositivity = 1f;//{ get; protected set; } = 1f;
    protected float _actionNegativity = 1f;//{ get; protected set; } = 1f;
    protected float _deathRate;
    protected float _recoveryRate;

    public House House { get; private set; }

    public PlagueBase Plague => House.Plague;

    public readonly PeopleAge Age;

    public bool Infected { get; protected set; } = false;
    public float Value { get; private set; }
    private float _proneToInfectionFactor;

    public Action ActionToBeTaken { get; set; } = null;

    public bool GettingInfected { get; protected set; } = false;
    public bool Dying { get; protected set; } = false;
    public bool Recovering { get; protected set; } = false;


    public bool Dead { get; protected set; } = false;

    public abstract override string ToString();

    protected Person(PeopleAge age, House house, float value, float proneToInfectionFactor, float deathRate, float recoveryRate)
    {
        Age = age;
        House = house;
        Value = value;
        _proneToInfectionFactor = proneToInfectionFactor;
        _deathRate = deathRate;
        _recoveryRate = recoveryRate;
    }

    protected virtual void GetInfectedSpecific()
    {

    }

    public virtual void DoAction()
    {
        if (ActionToBeTaken != null)
        {
            ActionToBeTaken.DoAction(this);
            //Debug.Log("Did " + ActionToBeTaken.Name() + " on " + this, House);
            ActionToBeTaken = null;
        }
    }

    public float TrustChangeInfected() => -Value / House.ResidanceAcceptance;

    public float TrustChangeDeath() => TrustChangeDeath(1f);

    public float TrustChangeDeath(float MultipliedNegativity) => TrustChangeDeath(MultipliedNegativity, _appliedWater);

    public float TrustChangeDeath(float MultipliedNegativity, bool appliedWater) => -Value / House.ResidanceAcceptance * (_actionNegativity * MultipliedNegativity) + (appliedWater ? 0f : -20f);

    public void ApplyWater() => _appliedWater = true;

    public float TrustChangeRecover() => TrustChangeRecover(1f);

    public float TrustChangeRecover(float MultipliedPositivity) => 5f * Value * House.ResidanceAcceptance * (_actionPositivity * MultipliedPositivity);

    public void ResetActionPolarity()
    {
        _actionPositivity = 1f;
        _actionNegativity = 1f;
    }

    public void MultDeathRate(float deathRateMultiplier) => _deathRate *= deathRateMultiplier;

    public void MultRecoveryRate(float recoveryRateMultiplier) => _recoveryRate *= recoveryRateMultiplier;

    public void MultActionPositivity(float PositivityMultiplier) => _actionPositivity *= PositivityMultiplier;

    public void MultActionNegativity(float NegativityMultiplier) => _actionNegativity *= NegativityMultiplier;

    public bool TryToBeInfected()
    {
        RandFloat r = CalcChanceToGetinfected();
        if (r.RollDice())
        {
            GetInfected();
            return true;
        }
        else
        {
            if (_daysTillInfectedAgain <= 0)
            {
                _daysTillInfectedAgain = Random.Range(2, 5 + 1);
            }
            return false;
        }
    }

    public void ConcludeInfected()
    {
        if (!Infected) return;

        if (_daysTillJudgement > 0)
        {
            DecreaseOneDayTillJudgement();
            return;
        }
        if (!DieRollDice() && RecoverRollDice())
        {
            ResetActionPolarity();
        }
    }

    private bool DieRollDice()
    {
        if (Dead) return false;

        RandFloat r = CalcChanceToDie();
        if (r.RollDice())
        {
            Die();
            return true;
        }
        return false;
    }

    private bool RecoverRollDice()
    {
        if (!Infected) return false;

        RandFloat r = CalcChanceToRecover();
        if (r.RollDice())
        {
            Recover();
            return true;
        }
        return false;
    }

    public virtual RandFloat CalcChanceToGetinfected() => RandFloat.Fifty * _proneToInfectionFactor;

    public RandFloat CalcChanceToDie() => CalcChanceToDie(1f);

    public RandFloat CalcChanceToDie(float multiplied) => Plague.MortalityChancePerDay * (_deathRate * multiplied);

    public RandFloat CalcChanceToRecover() => CalcChanceToRecover(1f);

    public RandFloat CalcChanceToRecover(float multiplied) => Plague.RecoveryChancePerDay * (_recoveryRate * multiplied);

    public RandFloat ChanceForFamilyToKill => House.ResidanceCooperation / Value.Sqrt();

    public bool CanBeInfectedAgain() => DayNightCycle.DayCount - _daysTillInfectedAgainStart >= _daysTillInfectedAgain;

    public void SetDaysToBeAbleToBeInfectedAgain()
    {
        _daysTillInfectedAgainStart = DayNightCycle.DayCount;
        _daysTillInfectedAgain = Random.Range(MIN_DAYS_TILL_INFECTED_AGAIN, MAX_DAYS_TILL_INFECTED_AGAIN + 1);
    }

    public void GetInfected()
    {
        if (Infected || Dead || !CanBeInfectedAgain()) return;

        GettingInfected = true;
        if (House != null) House.ResidentGetInfected(this);
        Infected = true;
        _dayOfInfection = DayNightCycle.DayCount;
        DetermineDaysTillJudgement();
        GetInfectedSpecific();
        GettingInfected = false;
    }

    private void DetermineDaysTillJudgement()
    {
        _daysTillJudgement = Random.Range(MIN_DAYS_TILL_JUDGEMENT, MAX_DAYS_TILL_JUDGEMENT + 1) - 1;
    }

    public void DecreaseOneDayTillJudgement()
    {
        if (_daysTillJudgement > 0)
        {
            _daysTillJudgement--;
        }
    }

    public void Die()
    {
        if (Dead) return;

        Dying = true;
        if (House != null) House.ResidentDeath(this);
        Infected = false;
        Dying = false;
        Dead = true;
    }

    public void Recover()
    {
        if (!Infected) return;

        Recovering = true;
        if (House != null) House.ResidentRecover(this);
        SetDaysToBeAbleToBeInfectedAgain();
        Infected = false;
        _proneToInfectionFactor /= 2f;
        HasBeenInfectedBefore = true;
        Recovering = false;
    }
}



public class Elder : Person
{
    public Elder(House house, float value = 1.5f, float infectionFactor = 1.3f, float deathRate = 3f, float recoveryRate = 0.5f) : base(PeopleAge.Elder, house, value, infectionFactor, deathRate, recoveryRate)
    {
    }

    public override string ToString() => "Elder";

}

public class MiddleAged : Person
{
    public MiddleAged(House house, float value = 2.5f, float infectionFactor = 0.85f, float deathRate = 1f, float recoveryRate = 1f) : base(PeopleAge.MiddleAged, house, value, infectionFactor, deathRate, recoveryRate)
    {
    }

    public override string ToString() => "Middle Aged";
}

public class Young : Person
{
    public Young(House house, float value = 2f, float infectionFactor = 1f, float deathRate = 1.5f, float recoveryRate = 1.5f) : base(PeopleAge.Young, house, value, infectionFactor, deathRate, recoveryRate)
    {
    }

    public override string ToString() => "Young";
}

public class Infant : Person
{
    public Infant(House house, float value = 1f, float infectionFactor = 1.2f, float deathRate = 3f, float recoveryRate = 2f) : base(PeopleAge.Infant, house, value, infectionFactor, deathRate, recoveryRate)
    {
    }

    public override string ToString() => "Infant";

}