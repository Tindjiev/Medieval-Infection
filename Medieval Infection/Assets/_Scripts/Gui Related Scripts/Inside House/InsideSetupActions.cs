using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using MathNM;

public class InsideSetupActions : MonoBehaviour
{
    private static readonly Color _actionNotSelectedColor = Color.white;
    private static readonly Color _actionSelectedColor = Color.yellow;

    private Transform[] _actionsButton;

    private Person _resident;

    [SerializeField]
    private TextMeshProUGUI _infoTextAction;
    [SerializeField]
    private TextMeshProUGUI _infoTextResident;


    private List<Action> _actions;

    //private void Start()
    //{
    //    _infoText.gameObject.SetActive(false);
    //}

    private void SetupActions()
    {
        CreateActionButtons();
        LabelActions();
    }


    private void CreateActionButtons()
    {
        CreateActionList();
        _actionsButton = new Transform[_actions.Count];
        Transform actionsParent = transform.Find("Actions");
        for (int i = 1; i < actionsParent.childCount; i++)
        {
            Destroy(actionsParent.GetChild(i).gameObject);
        }
        Transform original = actionsParent.GetChild(0);
        if (_actionsButton.Length == 0)
        {
            original.gameObject.SetActive(false);
            return;
        }
        original.gameObject.SetActive(true);
        _actionsButton[0] = original;
        for (int i = 1; i < _actionsButton.Length; i++)
        {
            _actionsButton[i] = Instantiate(_actionsButton[0], _actionsButton[0].parent);
        }
    }

    private void CreateActionList()
    {
        if (_resident.Infected)
        {
            _actions = new List<Action>(ActionsManager.Actions);
            _actions.RemoveAll(x => DayNightCycle.DayCount < x.AfterWhatDayCanBeApplied);
            _actions.RemoveAll(x => !x.Condition(_resident));
        }
        else
        {
            _actions = new List<Action>();
        }
    }

    private void LabelActions()
    {
        UpdateAfterChange();
        foreach (var action in _actions.Zip(_actionsButton, (r, g) => new { real = r, gui = g }))
        {
            Button button = action.gui.GetComponentInChildren<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(new ActionTemp(action.real, _resident).SetAction);
            button.onClick.AddListener(UpdateAfterChange);
            action.gui.GetComponentInChildren<TextMeshProUGUI>().text = action.real.Name;
        }
    }

    private void UpdateAfterChange()
    {
        HideInfoAction();
        foreach (var action in _actions.Zip(_actionsButton, (r, g) => new { real = r, gui = g }))
        {
            if (action.real == _resident.ActionToBeTaken)
            {
                action.gui.GetComponentInChildren<Image>().color = _actionSelectedColor;
                ShowInfoAction(action.real.Info(_resident));
            }
            else
            {
                action.gui.GetComponentInChildren<Image>().color = _actionNotSelectedColor;
            }
        }
        //foreach (Image image in transform.Find("Actions").GetComponentsInChildren<Image>())
    }

    private class ActionTemp
    {
        private readonly Action _action;
        private readonly Person _person;

        public ActionTemp(Action action, Person person)
        {
            _action = action;
            _person = person;
        }

        public void SetAction()
        {
            if (_person.ActionToBeTaken == _action)
            {
                _action.UnChooseAction(_person);
            }
            else if(_person.ActionToBeTaken != null)
            {
                _person.ActionToBeTaken.ReplaceAction(_person, _action);
            }
            else
            {
                _action.ChooseAction(_person);
            }
        }
    }

    private void ShowInfoAction(string text)
    {
        _infoTextAction.text = text;
        _infoTextAction.gameObject.SetActive(true);
    }

    private void HideInfoAction()
    {
        _infoTextAction.gameObject.SetActive(false);
    }

    private void SetupInfoResident()
    {
        if (_resident.Infected)
        {
            _infoTextResident.text = _resident + ":\nCurrent Chance to die: " + _resident.CalcChanceToDie() +
                                    "\nTrust will drop by " + (-_resident.TrustChangeDeath()).ToString("0") +
                                    "\n\nCurrent Chance to recover: " + _resident.CalcChanceToRecover() +
                                    "\nTrust will increase by " + _resident.TrustChangeRecover().ToString("0") +
                                    "\n\n" + (_resident.DaysInfected + 1).ToString(true, true).Capitalize1stLetter() + " day of infection" +
                                    (_resident.HasBeenInfectedBefore ? "\n\nThey had recovered from the Plague\nbut it seems it fell upon them again,\nsomething that is not very common..." : "");
            /*
            _infoTextResident.ForceMeshUpdate();

            TMP_WordInfo info = _infoTextResident.textInfo.wordInfo[0];
            Debug.Log(info.characterCount);
            Debug.Log(_infoTextResident.textInfo.wordInfo.Length);
            for (int i = 0; i < info.characterCount; i++)
            {
                int charIndex = info.firstCharacterIndex + i;
                int meshIndex = _infoTextResident.textInfo.characterInfo[charIndex].materialReferenceIndex;
                int vertexIndex = _infoTextResident.textInfo.characterInfo[charIndex].vertexIndex;

                Color32[] vertexColors = _infoTextResident.textInfo.meshInfo[meshIndex].colors32;

                for (int ii = 0; ii < 4; ii++)
                {
                    Debug.Log(vertexColors[ii + vertexIndex]);
                    vertexColors[ii + vertexIndex] = new Color32(255, 0, 0, 255);
                    Debug.Log(vertexColors[ii + vertexIndex]);
                }

            }

            _infoTextResident.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
            Debug.Log(_infoTextResident.textInfo.meshInfo[_infoTextResident.textInfo.characterInfo[_infoTextResident.textInfo.wordInfo[0].firstCharacterIndex].materialReferenceIndex].colors32[0]);
            _infoTextResident.ForceMeshUpdate(false);
            Debug.Log(_infoTextResident.textInfo.meshInfo[_infoTextResident.textInfo.characterInfo[_infoTextResident.textInfo.wordInfo[0].firstCharacterIndex].materialReferenceIndex].colors32[0]);
            */
        }
        else
        {
            _infoTextResident.text = "Resident seems healthy" + (_resident.HasBeenInfectedBefore? "\n\nThey were infected in the previous days,\nbut they recovered" : "");
        }
    }

    public void Activate(Person resident)
    {
        _resident = resident;
        SetupInfoResident();
        SetupActions();
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        _resident = null;
        gameObject.SetActive(false);
    }

}
