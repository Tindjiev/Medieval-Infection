using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Ending : MonoBehaviour
{
    private EndType _edningType = default;
    private int _deaths;
    private int _activeInfections;
    private int _recoveries;
    private int _totalInfections;
    private float _trust;

    private void Awake()
    {
        loadScene_setStats();
    }

    private void Start()
    {
        GameObject.Find("Day Count").GetComponent<TextMeshProUGUI>().text += DayNightCycle.DayCount.ToString();
        setStatsText();
        TextMeshProUGUI text = GameObject.Find("Ending Text").GetComponent<TextMeshProUGUI>();
        switch (_edningType)
        {
            case EndType.PlagueEnd:
                text.text = "The plague has ended and the villagers thank you as you leave...";
                break;
            case EndType.ManyDeaths:
                text.text = "Too many deaths have occured in this village, it is pointless to continue";
                break;
            case EndType.VillagerKick:
                text.text = "You were forced to leave as the villagers' distrust grew too much";
                break;
            default:
                text.text = "For some reason you just left the village";
                break;
        }
    }


    private void setStatsText()
    {
        GameObject.Find("Stats").GetComponent<TextMeshProUGUI>().text = "Deaths: " + _deaths +
                                                                        "\nInfected residents: " + _activeInfections +
                                                                        "\nTotal recoveries: " + _recoveries +
                                                                        "\n Total infections: " + _totalInfections +
                                                                        "\nTrust: " + _trust.ToString("0");
    }



    public void ExitGame()
    {
        MySceneManager.ExitGame();
    }

    public void PlayAgain()
    {
        MySceneManager.LoadStart();
    }


    private static object[] static_parameters;

    public static void StoreParameters_Ending(params object[] parameters)
    {
        static_parameters = parameters;
    }

    private void loadScene_setStats()
    {
        static_parameters[0].As(out _edningType);
        static_parameters[1].As(out _trust);
        static_parameters[2].As(out _deaths);
        static_parameters[3].As(out _activeInfections);
        static_parameters[4].As(out _recoveries);
        static_parameters[5].As(out _totalInfections);
        static_parameters = null;
    }

}



public enum EndType
{
    Null,
    PlagueEnd,
    ManyDeaths,
    VillagerKick
}