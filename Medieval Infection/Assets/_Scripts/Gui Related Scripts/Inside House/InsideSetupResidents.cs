using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class InsideSetupResidents : MonoBehaviour
{

    private InsideHouse _houseInfo;
    private GUIManager _manager;

    [Zenject.Inject]
    public void Construct(InsideHouse houseInfo, GUIManager guiManagser)
    {
        _houseInfo = houseInfo;
        _manager = guiManagser;
    }

    private Transform[] _residentsButton;

    public void Start()
    {
        SetupButtons();
    }

    private void OnEnable()
    {
        LabelButtons();
    }


    public void SetupButtons()
    {
        CreateButtons();
        LabelButtons();
    }

    public void CreateButtons()
    {
        _residentsButton = new Transform[_houseInfo.Count()];
        Transform original = transform.Find("Residents").GetChild(0);
        if (_residentsButton.Length == 0)
        {
            original.gameObject.SetActive(false);
            return;
        }
        original.gameObject.SetActive(true);
        _residentsButton[0] = original;
        for (int i = 1; i < _residentsButton.Length; i++)
        {
            _residentsButton[i] = Instantiate(_residentsButton[0], _residentsButton[0].parent);
        }
    }

    /*
    
        _residentsButton = new Transform[_houseInfo.Count()];
        Transform original = transform.Find("Residents").GetChild(0);
        if (_residentsButton.Length == 0)
        {
            original.gameObject.SetActive(false);
            return;
        }
        original.gameObject.SetActive(true);
        _residentsButton[0] = original;
        if (original.parent.childCount > _residentsButton.Length)
        {
            for (int i = original.parent.childCount - 1; i >= _residentsButton.Length; i--)
            {
                Destroy(original.parent.GetChild(i).gameObject);
            }
        }
        else
        {
            for (int i = original.parent.childCount; i < _residentsButton.Length; i++)
            {
                _residentsButton[i] = Instantiate(_residentsButton[0], _residentsButton[0].parent);
            }
        }
    */

    public void LabelButtons()
    {
        if (_residentsButton == null || _residentsButton.Length == 0)
        {
            return;
        }
        foreach (var resident in _houseInfo.Zip(_residentsButton, (r, g) => new { real = r, gui = g }))
        {
            Button button = resident.gui.GetComponentInChildren<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(new PersonTemp(resident.real, _manager).GoToActions);
            resident.gui.GetComponentInChildren<TextMeshProUGUI>().text = resident.real.ToString();
            resident.gui.GetComponentInChildren<Image>().color = resident.real.Infected ? (resident.real.ActionToBeTaken == null ? Color.red : new Color(1f, 0.5f, 0f)) : Color.green;
        }
    }

    private class PersonTemp
    {
        private readonly Person _person;
        private readonly GUIManager _manager;

        public PersonTemp(Person person, GUIManager manager)
        {
            _person = person;
            _manager = manager;
        }
        public void GoToActions()
        {
            _manager.MoveToAction(_person);
        }
    }



}
