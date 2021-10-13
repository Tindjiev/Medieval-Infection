using UnityEngine;
using RandNM;

public abstract class PlagueBase : MonoBehaviour
{
    protected Village _village;

    [Zenject.Inject]
    public void Construct(Village village, DayNightCycle dayCycle)
    {
        _village = village;
        dayCycle.AfterDayEvent.AddListener(ConcludeInfected);
        dayCycle.AfterDayEvent.AddListener(Spread);
    }

    [SerializeField]
    protected PlagueSO plagueInfo;

    public RandFloat MortalityChancePerDay { get; protected set; }
    public RandFloat RecoveryChancePerDay { get; protected set; }

    private void Awake()
    {
        MortalityChancePerDay = new RandFloat(plagueInfo.MortalityChancePerDay);
        RecoveryChancePerDay = new RandFloat(plagueInfo.RecoveryChancePerDay);
    }

    public float Spreadness => plagueInfo.Spreadness;

    public int MaxHousesInfect => plagueInfo.MaxHousesInfect;


    public abstract void Spread();
    public virtual void ConcludeInfected()
    {
        _village.ConcludeInfected();
    }
    protected abstract float CalcChanceForInfectionTotal(Building infectedBuilding, Building buildingToInfect);


    protected float CalcChanceFromDistanceInverseSquare(Building building1, Building building2)
    {
        float distanceSq = (building1.position - building2.position).sqrMagnitude;
        return 0.4f * 100f / distanceSq * Spreadness;
    }

}
