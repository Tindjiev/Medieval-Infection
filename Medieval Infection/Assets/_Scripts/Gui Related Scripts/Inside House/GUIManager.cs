using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIManager : MonoBehaviour
{
    private InsideSetupResidents _residentsGUI;
    private InsideSetupActions _actionsGUI;
    private InsideHouse _house;

    private bool _isViewingResident;

    [Zenject.Inject]
    public void Construct(InsideSetupResidents residentsGUI, InsideSetupActions actionsGUI,InsideHouse insideHouse)
    {
        _residentsGUI = residentsGUI;
        _actionsGUI = actionsGUI;
        _house = insideHouse;
    }

    private void Awake()
    {
        GoBack();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isViewingResident)
            {
                GoBack();
            }
            else
            {
                ExitHouse();
            }
        }
    }

    public void GoBack()
    {
        _actionsGUI.Deactivate();
        _residentsGUI.gameObject.SetActive(true);
        _isViewingResident = false;
    }

    public void MoveToAction(Person resident)
    {
        _residentsGUI.gameObject.SetActive(false);
        _actionsGUI.Activate(resident);
        _isViewingResident = true;
        //Debug.Log("Going to Action menu for " + resident);
    }

    public void SelectAction(Person resident, Action action)
    {
        resident.ActionToBeTaken = action;
    }

    public void ExitHouse()
    {
        MySceneManager.UnloadHouseScene(_house.House);
    }


}
