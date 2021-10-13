using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFPS : MonoBehaviour
{

    private GUIStyle _style = new GUIStyle();

    private float _avg0 = 0f;
    private float _avg1 = 0f;

    private void Awake()
    {
        _style.alignment = TextAnchor.UpperLeft;
        _style.normal.textColor = Color.white;
        _style.fontSize = 20;
        Application.targetFrameRate = Screen.currentResolution.refreshRate;
    }

    void OnGUI()
    {
        //_style.fontSize = Screen.width / 100;
        _avg0 += ((Time.deltaTime / Time.timeScale) - _avg0) * 0.03f;
        _avg1 += (_avg0 - _avg1) * 0.03f;
        GUI.Label(new Rect(0f, 0f, 0f, 0f), string.Format("{0:0.0} ms ({1} fps)", _avg1 * 1000f, (int)(1f / _avg1 + 0.5f)), _style);
    }
}
