using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DailyInfo : MonoBehaviour
{
    private Village _village;
    //private DayNightCycle _day;

    [Zenject.Inject]
    public void Construct(Village village, DayNightCycle day)
    {
        _village = village;
        //_day = day;
        day.UpdateValuesEvent.AddListener(UpdateValues);
    }


    private TextMeshProUGUI _dayText, _totalText, _infectedText, _deathText, _trustText;

    private void Awake()
    {
        foreach(Transform children in transform)
        {
            if(_dayText == null && children.name.Contains("ay"))
            {
                _dayText = children.GetChild(1).GetComponent<TextMeshProUGUI>();
            }
            else if (_totalText == null && children.name.Contains("otal"))
            {
                _totalText = children.GetChild(1).GetComponent<TextMeshProUGUI>();
            }
            else if(_infectedText == null && children.name.Contains("nfect"))
            {
                _infectedText = children.GetChild(1).GetComponent<TextMeshProUGUI>();
            }
            else if(_deathText == null && children.name.Contains("ea"))
            {
                _deathText = children.GetChild(1).GetComponent<TextMeshProUGUI>();
            }
            else if (_trustText == null && children.name.Contains("rust"))
            {
                _trustText = children.GetChild(1).GetComponent<TextMeshProUGUI>();
            }
        }
    }

    public void UpdateValues()
    {
        _dayText.text = DayNightCycle.DayCount.ToString();
        _totalText.text = _village.TotalResidents.ToString();
        _infectedText.text = _village.InfectedResidents.ToString();
        _deathText.text = _village.DeadResidents.ToString();
        _trustText.text = _village.Trust.ToString("0");
    }



}
