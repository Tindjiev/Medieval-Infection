using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "ControlSettings", menuName = "Installers/ControlSettings")]
public class ControlSettings : ScriptableObjectInstaller<ControlSettings>
{
    public override void InstallBindings()
    {
    }
}