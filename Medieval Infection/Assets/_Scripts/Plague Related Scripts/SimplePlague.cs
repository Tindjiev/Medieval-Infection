using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimplePlague : PlagueBase
{
    public override void Spread()
    {
        _village.SpreadWithinBuildings();

        int newInfections = 0;
        List<int> indexes = new List<int>();
        foreach (Building building in _village)
        {
            if (building.IsInfected())
            {
                indexes.Add(newInfections);
            }
            newInfections++;
        }
        newInfections = 0;
        foreach (int index in indexes)
        {
            if (InfectOtherBuildings(_village[index]))
            {
                newInfections++;
                if (MaxHousesInfect > 0 && newInfections > MaxHousesInfect)
                {
                    break;
                }
            }
        }
    }

    private bool InfectOtherBuildings(Building infectedBuilding)
    {
        bool newInfection = false;
        foreach(Building building in _village)
        {
            if (building.TotalResidents > 0)
            {
                float chance = CalcChanceForInfectionTotal(infectedBuilding, building);
                if (RandNM.Rand.RollDice(chance))
                {
                    building.GetInfected();
                    newInfection = true;
                }
            }
        }
        return newInfection;
    }

    protected override float CalcChanceForInfectionTotal(Building infectedBuilding, Building buildingToInfect)
    {
        return CalcChanceFromDistanceInverseSquare(infectedBuilding, buildingToInfect);
    }


}
