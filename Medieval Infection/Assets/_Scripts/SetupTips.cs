using UnityEngine;
using MathNM;
using TMPro;

public class SetupTips : MonoBehaviour
{
    private const string REPLACE = "^&*";
    private void Awake()
    {

        TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
        string str = text.text;

        str = str.Replace(REPLACE + 1, Person.MIN_DAYS_TILL_JUDGEMENT.ToString(true));
        str = str.Replace(REPLACE + 2, Person.MAX_DAYS_TILL_JUDGEMENT.ToString(true));
        str = str.Replace(REPLACE + 3, Person.MIN_DAYS_TILL_INFECTED_AGAIN.ToString(true));

        text.text = str;
    }
}
