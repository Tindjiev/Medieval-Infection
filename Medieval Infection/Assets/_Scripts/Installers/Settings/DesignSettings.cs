using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "DesignSettings", menuName = "Installers/DesignSettings")]
public class DesignSettings : ScriptableObjectInstaller<DesignSettings>
{
    public override void InstallBindings()
    {
    }
}