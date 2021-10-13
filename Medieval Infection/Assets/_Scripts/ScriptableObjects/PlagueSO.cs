using UnityEngine;
using RandNM;

[CreateAssetMenu(fileName = "Plague Info", menuName = "Game Related/Plague")]
public class PlagueSO : ScriptableObject
{

    public float MortalityChancePerDay = 0.5f;
    public float RecoveryChancePerDay = 0.55f;

    public float Spreadness = 0.8f;

    public int MaxHousesInfect = 3;


}
