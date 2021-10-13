using System.Collections.Generic;
using UnityEngine;

public class ActionsManager : MonoBehaviour
{


    public static List<Action> Actions { get; private set; }

    private void Start()
    {
        Actions = new List<Action>();
        Actions.Add(new ActionHolyWater());
        Actions.Add(new ActionTreacle());
        Actions.Add(new ActionBloodLetCut());
        Actions.Add(new ActionBloodLetLeeches());
        Actions.Add(new ActionSuggestKill());
        Actions.Add(new ActionKill());
    }



}


public abstract class Action
{

    public readonly string Name;
    public readonly int AfterWhatDayCanBeApplied;

    protected readonly float _deathRateMultiplier;
    protected readonly float _recoveryRateMultiplier;
    protected readonly float _positivityMultiplier;
    protected readonly float _negativityMultiplier;

    protected Action(string name, int afterWhatDayCanBeApplied = 0, float deathRateMultiplier = 1f, float recoveryRateMultiplier = 1f, float positivityMultiplier = 1f, float negativityMultiplier = 1f)
    {
        Name = name;
        AfterWhatDayCanBeApplied = afterWhatDayCanBeApplied;

        _deathRateMultiplier = deathRateMultiplier;
        _recoveryRateMultiplier = recoveryRateMultiplier;
        _positivityMultiplier = positivityMultiplier;
        _negativityMultiplier = negativityMultiplier;
    }



    public virtual bool Condition(Person person)
    {
        return true;
    }

    public void ChooseAction(Person person)
    {
        person.ActionToBeTaken = this;
        ChooseActionSepecific();
    }

    public void UnChooseAction(Person person)
    {
        person.ActionToBeTaken = null;
        UnChooseActionSepecific();
    }
    public void ReplaceAction(Person person, Action newAction)
    {
        UnChooseActionSepecific();
        newAction.ChooseAction(person);
    }

    protected virtual void ChooseActionSepecific()
    {

    }

    protected virtual void UnChooseActionSepecific()
    {

    }

    public virtual void DoAction(Person person)
    {
        person.MultDeathRate(_deathRateMultiplier);
        person.MultRecoveryRate(_recoveryRateMultiplier);
        person.MultActionPositivity(_positivityMultiplier);
        person.MultActionNegativity(_negativityMultiplier);
    }

    public virtual string Info(Person person)
    {
        return "Chance to die will be: " + person.CalcChanceToDie(_deathRateMultiplier) +
            ", Trust will drop by " + (-person.TrustChangeDeath(_negativityMultiplier)).ToString("0") +
            "\nChance to recover will be: " + person.CalcChanceToRecover(_recoveryRateMultiplier) +
            ", Trust will increase by " + person.TrustChangeRecover(_positivityMultiplier).ToString("0");
    }

    public override string ToString()
    {
        return Name;
    }

}

public class ActionKill : Action
{

    public ActionKill() : base("Kill the resident yourself", 5, negativityMultiplier: 15f)
    {
    }

    public override void DoAction(Person person)
    {
        person.MultActionNegativity(_negativityMultiplier);
        person.Die();
    }

    public override string Info(Person person)
    {
        return "This will kill them for sure at the end of the day before any chance of recovery" +
            "\nTrust will drop by " + (-person.TrustChangeDeath(_negativityMultiplier)).ToString("0");
    }
}

public class ActionSuggestKill : Action
{
    public ActionSuggestKill() : base("Tell the family to kill the resident", 3, positivityMultiplier: 1.5f, negativityMultiplier: 2.5f)
    {
    }

    public override void DoAction(Person person)
    {
        if (person.House.Useless || person.House.TotalResidents == 1)
        {
            return;
        }

        if (person.ChanceForFamilyToKill.RollDice())
        {
            person.MultActionNegativity(_negativityMultiplier);
            person.Die();
        }
        else
        {
            person.MultActionPositivity(_positivityMultiplier);
        }
    }

    public override bool Condition(Person person)
    {
        return !person.House.Useless && person.House.TotalResidents > 1;
    }

    public override string Info(Person person)
    {
        return "The chance is " + person.ChanceForFamilyToKill + " for the family to go through with this" +
            "\nTrust will drop by " + (-person.TrustChangeDeath(_negativityMultiplier)).ToString("0");
    }
}

public class ActionBloodLetCut : Action
{
    public ActionBloodLetCut() : base("Drain blood by cutting skin", 0, 1.1f, 1.3f, 2f, 1.1f)
    {
    }

    public override bool Condition(Person person)
    {
        return person.Age != PeopleAge.Infant && person.Age != PeopleAge.Elder;
    }

    public override void DoAction(Person person)
    {
        base.DoAction(person);
        person.DecreaseOneDayTillJudgement();
    }
}

public class ActionBloodLetLeeches : Action
{
    public ActionBloodLetLeeches() : base("Drain blood by applying leeches", 0, 1.3f, 1.2f, 2.5f, 1.5f)
    {
    }

    public override bool Condition(Person person)
    {
        return person.Age == PeopleAge.MiddleAged;
    }

    public override void DoAction(Person person)
    {
        base.DoAction(person);
        person.DecreaseOneDayTillJudgement();
        person.DecreaseOneDayTillJudgement();
    }

}

public class ActionTreacle : Action
{
    private int _treacleLeft;
    public ActionTreacle() : base("Give them treacle", 0, 0.5f, 1.7f, 1f, 1.3f)
    {
        _treacleLeft = 50;
    }

    protected override void ChooseActionSepecific()
    {
        _treacleLeft--;
    }

    protected override void UnChooseActionSepecific()
    {
        _treacleLeft++;
    }

    public override bool Condition(Person person)
    {
        return _treacleLeft > 0 || person.ActionToBeTaken == this;
    }

    public override string Info(Person person)
    {
        return base.Info(person) + "\n\nTreacle uses left after this: " + _treacleLeft;
    }
}

public class ActionHolyWater : Action
{
    public ActionHolyWater() : base("Purify with Holy Water", 0, positivityMultiplier: 1.1f)
    {
    }

    public override void DoAction(Person person)
    {
        person.ApplyWater();
        person.MultActionPositivity(_positivityMultiplier);
    }

    public override string Info(Person person)
    {
        return "In case of death, Trust will drop by " + (-person.TrustChangeDeath(_negativityMultiplier, true)).ToString("0") +
            "\nIn case of recovery, Trust will increase by " + person.TrustChangeRecover(_positivityMultiplier).ToString("0");
    }
}