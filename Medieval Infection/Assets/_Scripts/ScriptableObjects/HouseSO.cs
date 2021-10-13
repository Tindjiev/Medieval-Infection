using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "House Info", menuName = "Game Related/House")]
public class HouseSO : ScriptableObject
{
    public int NumberOfResidents_Elderly = 2;
    public int NumberOfResidents_MiddleAged = 2;
    public int NumberOfResidents_Young = 1;
    public int NumberOfResidents_Infants = 2;

    public float Infectiveness = 1f;                  //how likely it is to get infected
    public float PlagueSpreadnessWithinHouse = 1f;  //a factor of how fast plague will spread inside the house

    public float ResidanceValue = 0.5f;            //the status/prestige of residents all together
    public float ResidanceCooperation = 0.5f;        //how likely are the residents to listen to the doctor
    public float ResidanceAcceptance = 1f;         //how well the residents react to the doctor's actions



}
