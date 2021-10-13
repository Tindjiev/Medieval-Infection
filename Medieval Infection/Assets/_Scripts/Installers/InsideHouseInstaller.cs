using UnityEngine;
using Zenject;

public class InsideHouseInstaller : MonoInstaller
{
    [SerializeField]
    private InsideHouse _houseInfo;
    [SerializeField]
    private InsideSetupResidents _residentsGUI;
    [SerializeField]
    private InsideSetupActions _actionsGUI;
    [SerializeField]
    private GUIManager _guiManager;

    public override void InstallBindings()
    {
        InstallSingletons();
        InstallFactories();
    }

    private void InstallSingletons()
    {
        Container.Bind<InsideHouse>().FromInstance(_houseInfo).AsSingle();
        Container.Bind<InsideSetupResidents>().FromInstance(_residentsGUI).AsSingle();
        Container.Bind<InsideSetupActions>().FromInstance(_actionsGUI).AsSingle();
        Container.Bind<GUIManager>().FromInstance(_guiManager).AsSingle();
    }

    private void InstallFactories()
    {

    }
}