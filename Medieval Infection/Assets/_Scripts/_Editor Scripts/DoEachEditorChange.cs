using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DoEachEditorChange : MonoBehaviour
{
#if UNITY_EDITOR

    private void Awake()
    {
        if (Application.isPlaying)
        {
            enabled = false;
        }
    }

    void Update()
    {
        SetHouseLightsCentres();
    }

    private void SetHouseLightsCentres()
    {
        foreach(var HouseLightManager in FindObjectsOfType<HouseWindowLightsManager>())
        {
            HouseLightManager.SetCentres();
        }
    }
#endif
}
