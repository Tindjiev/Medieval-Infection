using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InsideHouse : MonoBehaviour, IEnumerable<Person>
{
    public House House { get; private set; }

    private void Awake()
    {
        loadScene_WhichHouse();
    }

    private void setupResidentsForTest()
    {
        Debug.Log("null house");
        /*
        Residents = new List<Person>();
        for (int i = 0; i < 2; i++)
        {
            Residents.Add(new Elder(null));
        }
        for (int i = 0; i < 2; i++)
        {
            Residents.Add(new MiddleAged(null));
        }
        for (int i = 0; i < 1; i++)
        {
            Residents.Add(new Young(null));
        }
        for (int i = 0; i < 2; i++)
        {
            Residents.Add(new Infant(null));
        }

        for(int i = 0; i < (Residents.Count >> 1); i++)
        {
            Residents.GetRandomElement().GetInfected();
        }
        */
    }

    public Person GetResident(int index)
    {
        return House.GetResident(index);
    }

    public IEnumerator<Person> GetEnumerator()
    {
        return House.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }


    private static House static_house;

    public static void StoreParametersEnteringHouse(params object[] parameters)
    {
        static_house = parameters[0].As<House>();
    }

    private void loadScene_WhichHouse()
    {
        if (static_house == null)
        {
            setupResidentsForTest();
            return;
        }
        House = static_house;
        static_house = null;
    }

}
