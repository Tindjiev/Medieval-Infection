using UnityEngine;
using UnityEngine.Events;

public class DayNightCycle : MonoBehaviour
{
    public static bool DebugOn = true;

    private Village _village;

    [Zenject.Inject]
    public void Construct(Village village)
    {
        _village = village;
    }

    public static int DayCount { get; private set; }

    public static bool ChangingDay { get; private set; }

    public UnityEvent EndayEvent { get; } = new UnityEvent();
    public UnityEvent AfterDayEvent { get; } = new UnityEvent();
    public UnityEvent NewDayEvent { get; } = new UnityEvent();
    public UnityEvent UpdateValuesEvent { get; } = new UnityEvent();

    private void Awake()
    {
        DayCount = 1;
        ChangingDay = false;
    }

    private void Start()
    {
        Invoke("ShamefullMethod", Time.deltaTime * 1.1f);
    }

    void ShamefullMethod()
    {
        UpdateValuesEvent.Invoke();
    }

    private void Update()
    {
        if (DebugOn && !ChangingDay)
        {
            if (new InputNM.Inputstruct(Input.GetKeyDown, new KeyCode[] { KeyCode.Return, KeyCode.KeypadEnter }).CheckInput())
            {
                ChangeDay();
                FindObjectOfType<Pause>()?.UnPauseGame();
            }
        }
    }

    private void endDay()
    {
        ChangingDay = true;
        EndayEvent.Invoke();
        DayCount++;
    }

    private bool afterDay()
    {
        AfterDayEvent.Invoke();
        return DecideToEnd();
    }

    private void newDay()
    {
        NewDayEvent.Invoke();
        ChangingDay = false;
        UpdateValuesEvent.Invoke();
    }

    public void ChangeDay()
    {
        if (ChangingDay)
        {
            return;
        }
        endDay();
        if (afterDay())
        {
            return;
        }
        newDay();
    }


    private bool DecideToEnd()
    {
        EndType endType = default;
        if (_village.Trust == -100f)
        {
            endType = EndType.VillagerKick;
        }
        if (_village.DeathPercentage > 0.6f)
        {
            endType = EndType.ManyDeaths;
        }
        if (_village.InfectedResidents == 0)
        {
            endType = EndType.PlagueEnd;
        }
        if (endType != default)
        {
            MySceneManager.LoadEnd(endType, _village.Trust, _village.DeadResidents, _village.InfectedResidents, _village.Recoveries, _village.Infections);
            return true;
        }
        return false;
    }
}
