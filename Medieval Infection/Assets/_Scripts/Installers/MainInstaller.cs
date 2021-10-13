using UnityEngine;
using Zenject;

public class MainInstaller : MonoInstaller
{

    [SerializeField]
    private Village _villageOfScene;

    [SerializeField]
    private MapSetup _mapSetup;

    [SerializeField]
    private PlagueBase _plauge;

    [SerializeField]
    private DayNightCycle _dayCycle;

    [SerializeField]
    private DailyInfo _dayInfo;

    [SerializeField]
    private Player _player;

    [SerializeField]
    private GameObject _interactableText;

    public override void InstallBindings()
    {
        InstallSingletons();
        InstallFactories();
    }

    private void InstallSingletons()
    {
        Container.Bind<Village>().FromInstance(_villageOfScene).AsSingle();
        Container.Bind<MapSetup>().FromInstance(_mapSetup).AsSingle();
        Container.Bind<PlagueBase>().FromInstance(_plauge).AsSingle();
        Container.Bind<DayNightCycle>().FromInstance(_dayCycle).AsSingle();
        Container.Bind<DailyInfo>().FromInstance(_dayInfo).AsSingle();
        Container.Bind<Player>().FromInstance(_player).AsSingle();
        Container.Bind<GameObject>().WithId(InjectIDs.INJECT_INTERACTABLE_TEXT).FromInstance(_interactableText).AsSingle();
    }

    private void InstallFactories()
    {
        
    }
}

public enum InjectIDs
{
    INJECT_INTERACTABLE_TEXT
}