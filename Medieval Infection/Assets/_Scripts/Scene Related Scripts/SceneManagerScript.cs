using ModestTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MySceneManager
{

    //public static House HouseEntering { get; private set; } = null;
    //public static Door DoorExiting { get; private set; } = null;

    public static GameObject SecondaryCamera;
    private static List<GameObject> _parentGameObjects = new List<GameObject>();

    public static void LoadHouseScene(params object[] parameters)
    {
        InsideHouse.StoreParametersEnteringHouse(parameters);
        foreach(GameObject go in Object.FindObjectsOfType<GameObject>())
        {
            if (go.name == "Secondary Camera")
            {
                (SecondaryCamera = go).GetComponent<Camera>().enabled = true;
            }
            else if (go.transform.parent == null && go.activeSelf)
            {
                _parentGameObjects.Add(go);
                go.SetActive(false);
            }
             
        }
        SceneManager.LoadScene("InsideHouse", LoadSceneMode.Additive);
    }

    public static void UnloadHouseScene(House house)
    {
        Player.StoreParametersExitingHouse(house.Door);
        SceneManager.UnloadSceneAsync("InsideHouse");
        foreach (GameObject go in _parentGameObjects)
        {
            go.SetActive(true);
        }
        SecondaryCamera.GetComponent<Camera>().enabled = false;
        SecondaryCamera = null;
        _parentGameObjects.Clear();
    }


    public static void LoadEnd(params object[] parameters)
    {
        Ending.StoreParameters_Ending(parameters);
        SceneManager.LoadScene("Ending");
    }

    public static void LoadStart()
    {
        SceneManager.LoadScene("Start");
    }

    public static void LoadVillageScene()
    {
        SceneManager.LoadScene("Final_Village");
    }

    public static void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}